using System;
using System.Threading.Tasks;
using Library.Core.Models;
using Library.Core.Interfaces;

namespace Library.Console
{
    public class AuthenticationMenu
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IMediaItemService _mediaItemService;
        private readonly ICategoryService _categoryService;
        private readonly IGenreService _genreService;
        private readonly ConsoleManager _console;
        private User? _currentUser;
        private volatile bool _isRunning = true;

        public async Task Shutdown()
        {
            try
            {
                _isRunning = false;
                _currentUser = null;
                Program.SetKeepRunning(false); // Signal the program to stop
                await Task.CompletedTask;
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException)
            {
                // Ignora errori di I/O durante lo shutdown
            }
        }

        public AuthenticationMenu(
            IAuthenticationService authService, 
            IUserRepository userRepository,
            IMediaItemService mediaItemService,
            ICategoryService categoryService,
            IGenreService genreService,
            ConsoleManager console)
        {
            _authService = authService;
            _userRepository = userRepository;
            _mediaItemService = mediaItemService;
            _categoryService = categoryService;
            _genreService = genreService;
            _console = console;
        }

        public async Task ShowMainMenuAsync()
        {
            while (_isRunning)
            {
                if (_currentUser == null)
                {
                    _console.Clear();
                    _console.WriteLine("=== Library Management System ===");
                    
                    var hasAdmin = await _authService.HasAdministratorAsync();
                    if (!hasAdmin)
                    {
                        _console.WriteLine("WARNING: No administrator account found in the system!");
                        _console.WriteLine("The first user to register will be made an administrator.");
                    }
                    
                    _console.WriteLine("1. Login");
                    _console.WriteLine("2. Register");
                    _console.WriteLine("3. Exit");
                    _console.Write("\nSelect an option: ");

                    switch (_console.ReadLine())
                    {
                        case "1":
                            await LoginAsync();
                            break;
                        case "2":
                            await RegisterAsync();
                            break;
                        case "3":
                            await Shutdown();
                            return;
                        default:
                            _console.WriteLine("Invalid option. Press any key to continue...");
                            _console.ReadKey();
                            break;
                    }
                }
                else
                {
                    await ShowLoggedInMenuAsync();
                }
            }
        }

        private async Task LoginAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Login ===");
            
            _console.Write("Email: ");
            var email = _console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(email))
            {
                _console.WriteLine("\nEmail is required. Press any key to continue...");
                _console.ReadKey();
                return;
            }

            _console.Write("Password: ");
            var password = ReadPassword();

            try
            {
                _currentUser = await _authService.LoginAsync(email, password);
                _console.WriteLine($"\nWelcome back, {_currentUser.Name}!");
                _console.WriteLine("Press any key to continue...");
                _console.ReadKey();
            }
            catch (Exception ex)
            {
                _console.WriteLine($"\nError: {ex.Message}");
                _console.WriteLine("Press any key to continue...");
                _console.ReadKey();
            }
        }

        private async Task RegisterAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Register ===");
            
            _console.Write("Name: ");
            var name = _console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                _console.WriteLine("\nName is required. Press any key to continue...");
                _console.ReadKey();
                return;
            }
            
            _console.Write("Email: ");
            var email = _console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(email))
            {
                _console.WriteLine("\nEmail is required. Press any key to continue...");
                _console.ReadKey();
                return;
            }
            
            _console.Write("Password: ");
            var password = ReadPassword();
            
            _console.Write("\nConfirm Password: ");
            var confirmPassword = ReadPassword();

            if (password != confirmPassword)
            {
                _console.WriteLine("\nPasswords do not match. Press any key to continue...");
                _console.ReadKey();
                return;
            }

            try
            {
                User user;
                if (!await _authService.HasAdministratorAsync())
                {
                    // Se non c'è un amministratore, registra il primo utente come admin
                    user = await _authService.RegisterAdminAsync(name, email, password);
                    _console.WriteLine($"\nRegistration successful! Welcome {user.Name}!");
                    _console.WriteLine("You have been registered as the system administrator.");
                }
                else
                {
                    user = await _authService.RegisterUserAsync(name, email, password);
                    _console.WriteLine($"\nRegistration successful! Welcome {user.Name}!");
                }
                _console.WriteLine("Press any key to continue...");
                _console.ReadKey();
            }
            catch (Exception ex)
            {
                _console.WriteLine($"\nError: {ex.Message}");
                _console.WriteLine("Press any key to continue...");
                _console.ReadKey();
            }
        }

        private async Task ShowLoggedInMenuAsync()
        {
            if (_currentUser == null)
            {
                throw new InvalidOperationException("User must be logged in to access this menu");
            }

            _console.Clear();
            _console.WriteLine($"=== Welcome {_currentUser.Name} ({_currentUser.Role}) ===");
            _console.WriteLine("1. Change Password");
            
            if (_currentUser.Role == UserRole.Administrator)
            {
                _console.WriteLine("2. List All Users");
                _console.WriteLine("3. 👑 Admin Panel");
                _console.WriteLine("4. 🔬 Quick Test Catalog (DEBUG)");
                _console.WriteLine("5. Logout");
                _console.Write("\nSelect an option: ");

                switch (_console.ReadLine())
                {
                    case "1":
                        await ChangePasswordAsync();
                        break;
                    case "2":
                        await ListAllUsersAsync();
                        break;
                    case "3":
                        await ShowAdminPanelAsync();
                        break;
                    case "4":
                        await QuickTestCatalogAsync();
                        break;
                    case "5":
                        _currentUser = null;
                        _console.WriteLine("\nLogged out successfully. Press any key to continue...");
                        _console.ReadKey();
                        break;
                    default:
                        _console.WriteLine("Invalid option. Press any key to continue...");
                        _console.ReadKey();
                        break;
                }
            }
            else
            {
                _console.WriteLine("2. 🔬 Quick Test Catalog (DEBUG)");
                _console.WriteLine("3. Logout");
                _console.Write("\nSelect an option: ");

                switch (_console.ReadLine())
                {
                    case "1":
                        await ChangePasswordAsync();
                        break;
                    case "2":
                        await QuickTestCatalogAsync();
                        break;
                    case "3":
                        _currentUser = null;
                        _console.WriteLine("\nLogged out successfully. Press any key to continue...");
                        _console.ReadKey();
                        break;
                    default:
                        _console.WriteLine("Invalid option. Press any key to continue...");
                        _console.ReadKey();
                        break;
                }
            }
        }

        private async Task ChangePasswordAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Change Password ===");
            
            _console.Write("Current Password: ");
            var currentPassword = ReadPassword();
            
            _console.Write("\nNew Password: ");
            var newPassword = ReadPassword();
            
            _console.Write("\nConfirm New Password: ");
            var confirmPassword = ReadPassword();

            if (newPassword != confirmPassword)
            {
                _console.WriteLine("\nNew passwords do not match. Press any key to continue...");
                _console.ReadKey();
                return;
            }

            try
            {
                if (_currentUser == null)
                {
                    throw new InvalidOperationException("User must be logged in to change password");
                }

                await _authService.ChangePasswordAsync(_currentUser.Id, currentPassword, newPassword);
                _console.WriteLine("\nPassword changed successfully! Press any key to continue...");
                _console.ReadKey();
            }
            catch (Exception ex)
            {
                _console.WriteLine($"\nError: {ex.Message}");
                _console.WriteLine("Press any key to continue...");
                _console.ReadKey();
            }
        }

        private async Task RegisterAdminAsync()
        {
            if (_currentUser?.Role != UserRole.Administrator)
            {
                throw new InvalidOperationException("Only administrators can register new admins");
            }

            _console.Clear();
            _console.WriteLine("=== Register New Admin ===");
            
            _console.Write("Name: ");
            var name = _console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                _console.WriteLine("\nName is required. Press any key to continue...");
                _console.ReadKey();
                return;
            }
            
            _console.Write("Email: ");
            var email = _console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(email))
            {
                _console.WriteLine("\nEmail is required. Press any key to continue...");
                _console.ReadKey();
                return;
            }
            
            _console.Write("Password: ");
            var password = ReadPassword();
            
            _console.Write("\nConfirm Password: ");
            var confirmPassword = ReadPassword();

            if (password != confirmPassword)
            {
                _console.WriteLine("\nPasswords do not match. Press any key to continue...");
                _console.ReadKey();
                return;
            }

            try
            {
                var admin = await _authService.RegisterAdminAsync(name, email, password);
                _console.WriteLine($"\nAdmin registration successful! {admin.Name} is now an administrator.");
                _console.WriteLine("Press any key to continue...");
                _console.ReadKey();
            }
            catch (Exception ex)
            {
                _console.WriteLine($"\nError: {ex.Message}");
                _console.WriteLine("Press any key to continue...");
                _console.ReadKey();
            }
        }

        private async Task PromoteToAdminAsync()
        {
            if (_currentUser?.Role != UserRole.Administrator)
            {
                throw new InvalidOperationException("Only administrators can promote users to admin");
            }

            _console.Clear();
            _console.WriteLine("=== Promote User to Admin ===");
            
            _console.Write("User ID: ");
            if (!int.TryParse(_console.ReadLine(), out int userId))
            {
                _console.WriteLine("\nInvalid user ID. Press any key to continue...");
                _console.ReadKey();
                return;
            }

            try
            {
                await _authService.SetUserRoleAsync(userId, UserRole.Administrator);
                _console.WriteLine($"\nUser with ID {userId} has been promoted to administrator.");
                _console.WriteLine("Press any key to continue...");
                _console.ReadKey();
            }
            catch (Exception ex)
            {
                _console.WriteLine($"\nError: {ex.Message}");
                _console.WriteLine("Press any key to continue...");
                _console.ReadKey();
            }
        }

        private async Task ListAllUsersAsync()
        {
            if (_currentUser?.Role != UserRole.Administrator)
            {
                throw new InvalidOperationException("Only administrators can list users");
            }

            _console.Clear();
            _console.WriteLine("=== Users List ===\n");
            
            var users = await _userRepository.GetAllAsync();
            foreach (var user in users)
            {
                _console.WriteLine($"ID: {user.Id}");
                _console.WriteLine($"Name: {user.Name}");
                _console.WriteLine($"Email: {user.Email}");
                _console.WriteLine($"Role: {user.Role}");
                _console.WriteLine($"Status: {(user.IsEnabled ? "Enabled" : "Disabled")}");
                _console.WriteLine($"Created: {user.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                _console.WriteLine(new string('-', 50));
            }

            _console.WriteLine("\nPress any key to continue...");
            _console.ReadKey();
        }

        private async Task ShowAdminPanelAsync()
        {
            if (_currentUser?.Role != UserRole.Administrator)
            {
                throw new InvalidOperationException("Only administrators can access the admin panel");
            }

            var adminMenu = new AdminMenu(
                _authService,
                _userRepository,
                _mediaItemService,
                _categoryService,
                _genreService,
                _console,
                _currentUser);

            await adminMenu.ShowMenuAsync();
        }

        private async Task QuickTestCatalogAsync()
        {
            if (_currentUser == null)
            {
                throw new InvalidOperationException("User must be logged in to access catalog test");
            }

            var quickTest = new CatalogQuickTest(_mediaItemService, _categoryService, _genreService, _console);
            await quickTest.RunAsync(_currentUser.Id);
        }

        private string ReadPassword()
        {
            var password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = _console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    _console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    _console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return password;
        }
    }
}
