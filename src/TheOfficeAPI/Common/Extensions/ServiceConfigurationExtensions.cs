using TheOfficeAPI.Common.Enums;
using TheOfficeAPI.Level0.Extensions;
using TheOfficeAPI.Level1.Extensions;

namespace TheOfficeAPI.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, MaturityLevel? maturityLevel)
        {
            // Add controllers
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Add Swagger based on maturity level
            if (maturityLevel == MaturityLevel.Level0)
            {
                TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.AddSwaggerServices(services);
                services.AddLevel0Services();
            }
            else if (maturityLevel == MaturityLevel.Level1)
            {
                TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.AddSwaggerServices(services);
                services.AddLevel1Services();
            }
        }

        public static void ConfigurePipeline(this WebApplication app, MaturityLevel? maturityLevel)
        {
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Use Swagger based on maturity level
            if (maturityLevel == MaturityLevel.Level0)
            {
                TheOfficeAPI.Level0.Extensions.SwaggerConfiguration.UseSwaggerMiddleware(app);
            }
            else if (maturityLevel == MaturityLevel.Level1)
            {
                TheOfficeAPI.Level1.Extensions.SwaggerConfiguration.UseSwaggerMiddleware(app);
            }

            app.UseSwagger();
            app.UseSwaggerUI();

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