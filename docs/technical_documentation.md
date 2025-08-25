# Specifiche Tecniche - Sistema Gestione Biblioteca

## 📐 Architettura del Sistema

### Architettura N-Tier
```
┌─────────────────┐
│  Presentation   │ → UI Layer (Console/Web)
├─────────────────┤
│  Business Logic │ → Domain Services, Use Cases
├─────────────────┤
│  Data Access    │ → Repositories, Data Context
└─────────────────┘
```

Il sistema è strutturato seguendo i principi di Clean Architecture e utilizza il pattern DI (Dependency Injection) per garantire un basso accoppiamento tra i componenti.

### Design Pattern Principali
1. **Repository Pattern**
   ```csharp
   public interface IRepository<T> where T : class
   {
       Task<T> GetByIdAsync(int id);
       Task<IEnumerable<T>> GetAllAsync();
       Task AddAsync(T entity);
       Task UpdateAsync(T entity);
       Task DeleteAsync(int id);
   }
   ```

2. **Unit of Work**
   ```csharp
   public interface IUnitOfWork
   {
       IBookRepository Books { get; }
       IUserRepository Users { get; }
       ILoanRepository Loans { get; }
       Task<int> SaveChangesAsync();
   }
   ```

3. **CQRS Pattern**
   ```csharp
   public interface ICommand<TResponse>
   public interface IQuery<TResponse>
   public interface ICommandHandler<TCommand, TResponse>
   public interface IQueryHandler<TQuery, TResponse>
   ```

## 🏗 Struttura del Progetto

```
Library.Solution/
├── src/
│   ├── Library.Core/                 # Domain Models, Interfaces
│   ├── Library.Infrastructure/       # Data Access, External Services
│   ├── Library.Application/          # Business Logic, Use Cases
│   ├── Library.API/                  # Web API
│   ├── Library.Web/                  # Web Frontend
│   └── Library.Console/              # Console Application
├── tests/
│   ├── Library.UnitTests/           # Test unitari
│   ├── Library.IntegrationTests/    # Test di integrazione
│   └── Library.E2ETests/
└── tools/                           # Scripts, Tools
```

## 💾 Modello Dati

### Entità Principali
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<Loan> Loans { get; set; }
}

public class MediaItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public MediaType Type { get; set; }
    public string Genre { get; set; }
    public int Year { get; set; }
    public bool IsAvailable { get; set; }
    public virtual ICollection<Preview> Previews { get; set; }
    public virtual ICollection<Loan> Loans { get; set; }
}

public class Loan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MediaItemId { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public LoanStatus Status { get; set; }
    public virtual User User { get; set; }
    public virtual MediaItem MediaItem { get; set; }
}
```

## 🔐 Sicurezza e Autenticazione

### Console Management
#### ConsoleManager
Il `ConsoleManager` è un componente singleton che gestisce in modo thread-safe tutte le operazioni di I/O della console.

Caratteristiche principali:
- Gestione thread-safe delle operazioni di console
- Prevenzione delle eccezioni durante lo shutdown
- Implementazione del pattern IDisposable per la gestione sicura delle risorse

```csharp
public class ConsoleManager : IDisposable
{
    private static readonly object _lock = new object();
    
    public void WriteLine(string message) 
    {
        lock (_lock)
        {
            // Implementazione thread-safe
        }
    }
    
    // Altri metodi thread-safe per operazioni di console
}
```

### AuthenticationMenu
Gestisce l'interfaccia utente per:
- Login
- Registrazione
- Gestione admin
- Cambio password

Utilizza il `ConsoleManager` per le operazioni di I/O sicure.

### JwtAuthenticationService
```csharp
public class JwtAuthenticationService
{
    public async Task<string> GenerateTokenAsync(User user)
    {
        // JWT generation logic
    }

    public async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
    {
        // Token validation logic
    }
}
```

### Password Hashing
```csharp
public class PasswordService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

## 📡 API Endpoints

### RESTful API Structure
```
GET    /api/v1/media              # List all media items
GET    /api/v1/media/{id}         # Get specific media item
POST   /api/v1/media              # Create new media item
PUT    /api/v1/media/{id}         # Update media item
DELETE /api/v1/media/{id}         # Delete media item

GET    /api/v1/loans              # List all loans
POST   /api/v1/loans              # Create new loan
PUT    /api/v1/loans/{id}/return  # Return a loan

POST   /api/v1/auth/login         # User login
POST   /api/v1/auth/register      # User registration
```

## 🔍 Sistema di Ricerca

### Implementazione Search
```csharp
public class SearchService
{
    private readonly IMediaRepository _mediaRepository;
    
    public async Task<SearchResult<MediaItem>> SearchAsync(
        SearchCriteria criteria)
    {
        var query = _mediaRepository.CreateQuery();
        
        // Apply filters
        if (!string.IsNullOrEmpty(criteria.Title))
            query = query.Where(m => m.Title.Contains(criteria.Title));
            
        // Apply sorting
        query = criteria.SortBy switch
        {
            SortCriteria.Title => query.OrderBy(m => m.Title),
            SortCriteria.Author => query.OrderBy(m => m.Author),
            _ => query.OrderBy(m => m.Id)
        };
        
        // Apply pagination
        return await query
            .Skip(criteria.Page * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();
    }
}
```

## 📊 Logging e Monitoring

### Logging Configuration
```csharp
public static class LoggingConfiguration
{
    public static ILoggingBuilder ConfigureLogging(
        this ILoggingBuilder builder)
    {
        return builder
            .AddSerilog()
            .AddApplicationInsights()
            .ConfigureStructuredLogging();
    }
}
```

Il sistema utilizza un sistema di logging per tracciare:
- Tentativi di login
- Operazioni di registrazione
- Modifiche ai ruoli
- Errori applicativi

### Metrics Collection
```csharp
public class MetricsCollector
{
    private readonly IMetricsClient _metrics;
    
    public void TrackLoanMetrics(Loan loan)
    {
        _metrics.TrackEvent("LoanCreated");
        _metrics.TrackMetric("LoanDuration", 
            (loan.DueDate - loan.LoanDate).Days);
    }
}
```

## 🔄 Background Jobs

### Job Definitions
```csharp
public class OverdueLoansJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // Check for overdue loans
        // Send notifications
    }
}

public class MediaAvailabilityJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // Update media availability status
        // Process reservations
    }
}
```

## 🎯 Caching Strategy

### Cache Implementation
```csharp
public class CacheService<T>
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _defaultExpiration;
    
    public async Task<T> GetOrSetAsync(
        string key, 
        Func<Task<T>> factory)
    {
        var cached = await _cache.GetAsync<T>(key);
        if (cached != null)
            return cached;
            
        var value = await factory();
        await _cache.SetAsync(key, value, _defaultExpiration);
        return value;
    }
}
```

## 🚀 Deployment

### Docker Configuration
```dockerfile
# API Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "Library.API.dll"]
```

### Kubernetes Deployment
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: library-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: library-api
  template:
    metadata:
      labels:
        app: library-api
    spec:
      containers:
      - name: library-api
        image: library-api:latest
        ports:
        - containerPort: 80
```

## 📈 Monitoring e Health Checks

### Health Check Endpoints
```csharp
public class HealthCheckController : ControllerBase
{
    [HttpGet("/health")]
    public IActionResult CheckHealth()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow
        });
    }
}
```

## 🧪 Test

### Test Project Setup
```csharp
public class TestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTestDatabase();
        services.AddTestAuthentication();
        services.AddMockServices();
    }
}
```

### Test Unitari
- Test dei servizi di autenticazione
- Test della validazione
- Test della gestione ruoli
- Test della logica di business

### Test di Integrazione
- Test del flusso completo di autenticazione
- Test della persistenza dati
- Test delle operazioni admin
- Test delle operazioni di I/O della console

### Integration Test Example
```csharp
public class MediaControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    [Fact]
    public async Task CreateMedia_ValidInput_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateClient();
        var mediaItem = new MediaItemDto { /* ... */ };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/media", mediaItem);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

## 📦 Dependency Injection e Gestione Errori

### Service Registration
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLibraryServices(
        this IServiceCollection services)
    {
        services.AddSingleton<ConsoleManager>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthenticationService, JwtAuthenticationService>();
        
        return services;
    }
}
```

### Gestione Errori
Il sistema implementa una gestione degli errori a più livelli:
1. Livello applicazione:
   - Gestione shutdown graceful
   - Logging delle operazioni critiche
2. Livello servizi:
   - Validazione input
   - Gestione eccezioni di business
3. Livello UI:
   - Feedback utente
   - Gestione errori di I/O

## Best Practices Implementate
1. SOLID Principles
   - Single Responsibility Principle nei servizi
   - Dependency Inversion con interfacce
   - Open/Closed Principle nelle estensioni

2. Thread Safety
   - Lock su operazioni critiche
   - Gestione thread-safe della console
   - Gestione sicura delle risorse condivise

3. Error Handling
   - Try-catch mirati
   - Logging appropriato
   - Feedback utente chiaro

4. Testing
   - Test unitari per la logica di business
   - Test di integrazione per i flussi completi
   - Mocking delle dipendenze
