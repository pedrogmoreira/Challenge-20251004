using MongoDB.Driver;

namespace Challenge.Microservices.RiderApi.Configuration
{
    /// <summary>
    /// Configuration for MongoDB database connection
    /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// Configures MongoDB client and database instance
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <returns>The service collection for chaining</returns>
        /// <exception cref="InvalidOperationException">Thrown when connection string is missing</exception>
        public static IServiceCollection SetupDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database") ??
                throw new InvalidOperationException("Database connection string missing");

            services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));

            services.AddScoped(sp =>
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("RiderServiceDb");
                return database;
            });

            return services;
        }
    }
}
