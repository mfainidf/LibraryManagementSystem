using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Library.Core.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Repositories;
using Library.Infrastructure.Services;
using Library.Console.Logging;

namespace Library.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = new StructuredLogger();
            
            try
            {
                var services = ConfigureServices();
                
                using (var scope = services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
                    await dbContext.Database.EnsureCreatedAsync();
                    await logger.LogAsync("Database initialized");

                    var authMenu = scope.ServiceProvider.GetRequiredService<AuthenticationMenu>();
                    await authMenu.ShowMainMenuAsync();
                }
            }
            catch (Exception ex)
            {
                await logger.LogErrorAsync(ex, "Application startup");
                throw;
            }
            
            await logger.LogAsync("Application shutting down...");
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Database
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlite("Data Source=library.db"));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Logging
            services.AddSingleton<StructuredLogger>();

            // UI
            services.AddScoped<AuthenticationMenu>();

            return services.BuildServiceProvider();
        }
    }
}