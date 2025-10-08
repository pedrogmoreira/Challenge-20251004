using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Nodes;
using System.Text.Json;

var unifiedJsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    WriteIndented = false
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapGet("/openapi/unified.json", async (IHttpClientFactory f, IConfiguration cfg, HttpRequest req, ILogger<Program> log) =>
{
    var sources = cfg.GetSection("SwaggerSources").Get<Src[]>() ?? [];
    if (sources.Length == 0)
    {
        log.LogError("Configuration 'SwaggerSources' is missing or empty. Environment: {Env}", app.Environment.EnvironmentName);
        return Results.Problem("SwaggerSources is not configured.", statusCode: 500);
    }

    var inContainer = string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);
    var http = f.CreateClient();

    var unified = new JsonObject
    {
        ["openapi"] = "3.0.1",
        ["info"] = new JsonObject
        {
            ["title"] = "Unified API (via Gateway)",
            ["version"] = "1.0.0"
        },
        // MUITO IMPORTANTE: servers aponta para o GATEWAY, assim o Swagger chama os paths prefixados no próprio gateway
        ["servers"] = new JsonArray(new JsonObject { ["url"] = $"{req.Scheme}://{req.Host}" }),
        ["paths"] = new JsonObject(),
        ["components"] = new JsonObject
        {
            ["schemas"] = new JsonObject(),
            ["parameters"] = new JsonObject(),
            ["responses"] = new JsonObject(),
            ["requestBodies"] = new JsonObject(),
            ["securitySchemes"] = new JsonObject()
        },
        ["tags"] = new JsonArray()
    };

    var unifiedPaths = unified["paths"]!.AsObject();
    var unifiedComponents = unified["components"]!.AsObject();
    var unifiedSchemas = unifiedComponents["schemas"]!.AsObject();
    var unifiedParameters = unifiedComponents["parameters"]!.AsObject();
    var unifiedResponses = unifiedComponents["responses"]!.AsObject();
    var unifiedReqBodies = unifiedComponents["requestBodies"]!.AsObject();
    var unifiedSecSchemes = unifiedComponents["securitySchemes"]!.AsObject();
    var unifiedTags = unified["tags"]!.AsArray();

    foreach (var src in sources)
    {
        var url =
            (inContainer && !string.IsNullOrWhiteSpace(src.InternalUrl)) ? src.InternalUrl! :
            (Uri.IsWellFormedUriString(src.Url, UriKind.Absolute)) ? src.Url :
            (inContainer ? "http://gateway:8080" : $"{req.Scheme}://{req.Host}") + src.Url;

        var prefix = string.IsNullOrWhiteSpace(src.Prefix) ? "" : (src.Prefix!.StartsWith('/') ? src.Prefix : '/' + src.Prefix);

        try
        {
            log.LogInformation("Fetching Swagger. source={Name} url={Url} prefix={Prefix}", src.Name, url, prefix);

            var jsonText = await http.GetStringAsync(url);
            var doc = JsonNode.Parse(jsonText)?.AsObject();
            if (doc is null)
            {
                log.LogWarning("Parsed document is null. Skipping. source={Name}", src.Name);
                continue;
            }

            // Tag por serviço (ajuda a agrupar no UI)
            unifiedTags.Add(new JsonObject
            {
                ["name"] = src.Name,
                ["description"] = $"Operations from '{src.Name}'"
            });

            // ----- PATHS -----
            var paths = doc["paths"] as JsonObject;
            if (paths is not null)
            {
                foreach (var kv in paths)
                {
                    var originalPath = kv.Key.StartsWith('/') ? kv.Key : '/' + kv.Key;
                    var path = (prefix + originalPath).Replace("//", "/");
                    if (!unifiedPaths.ContainsKey(path))
                        unifiedPaths[path] = new JsonObject();

                    var fromOps = kv.Value as JsonObject;
                    var toOps = unifiedPaths[path]!.AsObject();
                    if (fromOps is not null)
                    {
                        foreach (var op in fromOps)
                        {
                            var opKey = op.Key; // get/post/put/delete/patch/options/head/trace
                            if (toOps.ContainsKey(opKey))
                            {
                                var altKey = opKey + $"@{src.Name}";
                                log.LogWarning("Operation collision at {Path}.{Op}. Using alt operation key {AltKey}.", path, opKey, altKey);
                                var opClone = op.Value!.DeepClone()?.AsObject();
                                if (opClone is not null)
                                {
                                    if (opClone["tags"] is null) opClone["tags"] = new JsonArray(src.Name);
                                    if (opClone["operationId"] is JsonNode oid)
                                        opClone["operationId"] = oid!.GetValue<string>() + $"@{src.Name}";
                                    else
                                        opClone["operationId"] = $"{opKey}_{path.Replace('/', '_').Trim('_')}@{src.Name}";
                                    toOps[altKey] = opClone;
                                }
                            }
                            else
                            {
                                var opClone = op.Value!.DeepClone()?.AsObject();
                                if (opClone is not null && opClone["tags"] is null)
                                    opClone["tags"] = new JsonArray(src.Name);
                                toOps[opKey] = opClone ?? op.Value!.DeepClone();
                            }
                        }
                    }
                }
            }
            else
            {
                log.LogWarning("No 'paths' section found. source={Name}", src.Name);
            }

            // ----- COMPONENTS (OpenAPI 3) -----
            var comp = doc["components"] as JsonObject;
            if (comp is not null)
            {
                void MergeChild(string key, JsonObject unifiedTarget)
                {
                    if (comp[key] is not JsonObject child) return;
                    foreach (var ckv in child)
                    {
                        var name = ckv.Key;
                        if (unifiedTarget.ContainsKey(name))
                        {
                            var alt = $"{name}@{src.Name}";
                            log.LogWarning("Component collision at {Key}.{Name}. Using {Alt}.", key, name, alt);
                            unifiedTarget[alt] = ckv.Value!.DeepClone();
                        }
                        else
                        {
                            unifiedTarget[name] = ckv.Value!.DeepClone();
                        }
                    }
                }

                MergeChild("schemas", unifiedSchemas);
                MergeChild("parameters", unifiedParameters);
                MergeChild("responses", unifiedResponses);
                MergeChild("requestBodies", unifiedReqBodies);
                MergeChild("securitySchemes", unifiedSecSchemes);
            }

            // ----- Swagger 2.0 (definitions -> components.schemas) -----
            var definitions = doc["definitions"] as JsonObject;
            if (definitions is not null)
            {
                foreach (var dkv in definitions)
                {
                    var name = dkv.Key;
                    if (unifiedSchemas.ContainsKey(name))
                    {
                        var alt = $"{name}@{src.Name}";
                        log.LogWarning("Definition collision at definitions.{Name}. Using {Alt}.", name, alt);
                        unifiedSchemas[alt] = dkv.Value!.DeepClone();
                    }
                    else
                    {
                        unifiedSchemas[name] = dkv.Value!.DeepClone();
                    }
                }
            }

            log.LogInformation("Merged Swagger successfully. source={Name}", src.Name);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to fetch/merge Swagger. source={Name} url={Url} message={Message}", src.Name, url, ex.Message);
        }
    }

    var bytes = JsonSerializer.SerializeToUtf8Bytes(unified, unifiedJsonOptions);
    return Results.Bytes(bytes, "application/json");
});


app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "docs";
    c.SwaggerEndpoint("/openapi/unified.json", "Challenge API Gateway");
});

app.MapReverseProxy();

app.MapGet("/", () => Results.Redirect("/docs"));

app.Run();

record Src(string Name, string Url, string Prefix, string? InternalUrl);
