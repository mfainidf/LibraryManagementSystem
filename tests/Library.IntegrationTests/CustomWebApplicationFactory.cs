using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Library.Infrastructure.Data;
using Library.API;

namespace Library.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
    // Ensure the app runs in the Testing environment so Startup skips DB registration
    builder.UseSetting("environment", "Testing");

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext and provider registrations (Sqlite) to avoid multiple provider registrations
            var descriptors = services.Where(d => d.ServiceType == typeof(DbContextOptions<LibraryDbContext>) || d.ServiceType == typeof(LibraryDbContext)).ToList();
            foreach (var d in descriptors)
            {
                services.Remove(d);
            }

            // Also remove any service descriptors related to the Sqlite EF provider
            var sqliteDescriptors = services.Where(d =>
                (d.ServiceType != null && d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("Microsoft.EntityFrameworkCore.Sqlite")) ||
                (d.ImplementationType != null && d.ImplementationType.FullName != null && d.ImplementationType.FullName.Contains("Microsoft.EntityFrameworkCore.Sqlite")) ||
                (d.ImplementationFactory != null && d.ImplementationFactory.GetType().FullName.Contains("Microsoft.EntityFrameworkCore.Sqlite"))
            ).ToList();

            foreach (var d in sqliteDescriptors)
            {
                services.Remove(d);
            }

            // Add in-memory database for testing
            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        });
    }
}
