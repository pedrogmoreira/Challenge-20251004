using MongoDB.Driver;

namespace Challenge.Microservices.SubscriptionApi.Configuration
{
    /// <summary>
    /// Configuration for MongoDB database connection
    /// </summary>
    public static class DatabaseConfig
    {
        public static IServiceCollection SetupDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database") ??
                throw new InvalidOperationException("Database connection string missing");

            services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));

            services.AddScoped(sp =>
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("SubscriptionServiceDb");
                return database;
            });

            return services;
        }
    }
}