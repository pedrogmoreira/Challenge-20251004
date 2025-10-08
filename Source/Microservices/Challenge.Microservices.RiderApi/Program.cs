using Challenge.Common.Data.Mongo.Converters;
using Challenge.Microservices.RiderApi.Configuration;
using Challenge.Microservices.RiderApi.Configuration.Options;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using static System.Net.WebRequestMethods;

namespace Challenge.Microservices.RiderApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Kestrel to support HTTP/1.1 and HTTP/2
            // TODO(PROD): Move to HTTPS + HTTP/2 (ALPN) with a valid certificate (e.g., port 8443).
            builder.WebHost.ConfigureKestrel(o =>
            {
                // DEV: gRPC over HTTP / 2 without TLS(h2c)
                o.ListenAnyIP(8080, lo => lo.Protocols = HttpProtocols.Http2);

                // DEV: REST over HTTP/1.1
                o.ListenAnyIP(8081, lo => lo.Protocols = HttpProtocols.Http1);

                // TODO(PROD):
                // o.ListenAnyIP(8443, lo =>
                // {
                //     lo.UseHttps("certs/rider-prod.pfx", "PASSWORD");     // real cert
                //     lo.Protocols = HttpProtocols.Http2;                  // gRPC w/ TLS
                // });
            });

            builder.Services
                .SetupDatabase(builder.Configuration)
                .SetupDependencyInjection()
                .SetupApiVersioning()
                .SetupSwaggerGen()
                .SetupAws(builder.Configuration, builder.Environment);

            builder.Services.AddOptions<S3Options>()
                .BindConfiguration("AwsOptions:S3")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Logging.SetupLogging();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new ObjectIdJsonConverter());
                });

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.SetupSwagger();

            app.MapGrpcServices();

            app.MapControllers();

            app.Run();
        }
    }
}
