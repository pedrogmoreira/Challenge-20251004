
using Challenge.Microservices.RiderApi.Infra.Services;

namespace Challenge.Microservices.RiderApi.Configuration
{
    /// <summary>
    /// Configuration for gRPC services
    /// </summary>
    public static class GrpcConfig
    {
        /// <summary>
        /// Configures gRPC services
        /// </summary>
        public static IServiceCollection SetupGrpc(this IServiceCollection services)
        {
            services.AddGrpc();

            return services;
        }

        /// <summary>
        /// Maps gRPC endpoints
        /// </summary>
        public static WebApplication MapGrpcServices(this WebApplication app)
        {
            app.MapGrpcService<RiderGrpcService>();

            return app;
        }
    }
}