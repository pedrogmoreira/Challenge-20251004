using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Challenge.Microservices.MotorbikeApi.Configuration
{
    /// <summary>
    /// Configuration for Swagger/OpenAPI documentation
    /// </summary>
    public static class SwaggerConfig
    {
        public static IServiceCollection SetupSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Motorbike API",
                    Description = "API para Gerenciamento de Motos"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        public static WebApplication SetupSwagger(this WebApplication app)
        {
            app.UseSwagger();

            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Motorbike API v1");
                    options.RoutePrefix = "docs";
                });
            }

            return app;
        }
    }
}