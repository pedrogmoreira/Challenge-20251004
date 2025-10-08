using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Challenge.Microservices.RiderApi.Configuration
{
    /// <summary>
    /// Configuration for Swagger/OpenAPI documentation
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Configures Swagger generator with API documentation settings
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection SetupSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Rider API",
                    Description = "API para Gerenciamento de Riders"
                });
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                options.AddSecurityDefinition("ApiVersion", new OpenApiSecurityScheme
                {
                    Description = "API Version Header",
                    Name = "x-api-version",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });
            
            return services;
        }

        /// <summary>
        /// Configures Swagger UI middleware
        /// </summary>
        /// <param name="app">The web application</param>
        /// <returns>The web application for chaining</returns>
        public static WebApplication SetupSwagger(this WebApplication app)
        {
            app.UseSwagger();

            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Challenge API - Rider v1");
                    options.RoutePrefix = "docs";
                });
            }

            return app;
        }
    }
}
