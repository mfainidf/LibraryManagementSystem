using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Library.Core.Models;
using Library.Core.Interfaces;

namespace Library.Console
{
    public class AdminMenu
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IMediaItemService _mediaItemService;
        private readonly ICategoryService _categoryService;
        private readonly IGenreService _genreService;
        private readonly ConsoleManager _console;
        private readonly User _currentUser;

        public AdminMenu(
            IAuthenticationService authService,
            IUserRepository userRepository,
            IMediaItemService mediaItemService,
            ICategoryService categoryService,
            IGenreService genreService,
            ConsoleManager console,
            User currentUser)
        {
            _authService = authService;
            _userRepository = userRepository;
            _mediaItemService = mediaItemService;
            _categoryService = categoryService;
            _genreService = genreService;
            _console = console;
            _currentUser = currentUser;
        }

        public async Task ShowMenuAsync()
        {
            while (true)
            {
                _console.Clear();
                _console.WriteLine("=== 👑 ADMINISTRATION MENU ===");
                _console.WriteLine("📊 USER MANAGEMENT");
                _console.WriteLine("1. List All Users");
                _console.WriteLine("2. Register New Admin");
                _console.WriteLine("3. Promote User to Admin");
                _console.WriteLine("4. Disable/Enable User");
                _console.WriteLine("5. Reset User Password");
                _console.WriteLine("");
                _console.WriteLine("🗑️ DATABASE MANAGEMENT");
                _console.WriteLine("6. Reset Media Catalog");
                _console.WriteLine("7. Reset Categories & Genres");
                _console.WriteLine("8. Reset Everything (except users)");
                _console.WriteLine("9. Database Statistics");
                _console.WriteLine("");
                _console.WriteLine("🔬 TESTING");
                _console.WriteLine("10. Quick Test Catalog");
                _console.WriteLine("");
                _console.WriteLine("0. Back to Main Menu");
                _console.Write("\nSelect option: ");

                switch (_console.ReadLine())
                {
                    case "1":
                        await ListAllUsersAsync();
                        break;
                    case "2":
                        await RegisterAdminAsync();
                        break;
                    case "3":
                        await PromoteUserAsync();
                        break;
                    case "4":
                        await ToggleUserStatusAsync();
                        break;
                    case "5":
                        await ResetUserPasswordAsync();
                        break;
                    case "6":
                        await ResetMediaCatalogAsync();
                        break;
                    case "7":
                        await ResetLookupsAsync();
                        break;
                    case "8":
                        await ResetAllDataAsync();
                        break;
                    case "9":
                        await ShowDatabaseStatsAsync();
                        break;
                    case "10":
                        await QuickTestCatalogAsync();
                        break;
                    case "0":
                        return;
                    default:
                        _console.WriteLine("❌ Invalid option");
                        break;
                }

                if (_console.ReadLine() != "0")
                {
                    _console.WriteLine("\nPress any key to continue...");
                    System.Console.ReadKey();
                }
            }
        }

        private async Task ListAllUsersAsync()
        {
            _console.WriteLine("👥 ALL USERS:");
            
            try
            {
                var users = await _userRepository.GetAllAsync();
                
                foreach (var user in users)
                {
                    var status = user.IsEnabled ? "✅ Active" : "❌ Disabled";
                    var roleIcon = user.Role switch
                    {
                        UserRole.Administrator => "👑",
                        UserRole.Supervisor => "🔧",
                        _ => "👤"
                    };
                    
                    _console.WriteLine($"{roleIcon} [{user.Id}] {user.Name} ({user.Email})");
                    _console.WriteLine($"   Role: {user.Role} | Status: {status}");
                    _console.WriteLine($"   Created: {user.CreatedAt:yyyy-MM-dd HH:mm}");
                    _console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                _console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        private async Task RegisterAdminAsync()
        {
            _console.WriteLine("👑 REGISTER NEW ADMIN");
            
            _console.Write("Name: ");
            var name = _console.ReadLine();
            if (string.IsNullOrEmpty(name))
            {
                _console.WriteLine("❌ Name is required");
                return;
            }
            
            _console.Write("Email: ");
            var email = _console.ReadLine();
            if (string.IsNullOrEmpty(email))
            {
                _console.WriteLine("❌ Email is required");
                return;
            }
            
            _console.Write("Password: ");
            var password = ReadPassword();
            
            try
            {
                var admin = await _authService.RegisterAdminAsync(name, email, password);
                _console.WriteLine($"✅ Admin created: {admin.Name} ({admin.Email})");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        private async Task PromoteUserAsync()
        {
            _console.WriteLine("⬆️ PROMOTE USER");
            
            _console.Write("User ID: ");
            if (!int.TryParse(_console.ReadLine(), out int userId))
            {
                _console.WriteLine("❌ Invalid user ID");
                return;
            }

            _console.WriteLine("Select new role:");
            _console.WriteLine("1. User");
            _console.WriteLine("2. Supervisor");  
            _console.WriteLine("3. Administrator");
            _console.Write("Choice: ");

            var roleChoice = _console.ReadLine();
            var newRole = roleChoice switch
            {
                "1" => UserRole.User,
                "2" => UserRole.Supervisor,
                "3" => UserRole.Administrator,
                _ => (UserRole?)null
            };

            if (!newRole.HasValue)
            {
                _console.WriteLine("❌ Invalid role choice");
                return;
            }

            try
            {
                await _authService.SetUserRoleAsync(userId, newRole.Value);
                _console.WriteLine($"✅ User {userId} promoted to {newRole.Value}");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        private async Task ToggleUserStatusAsync()
        {
            _console.WriteLine("🔄 TOGGLE USER STATUS");
            
            _console.Write("User ID: ");
            if (!int.TryParse(_console.ReadLine(), out int userId))
            {
                _console.WriteLine("❌ Invalid user ID");
                return;
            }

            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _console.WriteLine("❌ User not found");
                    return;
                }

                user.IsEnabled = !user.IsEnabled;
                await _userRepository.UpdateAsync(user);
                
                var status = user.IsEnabled ? "enabled" : "disabled";
                _console.WriteLine($"✅ User {user.Name} {status}");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        private async Task ResetUserPasswordAsync()
        {
            _console.WriteLine("🔑 RESET USER PASSWORD");
            
            _console.Write("User ID: ");
            if (!int.TryParse(_console.ReadLine(), out int userId))
            {
                _console.WriteLine("❌ Invalid user ID");
                return;
            }

            _console.Write("New Password: ");
            var newPassword = ReadPassword();

            try
            {
                await _authService.ResetPasswordAsync(userId, newPassword);
                _console.WriteLine("✅ Password reset successfully");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        private async Task ResetMediaCatalogAsync()
        {
            _console.WriteLine("🗑️ RESET MEDIA CATALOG");
            _console.WriteLine("⚠️ This will delete ALL media items!");
            _console.Write("Type 'CONFIRM' to proceed: ");
            
            if (_console.ReadLine() != "CONFIRM")
            {
                _console.WriteLine("❌ Operation cancelled");
                return;
            }

            try
            {
                var allItems = await _mediaItemService.GetAllAsync();
                int deletedCount = 0;
                
                foreach (var item in allItems)
                {
                    if (await _mediaItemService.DeleteAsync(item.Id, _currentUser.Id))
                        deletedCount++;
                }

                _console.WriteLine($"✅ Deleted {deletedCount} media items");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        private async Task ResetLookupsAsync()
        {
            _console.WriteLine("🗑️ RESET CATEGORIES & GENRES");
            _console.WriteLine("⚠️ This will delete ALL categories and genres!");
            _console.Write("Type 'CONFIRM' to proceed: ");
            
            if (_console.ReadLine() != "CONFIRM")
            {
                _console.WriteLine("❌ Operation cancelled");
                return;
            }

            try
            {
                var categories = await _categoryService.GetAllAsync();
                var genres = await _genreService.GetAllAsync();
                
                int deletedCategories = 0, deletedGenres = 0;
                
                foreach (var category in categories)
                {
                    if (await _categoryService.DeleteAsync(category.Id))
                        deletedCategories++;
                }
                
                foreach (var genre in genres)
                {
                    if (await _genreService.DeleteAsync(genre.Id))
                        deletedGenres++;
                }

                _console.WriteLine($"✅ Deleted {deletedCategories} categories and {deletedGenres} genres");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        private async Task ResetAllDataAsync()
        {
            _console.WriteLine("💀 RESET ALL DATA (EXCEPT USERS)");
            _console.WriteLine("⚠️ This will delete ALL catalog data!");
            _console.Write("Type 'RESET ALL' to proceed: ");
            
            if (_console.ReadLine() != "RESET ALL")
            {
                _console.WriteLine("❌ Operation cancelled");
                return;
            }

            try
            {
                await ResetMediaCatalogAsync();
                await ResetLookupsAsync();
                _console.WriteLine("🎉 Complete data reset completed");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        private async Task ShowDatabaseStatsAsync()
        {
            _console.WriteLine("📊 DATABASE STATISTICS");
            
            try
            {
                var totalUsers = (await _userRepository.GetAllAsync()).Count();
                var activeUsers = (await _userRepository.GetAllAsync()).Count(u => u.IsEnabled);
                var totalMedia = await _mediaItemService.GetTotalCountAsync();
                var availableMedia = await _mediaItemService.GetAvailableCountAsync();
                var totalCategories = (await _categoryService.GetAllAsync()).Count();
                var totalGenres = (await _genreService.GetAllAsync()).Count();

                _console.WriteLine($"👥 Users: {totalUsers} total, {activeUsers} active");
                _console.WriteLine($"📚 Media Items: {totalMedia} total, {availableMedia} available");
                _console.WriteLine($"🏷️ Categories: {totalCategories}");
                _console.WriteLine($"🎭 Genres: {totalGenres}");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        private async Task QuickTestCatalogAsync()
        {
            var quickTest = new CatalogQuickTest(_mediaItemService, _categoryService, _genreService, _console);
            await quickTest.RunAsync(_currentUser.Id);
        }

        private string ReadPassword()
        {
            var password = "";
            ConsoleKeyInfo key;

            do
            {
                key = System.Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    _console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    _console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            _console.WriteLine(); // Add new line after password input
            return password;
        }
    }
}
