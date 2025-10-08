using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Microservices.MotorbikeApi.AutoMapper;
using Challenge.Microservices.MotorbikeApi.Commands;
using Challenge.Microservices.MotorbikeApi.Commands.Handlers;
using Challenge.Microservices.MotorbikeApi.Commands.Validations;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories;
using Challenge.Microservices.MotorbikeApi.Infra.Messaging.Consumers;
using Challenge.Microservices.MotorbikeApi.Infra.Messaging.Publishers;
using Challenge.Microservices.MotorbikeApi.Queries;
using Challenge.Microservices.MotorbikeApi.Queries.Handlers;
using Challenge.Microservices.MotorbikeApi.Queries.Validations;
using FluentValidation;

namespace Challenge.Microservices.MotorbikeApi.Configuration
{
    /// <summary>
    /// Configuration for dependency injection registrations
    /// </summary>
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection SetupDependencyInjection(this IServiceCollection services)
        {
            services
                .SetupValidators()
                .SetupCommandHandlers()
                .SetupQueryHandlers()
                .SetupRepositories()
                .SetupMessaging()
                .SetupAutoMapper();

            return services;
        }

        private static IServiceCollection SetupValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<DeleteMotorbikeCommand>, DeleteMotorbikeCommandValidator>();
            services.AddScoped<IValidator<RegisterMotorbikeCommand>, RegisterMotorbikeCommandValidator>();
            services.AddScoped<IValidator<UpdateMotorbikeLicensePlateCommand>, UpdateMotorbikeLicensePlateCommandValidator>();
            services.AddScoped<IValidator<GetMotorbikesQuery>, GetMotorbikesQueryValidator>();
            services.AddScoped<IValidator<GetMotorbikeByIdentifierQuery>, GetMotorbikeByIdentifierQueryValidator>();

            return services;
        }

        private static IServiceCollection SetupCommandHandlers(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandler<RegisterMotorbikeCommand>, RegisterMotorbikeCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateMotorbikeLicensePlateCommand>, UpdateMotorbikeLicensePlateCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteMotorbikeCommand>, DeleteMotorbikeCommandHandler>();

            return services;
        }

        private static IServiceCollection SetupQueryHandlers(this IServiceCollection services)
        {
            services.AddScoped<IQueryHandler<GetMotorbikesQuery>, GetMotorbikesQueryHandler>();
            services.AddScoped<IQueryHandler<GetMotorbikeByIdentifierQuery>, GetMotorbikeByIdentifierQueryHandler>();

            return services;
        }

        private static IServiceCollection SetupRepositories(this IServiceCollection services)
        {
            services.AddScoped<IMotorbikeRepository, MotorbikeRepository>();
            services.AddScoped<IMotorbikeNotificationRepository, MotorbikeNotificationRepository>();

            return services;
        }

        private static IServiceCollection SetupMessaging(this IServiceCollection services)
        {
            services.AddSingleton<IMotorbikeMessagePublisher, RabbitMqMotorbikePublisher>();
            services.AddHostedService<Motorbike2024NotificationConsumer>();

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