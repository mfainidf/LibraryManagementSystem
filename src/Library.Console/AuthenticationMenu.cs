using System;
using System.Threading.Tasks;
using Library.Core.Models;
using Library.Core.Interfaces;

namespace Library.Console
{
    public class AuthenticationMenu
    {
        private readonly IAuthenticationService _authService;
        private User? _currentUser;

        public AuthenticationMenu(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task ShowMainMenuAsync()
        {
            while (true)
            {
                if (_currentUser == null)
                {
                    System.Console.Clear();
                    System.Console.WriteLine("=== Library Management System ===");
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
                var user = await _authService.RegisterUserAsync(name, email, password);
                System.Console.WriteLine($"\nRegistration successful! Welcome {user.Name}!");
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
            System.Console.WriteLine($"=== Welcome {_currentUser.Name} ===");
            System.Console.WriteLine("1. Change Password");
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
