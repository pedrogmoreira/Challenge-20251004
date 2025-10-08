using Challenge.Common.Data.Mongo.Converters;
using Challenge.Microservices.SubscriptionApi.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Challenge.Microservices.SubscriptionApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var builder = WebApplication.CreateBuilder(args);

            // Configure Kestrel to support HTTP/1.1 and HTTP/2
            // TODO(PROD): Use https://... without the h2c switch and validate certificates properly.
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(8080, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                });
            });


            builder.Services
                .SetupDatabase(builder.Configuration)
                .SetupRabbitMq(builder.Configuration)
                .SetupDependencyInjection(builder.Configuration)
                .SetupApiVersioning()
                .SetupSwaggerGen();

            builder.Logging.SetupLogging();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new ObjectIdJsonConverter());
                });

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.SetupSwagger();

            app.MapControllers();

            app.Run();
        }
    }
}