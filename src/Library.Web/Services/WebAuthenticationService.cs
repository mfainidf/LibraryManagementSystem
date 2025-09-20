using System.Text.Json;
using Library.Core.Models;
using Library.Core.Interfaces;

namespace Library.Web.Services;

public class WebAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Library.Core.Interfaces.IAuthenticationService _authService;
    private readonly ILogger<WebAuthenticationService> _logger;
    
    // Cache semplice per l'utente corrente
    private static User? _currentUser = null;
    private static bool _isAuthenticated = false;

    public WebAuthenticationService(IHttpContextAccessor httpContextAccessor, Library.Core.Interfaces.IAuthenticationService authService, ILogger<WebAuthenticationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _authService = authService;
        _logger = logger;
    }

    public void SetTemporaryLoginData(string email, string password)
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                _logger.LogInformation("WebAuth: Setting temporary login data for {Email}", email);
                var tempData = JsonSerializer.Serialize(new { Email = email, Password = password });
                context.Session.SetString("TempLogin", tempData);
                _logger.LogInformation("WebAuth: Temporary login data set for {Email}", email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebAuth: Failed to set temporary login data for {Email}", email);
        }
    }

    public async Task<bool> CompleteLoginFromTempDataAsync()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var tempDataJson = context.Session.GetString("TempLogin");
                if (!string.IsNullOrEmpty(tempDataJson))
                {
                    var tempData = JsonSerializer.Deserialize<dynamic>(tempDataJson);
                    if (tempData != null)
                    {
                        var tempElement = (JsonElement)tempData;
                        var email = tempElement.GetProperty("Email").GetString();
                        var password = tempElement.GetProperty("Password").GetString();
                        
                        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                        {
                            _logger.LogInformation("WebAuth: Completing login from temp data for {Email}", email);
                            var (success, user) = await LoginAsync(email, password);
                            
                            // Pulisci i dati temporanei
                            context.Session.Remove("TempLogin");
                            
                            return success;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebAuth: Exception completing login from temp data");
        }
        return false;
    }

    public async Task<User?> ValidateCredentialsAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("WebAuth: Validating credentials for {Email}", email);
            var user = await _authService.LoginAsync(email, password);
            
            if (user != null)
            {
                _logger.LogInformation("WebAuth: Credentials valid for {Email}", email);
            }
            else
            {
                _logger.LogWarning("WebAuth: Invalid credentials for {Email}", email);
            }
            
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebAuth: Exception during credential validation for {Email}", email);
            return null;
        }
    }

    public async Task<(bool Success, User? User)> LoginAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("WebAuth: Starting direct login for {Email}", email);
            var user = await _authService.LoginAsync(email, password);
            
            if (user != null)
            {
                _logger.LogInformation("WebAuth: Backend login successful for {Email}", email);
                
                // Invece di usare session durante il rendering, usiamo un approccio diverso
                // Salviamo l'utente in una proprietà statica temporanea
                _currentUser = user;
                _isAuthenticated = true;
                
                _logger.LogInformation("WebAuth: User cached successfully for {Email}", email);
            }
            else
            {
                _logger.LogWarning("WebAuth: Backend login failed for {Email}", email);
                _currentUser = null;
                _isAuthenticated = false;
            }
            
            return (user != null, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebAuth: Exception during login for {Email}", email);
            _currentUser = null;
            _isAuthenticated = false;
            return (false, null);
        }
    }

    public Task LogoutAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            context.Session.Clear();
        }
        
        // Pulisci anche la cache statica
        _currentUser = null;
        _isAuthenticated = false;
        
        return Task.CompletedTask;
    }

    public User? GetCurrentUser()
    {
        return _currentUser;
    }

    public bool IsAuthenticated()
    {
        return _isAuthenticated && _currentUser != null;
    }

    public bool IsAdmin()
    {
        var user = GetCurrentUser();
        return user?.Role == UserRole.Administrator;
    }
}
