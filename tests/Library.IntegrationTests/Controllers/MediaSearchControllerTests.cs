using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Library.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Library.Infrastructure.Data;
using Xunit;

namespace Library.IntegrationTests.Controllers;

public class MediaSearchControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public MediaSearchControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Search_ReturnsResults_WithPaging()
    {
        // Seed data (do this before creating the client so the server sees the seeded data)
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            
            // First create a user for the CreatedByUserId foreign key
            var user = new User 
            { 
                Name = "Test User", 
                Email = "test@example.com", 
                PasswordHash = "hash", 
                Role = UserRole.Administrator 
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            
            // Now create MediaItems with proper CreatedByUserId
            context.MediaItems.AddRange(
                new MediaItem { Title = "Alpha", Author = "Author A", Genre = "Fiction", Quantity = 3, AvailableQuantity = 3, Type = MediaType.Book, CreatedByUserId = user.Id },
                new MediaItem { Title = "Beta", Author = "Author B", Genre = "Science", Quantity = 2, AvailableQuantity = 2, Type = MediaType.Book, CreatedByUserId = user.Id },
                new MediaItem { Title = "Gamma", Author = "Author C", Genre = "Fiction", Quantity = 1, AvailableQuantity = 1, Type = MediaType.DVD, CreatedByUserId = user.Id }
            );
            await context.SaveChangesAsync();
        }
        var client = _factory.CreateClient();

        // Call API
        var response = await client.GetAsync("/api/v1/media/search?title=a&page=0&pageSize=2");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<MediaItem>>();
        items.Should().NotBeNull();
        items!.Count.Should().BeGreaterThanOrEqualTo(1);
    }
}