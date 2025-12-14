using System.Reflection;

namespace MyWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1.0",
                    Title = "MyWebAPI - User Management System",
                    Description = @"
                        <h2>🚀 ASP.NET Core 8.0 Web API</h2>
                        <p>Complete REST API for managing users with CRUD operations.</p>
                        
                        <h3>✨ Features</h3>
                        <ul>
                            <li>✅ Full CRUD operations (Create, Read, Update, Delete)</li>
                            <li>📊 Business analytics (Average Age calculation)</li>
                            <li>🔒 Data validation with error handling</li>
                            <li>📚 Complete XML documentation</li>
                        </ul>
                        
                        <h3>🌐 Base URL</h3>
                        <code>http://localhost:5137</code>
                    ",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Your Name",
                        Email = "your.email@example.com",
                        Url = new Uri("https://github.com/yourusername")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebAPI v1");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "MyWebAPI - API Documentation";
                options.DefaultModelsExpandDepth(2);
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                options.DisplayRequestDuration();
            });

            app.UseAuthorization();
            app.MapHealthChecks("/health");
            app.MapControllers();

            // ═══════════════════════════════════════════════════════════════
            // AFFICHER LES LIENS AU DÉMARRAGE
            // ═══════════════════════════════════════════════════════════════
            
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                logger.LogInformation("");
                logger.LogInformation("╔═══════════════════════════════════════════════════════════════╗");
                logger.LogInformation("║                   🚀 MyWebAPI Started! 🚀                      ║");
                logger.LogInformation("╠═══════════════════════════════════════════════════════════════╣");
                logger.LogInformation("║                                                               ║");
                logger.LogInformation("║  📚 Swagger UI Documentation:                                 ║");
                logger.LogInformation("║     🔗 http://localhost:5137/swagger                          ║");
                logger.LogInformation("║                                                               ║");
                logger.LogInformation("║  📋 OpenAPI Specification:                                    ║");
                logger.LogInformation("║     🔗 http://localhost:5137/swagger/v1/swagger.json          ║");
                logger.LogInformation("║                                                               ║");
                logger.LogInformation("║  💚 Health Check:                                             ║");
                logger.LogInformation("║     🔗 http://localhost:5137/health                           ║");
                logger.LogInformation("║                                                               ║");
                logger.LogInformation("║  👥 Users API:                                                ║");
                logger.LogInformation("║     🔗 http://localhost:5137/Users                            ║");
                logger.LogInformation("║                                                               ║");
                logger.LogInformation("║  📊 Average Age:                                              ║");
                logger.LogInformation("║     🔗 http://localhost:5137/Users/average-age                ║");
                logger.LogInformation("║                                                               ║");
                logger.LogInformation("╚═══════════════════════════════════════════════════════════════╝");
                logger.LogInformation("");
            });

            app.Run();
        }
    }
}
