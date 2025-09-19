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
            
            IServiceProvider? serviceProvider = null;
            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                serviceProvider = services.BuildServiceProvider();
                
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
                    await dbContext.Database.EnsureCreatedAsync();
                    await Logger.LogAsync("Database initialized");

                    var authMenu = scope.ServiceProvider.GetRequiredService<AuthenticationMenu>();
                    
                    using var cts = new System.Threading.CancellationTokenSource();
                    
                    // Create a task to monitor the keepRunning flag
                    var monitorTask = Task.Run(async () => {
                        try
                        {
                            while (_keepRunning && !cts.Token.IsCancellationRequested)
                            {
                                await Task.Delay(100, cts.Token);
                            }
                            await authMenu.Shutdown();
                        }
                        catch (OperationCanceledException)
                        {
                            // Task cancelled normally, just exit
                        }
                    }, cts.Token);

                    // Show the menu
                    await authMenu.ShowMainMenuAsync();
                    
                    // Signal cancellation and wait for the monitor task to complete
                    cts.Cancel();
                    await monitorTask;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Log solo errori non relativi alla cancellazione
                try
                {
                    await Logger.LogAsync($"Application error: {ex.Message}", LogType.Error);
                }
                catch (IOException)
                {
                    // Ignora errori di I/O durante la chiusura
                }
                throw;
            }
            catch (OperationCanceledException)
            {
                // Gestione normale della cancellazione
            }
            finally
            {
                try
                {
                    if (serviceProvider is IAsyncDisposable asyncDisposable)
                    {
                        await asyncDisposable.DisposeAsync();
                    }
                    else if (serviceProvider is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    
                    await Logger.LogAsync("Application shutting down...");
                }
                catch (Exception ex) when (ex is IOException or ObjectDisposedException)
                {
                    // Ignora errori di I/O e ObjectDisposed durante lo shutdown
                }
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Database
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlite("Data Source=library.db"));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMediaItemRepository, MediaItemRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();

            // Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IMediaItemService, MediaItemService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IGenreService, GenreService>();

            // UI
            services.AddSingleton<ConsoleManager>();
            services.AddScoped<AuthenticationMenu>();
        }
    }
}