using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Library.Core.Interfaces;
using Library.Core.Models;
using Library.Infrastructure.Data;
using Library.Infrastructure.Repositories;
using Library.Infrastructure.Services;

var services = new ServiceCollection();

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

var serviceProvider = services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    
    var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
    var mediaItemService = scope.ServiceProvider.GetRequiredService<IMediaItemService>();
    var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();
    var genreService = scope.ServiceProvider.GetRequiredService<IGenreService>();
    
    Console.WriteLine("🔧 Creating sample data...");
    
    // Create admin user if not exists
    var adminEmail = "admin@library.com";
    try
    {
        var admin = await authService.RegisterAsync(adminEmail, "Admin123!", "Admin User", UserRole.Administrator);
        Console.WriteLine($"✅ Admin user created: {adminEmail}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ℹ️ Admin user might already exist: {ex.Message}");
    }
    
    // Get admin user ID
    var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var adminUser = await userRepo.GetByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        Console.WriteLine("❌ Could not find admin user");
        return;
    }
    
    var userId = adminUser.Id;
    
    // Create Categories
    var categories = new[]
    {
        new Category { Name = "Fiction", Description = "Fiction books" },
        new Category { Name = "Science", Description = "Science books" },
        new Category { Name = "Programming", Description = "Programming books" },
        new Category { Name = "Movies", Description = "Movie collection" },
        new Category { Name = "History", Description = "Historical books" }
    };

    foreach (var category in categories)
    {
        try
        {
            await categoryService.CreateAsync(category, userId);
            Console.WriteLine($"✅ Category: {category.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ℹ️ Category {category.Name}: {ex.Message}");
        }
    }

    // Create Genres
    var genres = new[]
    {
        new Genre { Name = "Fantasy", Description = "Fantasy genre" },
        new Genre { Name = "Thriller", Description = "Thriller genre" },
        new Genre { Name = "Educational", Description = "Educational content" },
        new Genre { Name = "Action", Description = "Action movies" },
        new Genre { Name = "Comedy", Description = "Comedy entertainment" },
        new Genre { Name = "Biography", Description = "Biographical content" }
    };

    foreach (var genre in genres)
    {
        try
        {
            await genreService.CreateAsync(genre, userId);
            Console.WriteLine($"✅ Genre: {genre.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ℹ️ Genre {genre.Name}: {ex.Message}");
        }
    }

    // Create Media Items
    var mediaItems = new[]
    {
        new MediaItem
        {
            Title = "The Hobbit",
            Author = "J.R.R. Tolkien",
            ISBN = "978-0547928227",
            Type = MediaType.Book,
            Category = "Fiction",
            Genre = "Fantasy",
            Quantity = 5,
            PublicationDate = new DateTime(1937, 9, 21),
            Description = "A classic fantasy novel about Bilbo Baggins' adventure"
        },
        new MediaItem
        {
            Title = "The Lord of the Rings: The Fellowship of the Ring",
            Author = "J.R.R. Tolkien",
            ISBN = "978-0547928210",
            Type = MediaType.Book,
            Category = "Fiction",
            Genre = "Fantasy",
            Quantity = 3,
            PublicationDate = new DateTime(1954, 7, 29),
            Description = "First book in the epic fantasy trilogy"
        },
        new MediaItem
        {
            Title = "Inception",
            Author = "Christopher Nolan",
            Type = MediaType.DVD,
            Category = "Movies",
            Genre = "Thriller",
            Quantity = 3,
            PublicationDate = new DateTime(2010, 7, 16),
            Description = "Mind-bending thriller movie about dreams within dreams"
        },
        new MediaItem
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            ISBN = "978-0132350884",
            Type = MediaType.Book,
            Category = "Programming",
            Genre = "Educational",
            Quantity = 2,
            PublicationDate = new DateTime(2008, 8, 1),
            Description = "A guide to writing clean, maintainable code"
        },
        new MediaItem
        {
            Title = "Design Patterns",
            Author = "Gang of Four",
            ISBN = "978-0201633610",
            Type = MediaType.Book,
            Category = "Programming",
            Genre = "Educational",
            Quantity = 1,
            PublicationDate = new DateTime(1994, 10, 31),
            Description = "Elements of Reusable Object-Oriented Software"
        },
        new MediaItem
        {
            Title = "The Matrix",
            Author = "The Wachowskis",
            Type = MediaType.DVD,
            Category = "Movies",
            Genre = "Action",
            Quantity = 2,
            PublicationDate = new DateTime(1999, 3, 31),
            Description = "Groundbreaking sci-fi action movie"
        },
        new MediaItem
        {
            Title = "Steve Jobs",
            Author = "Walter Isaacson",
            ISBN = "978-1451648539",
            Type = MediaType.Book,
            Category = "History",
            Genre = "Biography",
            Quantity = 2,
            PublicationDate = new DateTime(2011, 10, 24),
            Description = "Biography of Apple's co-founder"
        },
        new MediaItem
        {
            Title = "The Pragmatic Programmer",
            Author = "Andrew Hunt, David Thomas",
            ISBN = "978-0201616224",
            Type = MediaType.Book,
            Category = "Programming",
            Genre = "Educational",
            Quantity = 3,
            PublicationDate = new DateTime(1999, 10, 20),
            Description = "Your journey to mastery"
        },
        new MediaItem
        {
            Title = "Forrest Gump",
            Author = "Robert Zemeckis",
            Type = MediaType.DVD,
            Category = "Movies",
            Genre = "Comedy",
            Quantity = 1,
            PublicationDate = new DateTime(1994, 7, 6),
            Description = "Heartwarming story of an extraordinary man"
        },
        new MediaItem
        {
            Title = "The Art of Computer Programming Vol. 1",
            Author = "Donald E. Knuth",
            ISBN = "978-0201896831",
            Type = MediaType.Book,
            Category = "Programming",
            Genre = "Educational",
            Quantity = 1,
            PublicationDate = new DateTime(1997, 7, 17),
            Description = "Fundamental algorithms and data structures"
        }
    };

    foreach (var item in mediaItems)
    {
        try
        {
            var result = await mediaItemService.CreateAsync(item, userId);
            Console.WriteLine($"✅ Media: {item.Title} (ID: {result.Id})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ℹ️ Media {item.Title}: {ex.Message}");
        }
    }

    Console.WriteLine($"\n🎉 Sample data creation completed!");
    Console.WriteLine($"📊 Admin user: {adminEmail} / Admin123!");
    Console.WriteLine($"📊 Created categories, genres, and {mediaItems.Length} media items");
    Console.WriteLine($"📊 Database file: library.db");
}