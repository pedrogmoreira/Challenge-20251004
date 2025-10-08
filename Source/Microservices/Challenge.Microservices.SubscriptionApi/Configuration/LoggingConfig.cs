namespace Challenge.Microservices.SubscriptionApi.Configuration
{
    /// <summary>
    /// Configuration for application logging
    /// </summary>
    public static class LoggingConfig
    {
        public static ILoggingBuilder SetupLogging(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddConsole();
            return loggingBuilder;
        }
    }
}