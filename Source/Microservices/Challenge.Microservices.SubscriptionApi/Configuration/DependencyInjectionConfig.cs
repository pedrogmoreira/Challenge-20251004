using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Microservices.SubscriptionApi.AutoMapper;
using Challenge.Microservices.SubscriptionApi.Commands;
using Challenge.Microservices.SubscriptionApi.Commands.Handlers;
using Challenge.Microservices.SubscriptionApi.Commands.Validations;
using Challenge.Microservices.SubscriptionApi.Infra.Data.Repositories;
using Challenge.Microservices.SubscriptionApi.Infra.Messaging.Consumers;
using Challenge.Microservices.SubscriptionApi.Infra.Services;
using Challenge.Microservices.SubscriptionApi.Queries;
using Challenge.Microservices.SubscriptionApi.Queries.Handlers;
using Challenge.Microservices.SubscriptionApi.Queries.Validations;
using FluentValidation;

namespace Challenge.Microservices.SubscriptionApi.Configuration
{
    /// <summary>
    /// Configuration for dependency injection registrations
    /// </summary>
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection SetupDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .SetupValidators()
                .SetupCommandHandlers()
                .SetupQueryHandlers()
                .SetupMessaging()
                .SetupRepositories()
                .SetupServices()
                .SetupGrpc(configuration)
                .SetupAutoMapper();

            return services;
        }

        private static IServiceCollection SetupValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CreateSubscriptionCommand>, CreateSubscriptionCommandValidator>();
            services.AddScoped<IValidator<InformReturnDateCommand>, InformReturnDateCommandValidator>();
            services.AddScoped<IValidator<GetSubscriptionQuery>, GetSubscriptionQueryValidator>();

            return services;
        }

        private static IServiceCollection SetupCommandHandlers(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandler<CreateSubscriptionCommand>, CreateSubscriptionCommandHandler>();
            services.AddScoped<ICommandHandler<InformReturnDateCommand>, InformReturnDateCommandHandler>();

            return services;
        }

        private static IServiceCollection SetupQueryHandlers(this IServiceCollection services)
        {
            services.AddScoped<IQueryHandler<GetSubscriptionQuery>, GetSubscriptionQueryHandler>();

            return services;
        }

        private static IServiceCollection SetupMessaging(this IServiceCollection services)
        {
            services.AddHostedService<ActiveRentalsCheckConsumer>();

            return services;
        }

        private static IServiceCollection SetupRepositories(this IServiceCollection services)
        {
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

            return services;
        }

        private static IServiceCollection SetupServices(this IServiceCollection services)
        {
            services.AddScoped<ISubscriptionCostService, SubscriptionCostService>();
            services.AddScoped<IRiderValidationService, RiderValidationService>();

            return services;
        }

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