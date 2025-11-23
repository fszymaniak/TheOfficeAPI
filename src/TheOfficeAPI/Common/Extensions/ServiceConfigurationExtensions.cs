using TheOfficeAPI.Common.Enums;
using TheOfficeAPI.Level0.Extensions;
using TheOfficeAPI.Level1.Extensions;
using TheOfficeAPI.Level2.Extensions;
using TheOfficeAPI.Level3.Extensions;

namespace TheOfficeAPI.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, MaturityLevel? maturityLevel)
        {
            // Add controllers
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Register services for ALL levels to support all API versions in Swagger
            services.AddLevel0Services();
            services.AddLevel1Services();
            services.AddLevel2Services();
            services.AddLevel3Services();

            // Add Swagger based on maturity level (all configurations now register all versions)
            if (maturityLevel == MaturityLevel.Level0)
            {
                TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.AddSwaggerServices(services);
            }
            else if (maturityLevel == MaturityLevel.Level1)
            {
                TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.AddSwaggerServices(services);
            }
            else if (maturityLevel == MaturityLevel.Level2)
            {
                TheOfficeAPI.Level2.Extensions.SwaggerConfiguration.AddSwaggerServices(services);
            }
            else if (maturityLevel == MaturityLevel.Level3)
            {
                TheOfficeAPI.Level3.Extensions.SwaggerConfiguration.AddSwaggerServices(services);
            }
        }

        public static void ConfigurePipeline(this WebApplication app, MaturityLevel? maturityLevel)
        {
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Root endpoint
            app.MapGet("/", () => Results.Ok(new {
                message = "The Office API is running",
                maturityLevel = maturityLevel?.ToString() ?? "Unknown",
                endpoints = new {
                    swagger = "/swagger",
                    api = "/api"
                }
            }));

            // Use Swagger based on maturity level
            if (maturityLevel == MaturityLevel.Level0)
            {
                TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.UseSwaggerMiddleware(app);
            }
            else if (maturityLevel == MaturityLevel.Level1)
            {
                TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.UseSwaggerMiddleware(app);
            }
            else if (maturityLevel == MaturityLevel.Level2)
            {
                TheOfficeAPI.Level2.Extensions.SwaggerConfiguration.UseSwaggerMiddleware(app);
            }
            else if (maturityLevel == MaturityLevel.Level3)
            {
                TheOfficeAPI.Level3.Extensions.SwaggerConfiguration.UseSwaggerMiddleware(app);
            }

            // Don't use HTTPS redirect on Railway/production
            if (app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}