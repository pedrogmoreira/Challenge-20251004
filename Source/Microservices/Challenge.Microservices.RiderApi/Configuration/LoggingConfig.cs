namespace Challenge.Microservices.RiderApi.Configuration
{
    /// <summary>
    /// Configuration for application logging
    /// </summary>
    public static class LoggingConfig
    {
        /// <summary>
        /// Configures logging providers for the application
        /// </summary>
        /// <param name="loggingBuilder">The logging builder</param>
        /// <returns>The logging builder for chaining</returns>
        public static ILoggingBuilder SetupLogging(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddConsole();

            return loggingBuilder;
        }
    }
}
