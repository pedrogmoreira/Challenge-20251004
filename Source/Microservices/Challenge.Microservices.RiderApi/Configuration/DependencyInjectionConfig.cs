using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Microservices.RiderApi.AutoMapper;
using Challenge.Microservices.RiderApi.Commands;
using Challenge.Microservices.RiderApi.Commands.Handlers;
using Challenge.Microservices.RiderApi.Commands.Validations;
using Challenge.Microservices.RiderApi.Infra.Data.Repositories;
using Challenge.Microservices.RiderApi.Infra.Services;
using FluentValidation;

namespace Challenge.Microservices.RiderApi.Configuration
{
    /// <summary>
    /// Configuration for dependency injection registrations
    /// </summary>
    public static class DependencyInjectionConfig
    {
        /// <summary>
        /// Configures all dependency injection registrations for the application
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection SetupDependencyInjection(this IServiceCollection services)
        {
            services
                .SetupValidators()
                .SetupCommandHandlers()
                .SetupRepositories()
                .SetupServices()
                .SetupAutoMapper()
                .SetupGrpc();

            return services;
        }

        /// <summary>
        /// Registers FluentValidation validators
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        private static IServiceCollection SetupValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<RegisterRiderCommand>, RegisterRiderCommandValidator>();
            services.AddScoped<IValidator<UploadRiderCnhImageCommand>, UploadRiderCnhImageCommandValidator>();

            return services;
        }

        /// <summary>
        /// Registers command handlers
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        private static IServiceCollection SetupCommandHandlers(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandler<RegisterRiderCommand>, RegisterRiderCommandHandler>();
            services.AddScoped<ICommandHandler<UploadRiderCnhImageCommand>, UploadRiderCnhImageCommandHandler>();

            return services;
        }

        /// <summary>
        /// Registers repository implementations
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        private static IServiceCollection SetupRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRiderRepository, RiderRepository>();

            return services;
        }

        /// <summary>
        /// Registers application services
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        private static IServiceCollection SetupServices(this IServiceCollection services)
        {
            services.AddScoped<IS3Service, S3Service>();

            return services;
        }

        /// <summary>
        /// Registers AutoMapper with mapping profiles
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        private static IServiceCollection SetupAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, typeof(MappingProfile).Assembly);
            
            return services;
        }
    }
}
