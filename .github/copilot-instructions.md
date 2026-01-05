# GitHub Copilot Instructions - Library Management System

## Project Overview

This is a comprehensive Library Management System built with .NET 9.0, following Clean Architecture and Domain-Driven Design principles. The system manages library operations including user authentication, catalog management, loans, and reservations.

**Current State**: The project is transitioning from a console application to a web-based system while maintaining the existing console interface.

## Technology Stack

- **.NET 9.0**: Primary framework
- **Entity Framework Core 9.0.8**: Data access and ORM
- **SQLite**: Database engine
- **BCrypt.Net-Next 4.0.3**: Password hashing
- **Serilog**: Structured logging
- **xUnit**: Testing framework
- **ASP.NET Core**: Web API (in development)

## Project Structure

```
Library.Solution/
├── src/
│   ├── Library.Core/              # Domain models, interfaces (no dependencies)
│   ├── Library.Infrastructure/    # Data access, repositories, external services
│   ├── Library.Application/       # Business logic, use cases
│   ├── Library.API/              # Web API (in development)
│   ├── Library.Web/              # Web frontend (planned)
│   └── Library.Console/          # Console application
├── tests/
│   ├── Library.UnitTests/        # Unit tests
│   └── Library.IntegrationTests/ # Integration tests
└── docs/                         # Documentation
```

## Architecture Principles

### Clean Architecture Layers
1. **Core Layer** (`Library.Core`): Domain entities, interfaces, and enums. Should have NO dependencies on other projects.
2. **Infrastructure Layer** (`Library.Infrastructure`): Implements Core interfaces, contains EF Core DbContext, repositories, and external service integrations.
3. **Application Layer** (`Library.Application`): Business logic and use cases.
4. **Presentation Layer** (`Library.Console`, `Library.API`, `Library.Web`): User interfaces and API endpoints.

### Key Design Patterns
- **Repository Pattern**: Use for data access abstraction
- **Dependency Injection**: Always use constructor injection
- **Interface Segregation**: Keep interfaces small and focused
- **Async/Await**: Prefer async operations for I/O operations

## Coding Standards

### C# Style Guidelines
- **Naming Conventions**:
  - PascalCase for classes, methods, properties, and public members
  - camelCase for private fields with underscore prefix (`_fieldName`)
  - PascalCase for constants
  - Interfaces start with `I` prefix (e.g., `IUserRepository`)
  
- **Required Features**:
  - Enable nullable reference types (`<Nullable>enable</Nullable>`)
  - Use `required` keyword for required properties in models
  - Use implicit usings (`<ImplicitUsings>enable</ImplicitUsings>`)
  
- **Async Patterns**:
  - All repository methods should be async and return `Task<T>` or `Task`
  - Always suffix async methods with `Async` (e.g., `GetByIdAsync`)
  - Use `ConfigureAwait(false)` in library code, not in application code

### Code Organization
- One class per file
- File name should match the class name
- Organize using statements (System namespaces first, then third-party, then project namespaces)
- Use explicit access modifiers (always specify `public`, `private`, etc.)

### Models and Entities
- Place all domain models in `Library.Core/Models`
- Use properties with getters and setters, not fields
- Include validation attributes where appropriate
- Example:
```csharp
public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Interfaces
- Place all core interfaces in `Library.Core/Interfaces`
- Keep interfaces focused and cohesive
- Example:
```csharp
public interface IMediaItemRepository
{
    Task<MediaItem?> GetByIdAsync(int id);
    Task<IEnumerable<MediaItem>> GetAllAsync();
    Task<IEnumerable<MediaItem>> SearchAsync(SearchCriteria criteria);
    Task AddAsync(MediaItem item);
    Task UpdateAsync(MediaItem item);
    Task DeleteAsync(int id);
}
```

## Database and Data Access

### Entity Framework Core
- Use code-first approach with migrations
- Repository implementations go in `Library.Infrastructure/Repositories`
- DbContext is in `Library.Infrastructure/Data`
- Always use parameterized queries (EF Core handles this)
- Use `AsNoTracking()` for read-only queries

### Migrations
- Create migrations with descriptive names: `dotnet ef migrations add DescriptiveName`
- Always review generated migrations before applying
- Run migrations: `dotnet ef database update`

## Security Guidelines

### Authentication
- Use JWT tokens for API authentication
- Password hashing is done with BCrypt (never store plain text passwords)
- Password hashing service is in `Library.Infrastructure/Services`
- Implement role-based authorization (User, Administrator, Supervisor roles)

### Input Validation
- Validate all user inputs
- Sanitize data before database operations
- Use data annotations for model validation
- Never trust client-side validation alone

### Sensitive Data
- Never log passwords or tokens
- Store connection strings in configuration files (not in code)
- Use environment variables for production secrets
- Password hashes should use BCrypt with proper work factor

## Logging

### Serilog Configuration
- Use Serilog for all logging
- Log levels:
  - **Information**: Normal operations, state changes
  - **Warning**: Unexpected situations that don't prevent operation
  - **Error**: Errors that prevent a specific operation
  - **Debug**: Detailed information for debugging
- Include structured logging with properties
- Example:
```csharp
_logger.LogInformation("User {UserId} logged in successfully", userId);
_logger.LogError(ex, "Failed to create media item {Title}", title);
```

## Testing

### Unit Tests
- Place in `tests/Library.UnitTests`
- Use xUnit framework
- Name tests: `MethodName_Scenario_ExpectedResult`
- Follow Arrange-Act-Assert pattern
- Mock dependencies using interfaces
- Example:
```csharp
[Fact]
public async Task GetByIdAsync_ValidId_ReturnsUser()
{
    // Arrange
    var userId = 1;
    
    // Act
    var result = await _userRepository.GetByIdAsync(userId);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(userId, result.Id);
}
```

### Integration Tests
- Place in `tests/Library.IntegrationTests`
- Test complete workflows and database interactions
- Use in-memory database or test database
- Clean up test data after each test

## Build and Development Commands

### Building
```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/Library.Core/Library.Core.csproj
```

### Running
```bash
# Run console application
cd src/Library.Console
dotnet run

# Run API (when available)
cd src/Library.API
dotnet run
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Library.UnitTests/Library.UnitTests.csproj

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/Library.Infrastructure --startup-project src/Library.Console

# Update database
dotnet ef database update --project src/Library.Infrastructure --startup-project src/Library.Console

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/Library.Infrastructure --startup-project src/Library.Console
```

## Do's and Don'ts

### Do:
✅ Use async/await for all I/O operations  
✅ Follow the dependency direction: Presentation → Application → Infrastructure → Core  
✅ Use dependency injection for all services  
✅ Write unit tests for business logic  
✅ Use meaningful variable and method names  
✅ Log important operations and errors  
✅ Validate user inputs  
✅ Use BCrypt for password hashing  
✅ Keep Core layer free of dependencies  
✅ Use nullable reference types  

### Don't:
❌ Don't reference Infrastructure from Core  
❌ Don't store passwords in plain text  
❌ Don't use blocking calls (.Result, .Wait()) on async operations  
❌ Don't catch exceptions without logging them  
❌ Don't expose database entities directly in API responses  
❌ Don't use magic strings; use constants or enums  
❌ Don't ignore compiler warnings  
❌ Don't commit sensitive data (connection strings, secrets)  
❌ Don't bypass the repository pattern for data access  
❌ Don't mix business logic with presentation logic  

## Thread Safety

### Console Operations
- Use `ConsoleManager` singleton for thread-safe console I/O
- All console operations should go through `ConsoleManager`
- Lock critical sections properly
- Example structure:
```csharp
public class ConsoleManager : IDisposable
{
    private static readonly object _lock = new object();
    
    public void WriteLine(string message)
    {
        lock (_lock)
        {
            Console.WriteLine(message);
        }
    }
}
```

## Error Handling

### Exception Strategy
- Catch specific exceptions, not general `Exception`
- Log all exceptions with context
- Return meaningful error messages to users
- Don't expose stack traces to end users
- Use custom exceptions for business rule violations

### Example:
```csharp
try
{
    await _repository.SaveAsync(entity);
}
catch (DbUpdateException ex)
{
    _logger.LogError(ex, "Failed to save entity {EntityType}", typeof(T).Name);
    throw new ApplicationException("Failed to save data", ex);
}
```

## API Design (for Library.API)

### RESTful Conventions
- Use proper HTTP verbs: GET (read), POST (create), PUT (update), DELETE (remove)
- Use plural nouns for resource names: `/api/media`, `/api/users`
- Use proper status codes: 200 (OK), 201 (Created), 400 (Bad Request), 401 (Unauthorized), 404 (Not Found), 500 (Internal Error)
- Version APIs: `/api/v1/media`
- Return consistent response structures

### Example Endpoints:
```
GET    /api/v1/media              # List all media items
GET    /api/v1/media/{id}         # Get specific media item
POST   /api/v1/media              # Create new media item
PUT    /api/v1/media/{id}         # Update media item
DELETE /api/v1/media/{id}         # Delete media item

POST   /api/v1/auth/login         # User login
POST   /api/v1/auth/register      # User registration
```

## Migration from Console to Web

The project is currently in a migration phase from console to web:
- Keep console application functional during transition
- Extract business logic from console UI to Application layer
- Ensure services are UI-agnostic
- Both console and web should use the same business logic layer

## Performance Considerations

- Use `AsNoTracking()` for read-only queries
- Implement pagination for large result sets
- Cache frequently accessed, rarely changed data
- Use async operations to avoid blocking threads
- Consider using compiled queries for frequently executed queries

## Documentation

- Update technical documentation when making architectural changes
- Keep README.md current with setup instructions
- Document complex business logic with comments
- Use XML documentation comments for public APIs

## References

- [Technical Documentation](../docs/technical_documentation.md)
- [Library System Specifications](../docs/LibrarySystem_Specifications.md)
- [.NET 9.0 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Clean Architecture Principles](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

*Last Updated: 2026-01-05*
