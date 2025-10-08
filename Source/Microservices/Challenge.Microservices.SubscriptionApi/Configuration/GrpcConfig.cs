using Challenge.Common.Messaging.Grpc;
using Challenge.Microservices.SubscriptionApi.Infra.Services;

namespace Challenge.Microservices.SubscriptionApi.Configuration
{
    /// <summary>
    /// Configuration for gRPC clients
    /// </summary>
    public static class GrpcConfig
    {
        /// <summary>
        /// Configures gRPC clients
        /// </summary>
        public static IServiceCollection SetupGrpc(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure gRPC client for RiderService
            services.AddGrpcClient<RiderService.RiderServiceClient>(options =>
            {
                var riderApiUrl = configuration["Services:RiderApi:GrpcUrl"]
                    ?? "http://rider:8080";

                options.Address = new Uri(riderApiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(_ => new SocketsHttpHandler
            {
                // Single handler for DEV; allows multiple HTTP/2 connections if needed
                EnableMultipleHttp2Connections = true
            });

            // TODO(PROD):
            // - Publish Rider with HTTPS + HTTP/2 (e.g., https://rider:8443).
            // - Remove the h2c switch above.
            // - Configure the client to use the https URL:
            //     var riderUrl = builder.Configuration["Services:RiderApi:GrpcUrl"] ?? "https://rider:8443";
            // - For dev/staging with self-signed certs only, you may relax validation:
            //     .ConfigurePrimaryHttpMessageHandler(_ => new SocketsHttpHandler
            //     {
            //         EnableMultipleHttp2Connections = true,
            //         SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            //         {
            //             RemoteCertificateValidationCallback = static (_, __, ___, ____) => true
            //         }
            //     });

            // Register the service that uses gRPC
            services.AddScoped<IRiderValidationService, RiderValidationService>();

            return services;
        }
    }
}