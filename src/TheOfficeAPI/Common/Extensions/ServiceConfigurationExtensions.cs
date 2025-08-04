using TheOfficeAPI.Common.Enums;
using TheOfficeAPI.Level0.Extensions;
using TheOfficeAPI.Level0.Services;

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
                services.AddSwaggerServices();
            }

            // Register our custom services
            services.AddSingleton<TheOfficeService>();
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
                app.UseSwaggerMiddleware();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}