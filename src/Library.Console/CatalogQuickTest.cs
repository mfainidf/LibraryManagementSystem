using System;
using System.Threading.Tasks;
using Library.Core.Models;
using Library.Core.Interfaces;

namespace Library.Console
{
    public class CatalogQuickTest
    {
        private readonly IMediaItemService _mediaItemService;
        private readonly ICategoryService _categoryService;
        private readonly IGenreService _genreService;
        private readonly ConsoleManager _console;

        public CatalogQuickTest(
            IMediaItemService mediaItemService,
            ICategoryService categoryService,
            IGenreService genreService,
            ConsoleManager console)
        {
            _mediaItemService = mediaItemService;
            _categoryService = categoryService;
            _genreService = genreService;
            _console = console;
        }

        public async Task RunAsync(int userId)
        {
            while (true)
            {
                _console.Clear();
                _console.WriteLine("=== 🔬 CATALOG QUICK TEST ===");
                _console.WriteLine("1. Create Sample Data");
                _console.WriteLine("2. List All Media Items");
                _console.WriteLine("3. Search Test");
                _console.WriteLine("4. Add Random Book");
                _console.WriteLine("5. Statistics");
                _console.WriteLine("6. Test Validations");
                _console.WriteLine("7. Back to Main");
                _console.Write("\nOption: ");

                switch (_console.ReadLine())
                {
                    case "1":
                        await CreateSampleDataAsync(userId);
                        break;
                    case "2":
                        await ListAllMediaItemsAsync();
                        break;
                    case "3":
                        await SearchTestAsync();
                        break;
                    case "4":
                        await AddRandomBookAsync(userId);
                        break;
                    case "5":
                        await ShowStatisticsAsync();
                        break;
                    case "6":
                        await TestValidationsAsync(userId);
                        break;
                    case "7":
                        return;
                    default:
                        _console.WriteLine("Invalid option");
                        break;
                }

                _console.WriteLine("\nPress any key...");
                _console.ReadKey();
            }
        }

        private async Task CreateSampleDataAsync(int userId)
        {
            _console.WriteLine("🔧 Creating sample data...");

            try
            {
                // Create Categories
                var categories = new[]
                {
                    new Category { Name = "Fiction", Description = "Fiction books" },
                    new Category { Name = "Science", Description = "Science books" },
                    new Category { Name = "Programming", Description = "Programming books" }
                };

                foreach (var category in categories)
                {
                    try
                    {
                        await _categoryService.CreateAsync(category, userId);
                        _console.WriteLine($"✅ Category: {category.Name}");
                    }
                    catch (Exception ex)
                    {
                        _console.WriteLine($"❌ Category {category.Name}: {ex.Message}");
                    }
                }

                // Create Genres
                var genres = new[]
                {
                    new Genre { Name = "Fantasy", Description = "Fantasy genre" },
                    new Genre { Name = "Thriller", Description = "Thriller genre" },
                    new Genre { Name = "Educational", Description = "Educational content" }
                };

                foreach (var genre in genres)
                {
                    try
                    {
                        await _genreService.CreateAsync(genre, userId);
                        _console.WriteLine($"✅ Genre: {genre.Name}");
                    }
                    catch (Exception ex)
                    {
                        _console.WriteLine($"❌ Genre {genre.Name}: {ex.Message}");
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
                        Description = "A classic fantasy novel"
                    },
                    new MediaItem
                    {
                        Title = "Inception",
                        Author = "Christopher Nolan",
                        Type = MediaType.DVD,
                        Category = "Fiction",
                        Genre = "Thriller",
                        Quantity = 3,
                        Description = "Mind-bending thriller movie"
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
                        Description = "Programming best practices"
                    }
                };

                foreach (var item in mediaItems)
                {
                    try
                    {
                        var result = await _mediaItemService.CreateAsync(item, userId);
                        _console.WriteLine($"✅ Media: {item.Title} (ID: {result.Id})");
                    }
                    catch (Exception ex)
                    {
                        _console.WriteLine($"❌ Media {item.Title}: {ex.Message}");
                    }
                }

                _console.WriteLine("\n🎉 Sample data creation completed!");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"💥 Error: {ex.Message}");
            }
        }

        private async Task ListAllMediaItemsAsync()
        {
            _console.WriteLine("📚 All Media Items:");

            try
            {
                var items = await _mediaItemService.GetAllAsync();
                
                if (!items.Any())
                {
                    _console.WriteLine("❌ No media items found. Create some first!");
                    return;
                }

                foreach (var item in items)
                {
                    _console.WriteLine($"📖 [{item.Id}] {item.Title}");
                    _console.WriteLine($"   Author: {item.Author ?? "N/A"}");
                    _console.WriteLine($"   Type: {item.Type.GetDisplayName()}");
                    _console.WriteLine($"   ISBN: {item.ISBN ?? "N/A"}");
                    _console.WriteLine($"   Qty: {item.AvailableQuantity}/{item.Quantity}");
                    _console.WriteLine($"   Category: {item.Category ?? "N/A"}");
                    _console.WriteLine($"   Genre: {item.Genre ?? "N/A"}");
                    _console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                _console.WriteLine($"💥 Error: {ex.Message}");
            }
        }

        private async Task SearchTestAsync()
        {
            _console.Write("🔍 Search term: ");
            var searchTerm = _console.ReadLine();

            if (string.IsNullOrEmpty(searchTerm))
            {
                _console.WriteLine("❌ Empty search term");
                return;
            }

            try
            {
                var results = await _mediaItemService.SearchAsync(searchTerm);
                
                _console.WriteLine($"\n📊 Found {results.Count()} items:");
                
                foreach (var item in results)
                {
                    _console.WriteLine($"📖 {item.Title} by {item.Author ?? "Unknown"}");
                }
            }
            catch (Exception ex)
            {
                _console.WriteLine($"💥 Error: {ex.Message}");
            }
        }

        private async Task AddRandomBookAsync(int userId)
        {
            var random = new Random();
            var randomId = random.Next(1000, 9999);

            var book = new MediaItem
            {
                Title = $"Test Book {randomId}",
                Author = $"Test Author {randomId}",
                ISBN = $"978-{randomId:0000}-{random.Next(100, 999)}-{random.Next(10, 99)}",
                Type = MediaType.Book,
                Category = "Fiction",
                Genre = "Fantasy",
                Quantity = random.Next(1, 10),
                Description = $"Auto-generated test book {randomId}"
            };

            try
            {
                var result = await _mediaItemService.CreateAsync(book, userId);
                _console.WriteLine($"✅ Created: {book.Title} (ID: {result.Id})");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        private async Task ShowStatisticsAsync()
        {
            try
            {
                _console.WriteLine("📊 STATISTICS:");
                
                var totalCount = await _mediaItemService.GetTotalCountAsync();
                var availableCount = await _mediaItemService.GetAvailableCountAsync();
                var countByType = await _mediaItemService.GetCountByTypeAsync();

                _console.WriteLine($"📚 Total Items: {totalCount}");
                _console.WriteLine($"✅ Available: {availableCount}");
                _console.WriteLine($"🚫 Unavailable: {totalCount - availableCount}");
                
                _console.WriteLine("\n📈 By Type:");
                foreach (var kvp in countByType)
                {
                    if (kvp.Value > 0)
                        _console.WriteLine($"   {kvp.Key.GetDisplayName()}: {kvp.Value}");
                }
            }
            catch (Exception ex)
            {
                _console.WriteLine($"💥 Error: {ex.Message}");
            }
        }

        private async Task TestValidationsAsync(int userId)
        {
            _console.WriteLine("🧪 Testing validations...");

            // Test 1: Duplicate ISBN
            try
            {
                var book1 = new MediaItem
                {
                    Title = "Test Book 1",
                    Author = "Test Author",
                    ISBN = "978-DUPLICATE-TEST",
                    Type = MediaType.Book,
                    Quantity = 1
                };

                await _mediaItemService.CreateAsync(book1, userId);
                _console.WriteLine("✅ First book created");

                var book2 = new MediaItem
                {
                    Title = "Test Book 2", 
                    Author = "Different Author",
                    ISBN = "978-DUPLICATE-TEST", // Same ISBN!
                    Type = MediaType.Book,
                    Quantity = 1
                };

                await _mediaItemService.CreateAsync(book2, userId);
                _console.WriteLine("❌ VALIDATION FAILED: Duplicate ISBN should be rejected!");
            }
            catch (InvalidOperationException ex)
            {
                _console.WriteLine($"✅ VALIDATION OK: {ex.Message}");
            }

            // Test 2: Empty title
            try
            {
                var badBook = new MediaItem
                {
                    Title = "", // Empty!
                    Type = MediaType.Book,
                    Quantity = 1
                };

                await _mediaItemService.CreateAsync(badBook, userId);
                _console.WriteLine("❌ VALIDATION FAILED: Empty title should be rejected!");
            }
            catch (ArgumentException ex)
            {
                _console.WriteLine($"✅ VALIDATION OK: {ex.Message}");
            }

            _console.WriteLine("\n🎉 Validation tests completed!");
        }
    }
}
