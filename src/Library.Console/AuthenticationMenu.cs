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
        private User? _currentUser;
        private volatile bool _isRunning = true;

        public async Task Shutdown()
        {
            _isRunning = false;
            _currentUser = null;
            Program.SetKeepRunning(false); // Signal the program to stop
            await Task.CompletedTask;
        }

        public AuthenticationMenu(IAuthenticationService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        public async Task ShowMainMenuAsync()
        {
            while (_isRunning)
            {
                if (_currentUser == null)
                {
                    System.Console.Clear();
                    System.Console.WriteLine("=== Library Management System ===");
                    
                    var hasAdmin = await _authService.HasAdministratorAsync();
                    if (!hasAdmin)
                    {
                        System.Console.WriteLine("WARNING: No administrator account found in the system!");
                        System.Console.WriteLine("The first user to register will be made an administrator.");
                    }
                    
                    System.Console.WriteLine("1. Login");
                    System.Console.WriteLine("2. Register");
                    System.Console.WriteLine("3. Exit");
                    System.Console.Write("\nSelect an option: ");

                    switch (System.Console.ReadLine())
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
                            System.Console.WriteLine("Invalid option. Press any key to continue...");
                            System.Console.ReadKey();
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
            System.Console.Clear();
            System.Console.WriteLine("=== Login ===");
            
            System.Console.Write("Email: ");
            var email = System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(email))
            {
                System.Console.WriteLine("\nEmail is required. Press any key to continue...");
                System.Console.ReadKey();
                return;
            }

            System.Console.Write("Password: ");
            var password = ReadPassword();

            try
            {
                _currentUser = await _authService.LoginAsync(email, password);
                System.Console.WriteLine($"\nWelcome back, {_currentUser.Name}!");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"\nError: {ex.Message}");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
        }

        private async Task RegisterAsync()
        {
            System.Console.Clear();
            System.Console.WriteLine("=== Register ===");
            
            System.Console.Write("Name: ");
            var name = System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                System.Console.WriteLine("\nName is required. Press any key to continue...");
                System.Console.ReadKey();
                return;
            }
            
            System.Console.Write("Email: ");
            var email = System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(email))
            {
                System.Console.WriteLine("\nEmail is required. Press any key to continue...");
                System.Console.ReadKey();
                return;
            }
            
            System.Console.Write("Password: ");
            var password = ReadPassword();
            
            System.Console.Write("\nConfirm Password: ");
            var confirmPassword = ReadPassword();

            if (password != confirmPassword)
            {
                System.Console.WriteLine("\nPasswords do not match. Press any key to continue...");
                System.Console.ReadKey();
                return;
            }

            try
            {
                User user;
                if (!await _authService.HasAdministratorAsync())
                {
                    // Se non c'è un amministratore, registra il primo utente come admin
                    user = await _authService.RegisterAdminAsync(name, email, password);
                    System.Console.WriteLine($"\nRegistration successful! Welcome {user.Name}!");
                    System.Console.WriteLine("You have been registered as the system administrator.");
                }
                else
                {
                    user = await _authService.RegisterUserAsync(name, email, password);
                    System.Console.WriteLine($"\nRegistration successful! Welcome {user.Name}!");
                }
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"\nError: {ex.Message}");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
        }

        private async Task ShowLoggedInMenuAsync()
        {
            if (_currentUser == null)
            {
                throw new InvalidOperationException("User must be logged in to access this menu");
            }

            System.Console.Clear();
            System.Console.WriteLine($"=== Welcome {_currentUser.Name} ({_currentUser.Role}) ===");
            System.Console.WriteLine("1. Change Password");
            
            if (_currentUser.Role == UserRole.Administrator)
            {
                System.Console.WriteLine("2. List All Users");
                System.Console.WriteLine("3. Register New Admin");
                System.Console.WriteLine("4. Promote User to Admin");
                System.Console.WriteLine("5. Logout");
                System.Console.Write("\nSelect an option: ");

                switch (System.Console.ReadLine())
                {
                    case "1":
                        await ChangePasswordAsync();
                        break;
                    case "2":
                        await ListAllUsersAsync();
                        break;
                    case "3":
                        await RegisterAdminAsync();
                        break;
                    case "4":
                        await PromoteToAdminAsync();
                        break;
                    case "5":
                        _currentUser = null;
                        System.Console.WriteLine("\nLogged out successfully. Press any key to continue...");
                        System.Console.ReadKey();
                        break;
                    default:
                        System.Console.WriteLine("Invalid option. Press any key to continue...");
                        System.Console.ReadKey();
                        break;
                }
            }
            else
            {
                System.Console.WriteLine("2. Logout");
                System.Console.Write("\nSelect an option: ");

                switch (System.Console.ReadLine())
                {
                    case "1":
                        await ChangePasswordAsync();
                        break;
                    case "2":
                        _currentUser = null;
                        System.Console.WriteLine("\nLogged out successfully. Press any key to continue...");
                        System.Console.ReadKey();
                        break;
                    default:
                        System.Console.WriteLine("Invalid option. Press any key to continue...");
                        System.Console.ReadKey();
                        break;
                }
            }
        }

        private async Task ChangePasswordAsync()
        {
            System.Console.Clear();
            System.Console.WriteLine("=== Change Password ===");
            
            System.Console.Write("Current Password: ");
            var currentPassword = ReadPassword();
            
            System.Console.Write("\nNew Password: ");
            var newPassword = ReadPassword();
            
            System.Console.Write("\nConfirm New Password: ");
            var confirmPassword = ReadPassword();

            if (newPassword != confirmPassword)
            {
                System.Console.WriteLine("\nNew passwords do not match. Press any key to continue...");
                System.Console.ReadKey();
                return;
            }

            try
            {
                if (_currentUser == null)
                {
                    throw new InvalidOperationException("User must be logged in to change password");
                }

                await _authService.ChangePasswordAsync(_currentUser.Id, currentPassword, newPassword);
                System.Console.WriteLine("\nPassword changed successfully! Press any key to continue...");
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"\nError: {ex.Message}");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
        }

        private async Task RegisterAdminAsync()
        {
            if (_currentUser?.Role != UserRole.Administrator)
            {
                throw new InvalidOperationException("Only administrators can register new admins");
            }

            System.Console.Clear();
            System.Console.WriteLine("=== Register New Admin ===");
            
            System.Console.Write("Name: ");
            var name = System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                System.Console.WriteLine("\nName is required. Press any key to continue...");
                System.Console.ReadKey();
                return;
            }
            
            System.Console.Write("Email: ");
            var email = System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(email))
            {
                System.Console.WriteLine("\nEmail is required. Press any key to continue...");
                System.Console.ReadKey();
                return;
            }
            
            System.Console.Write("Password: ");
            var password = ReadPassword();
            
            System.Console.Write("\nConfirm Password: ");
            var confirmPassword = ReadPassword();

            if (password != confirmPassword)
            {
                System.Console.WriteLine("\nPasswords do not match. Press any key to continue...");
                System.Console.ReadKey();
                return;
            }

            try
            {
                var admin = await _authService.RegisterAdminAsync(name, email, password);
                System.Console.WriteLine($"\nAdmin registration successful! {admin.Name} is now an administrator.");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"\nError: {ex.Message}");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
        }

        private async Task PromoteToAdminAsync()
        {
            if (_currentUser?.Role != UserRole.Administrator)
            {
                throw new InvalidOperationException("Only administrators can promote users to admin");
            }

            System.Console.Clear();
            System.Console.WriteLine("=== Promote User to Admin ===");
            
            System.Console.Write("User ID: ");
            if (!int.TryParse(System.Console.ReadLine(), out int userId))
            {
                System.Console.WriteLine("\nInvalid user ID. Press any key to continue...");
                System.Console.ReadKey();
                return;
            }

            try
            {
                await _authService.SetUserRoleAsync(userId, UserRole.Administrator);
                System.Console.WriteLine($"\nUser with ID {userId} has been promoted to administrator.");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"\nError: {ex.Message}");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
        }

        private async Task ListAllUsersAsync()
        {
            if (_currentUser?.Role != UserRole.Administrator)
            {
                throw new InvalidOperationException("Only administrators can list users");
            }

            System.Console.Clear();
            System.Console.WriteLine("=== Users List ===\n");
            
            var users = await _userRepository.GetAllAsync();
            foreach (var user in users)
            {
                System.Console.WriteLine($"ID: {user.Id}");
                System.Console.WriteLine($"Name: {user.Name}");
                System.Console.WriteLine($"Email: {user.Email}");
                System.Console.WriteLine($"Role: {user.Role}");
                System.Console.WriteLine($"Status: {(user.IsEnabled ? "Enabled" : "Disabled")}");
                System.Console.WriteLine($"Created: {user.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                System.Console.WriteLine(new string('-', 50));
            }

            System.Console.WriteLine("\nPress any key to continue...");
            System.Console.ReadKey();
        }

        private string ReadPassword()
        {
            var password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = System.Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    System.Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    System.Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return password;
        }
    }
}
