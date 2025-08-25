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
        private static volatile bool _keepRunning = true;
        
        public static void SetKeepRunning(bool value)
        {
            _keepRunning = value;
        }
        
        public static async Task Main(string[] args)
        {
            // Gestisce la chiusura dell'applicazione quando si preme Ctrl+C
            System.Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true; // Previene la chiusura immediata
                _keepRunning = false;
            };

            await Logger.LogAsync("Application starting...");
            
            try
            {
                var services = ConfigureServices();
                
                using (var scope = services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
                    await dbContext.Database.EnsureCreatedAsync();
                    await Logger.LogAsync("Database initialized");

                    var authMenu = scope.ServiceProvider.GetRequiredService<AuthenticationMenu>();
                    
                    // Create a task to monitor the keepRunning flag
                    var monitorTask = Task.Run(async () => {
                        while (_keepRunning)
                        {
                            await Task.Delay(100);
                        }
                        await authMenu.Shutdown();
                    });

                    // Show the menu
                    await authMenu.ShowMainMenuAsync();
                    
                    // Wait for the monitor task to complete
                    await monitorTask;
                }
            }
            catch (Exception ex)
            {
                await Logger.LogAsync($"Application error: {ex.Message}", LogType.Error);
                throw;
            }
            
            await Logger.LogAsync("Application shutting down...");
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

            // UI
            services.AddScoped<AuthenticationMenu>();

            return services.BuildServiceProvider();
        }
    }
}