using RabbitMQ.Client;

namespace Challenge.Microservices.MotorbikeApi.Configuration
{
    /// <summary>
    /// Configuration for RabbitMQ connection
    /// </summary>
    public static class RabbitMqConfig
    {
        public static IServiceCollection SetupRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = configuration.GetValue<int>("RabbitMQ:Port", 5672),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest",
                VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            var connection = factory.CreateConnection();
            services.AddSingleton(connection);

            return services;
        }
    }
}