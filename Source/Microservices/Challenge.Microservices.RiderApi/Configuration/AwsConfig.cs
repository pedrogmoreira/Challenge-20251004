using Amazon.Runtime;
using Amazon.S3;
using Challenge.Microservices.RiderApi.Configuration.Options;
using Challenge.Microservices.RiderApi.Infra.Services;

namespace Challenge.Microservices.RiderApi.Configuration
{
    /// <summary>
    /// Extension methods for configuring AWS services
    /// </summary>
    public static class AwsConfig
    {
        /// <summary>
        /// Configures AWS S3 services with environment-specific settings
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <param name="environment">Hosting environment</param>
        /// <returns>Service collection for chaining</returns>
        public static IServiceCollection SetupAws(this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
        {
            IAmazonS3 s3Client;

            var serviceUrl = configuration["AWS:ServiceUrl"];

            // If ServiceUrl is configured, use MinIO
            if (environment.IsDevelopment() && !string.IsNullOrEmpty(serviceUrl))
            {
                var credentials = new BasicAWSCredentials(
                    configuration["AWS:AccessKey"]!,
                    configuration["AWS:SecretKey"]!
                );

                var config = new AmazonS3Config
                {
                    ServiceURL = serviceUrl,
                    ForcePathStyle = true, // Required for MinIO
                    UseHttp = configuration.GetValue<bool>("AWS:UseHttp")
                };

                s3Client = new AmazonS3Client(credentials, config);
            }
            else
            {
                // Use real AWS S3
                var awsOptions = configuration.GetAWSOptions();

                // If explicit credentials are provided (CI/CD)
                var accessKey = configuration["AWS:AccessKey"];
                var secretKey = configuration["AWS:SecretKey"];

                if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
                {
                    awsOptions.Credentials = new BasicAWSCredentials(accessKey, secretKey);
                }
                // Otherwise, use IAM Role or environment variables

                s3Client = awsOptions.CreateServiceClient<IAmazonS3>();
            }

            services.AddSingleton(s3Client);

            // Configure S3 options (BucketName, etc)
            services.Configure<S3Options>(
                configuration.GetSection("S3"));

            // Register services
            services.AddScoped<IS3Service, S3Service>();

            return services;
        }
    }
}
