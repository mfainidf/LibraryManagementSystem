# Specifiche Tecniche - Sistema Gestione Biblioteca

## 🔄 Strategia di Migrazione Console → Web

### Riutilizzo del Backend Esistente
Il backend attuale è stato progettato seguendo i principi SOLID e Clean Architecture, permettendoci di riutilizzare completamente:

1. **Core Domain Layer**
   - Tutte le entità di dominio (User, MediaItem, Loan, etc.)
   - Le interfacce dei repository
   - I value objects e le enumerazioni
   - Le regole di business di dominio

2. **Business Logic Layer**
   - I servizi applicativi esistenti (AuthenticationService, etc.)
   - La logica di validazione
   - Le interfacce dei servizi
   - I gestori delle operazioni CRUD

3. **Infrastructure Layer**
   - Implementazioni dei repository
   - Configurazione del database
   - Gestione delle migrazioni
   - Servizi di logging

### Componenti da Adattare/Estendere

1. **Authentication**
   ```csharp
   // Esistente - Verrà riutilizzato
   public interface IAuthenticationService
   {
       Task<User> LoginAsync(string email, string password);
       Task<User> RegisterUserAsync(string name, string email, string password);
       // ... altri metodi ...
   }

   // Nuovo - Da aggiungere per Web
   public class JwtAuthenticationHandler
   {
       private readonly IAuthenticationService _authService;
       
       public async Task<string> GenerateTokenAsync(User user) { ... }
       public async Task<ClaimsPrincipal> ValidateTokenAsync(string token) { ... }
   }
   ```

2. **API Controllers**
   ```csharp
   // Nuovo - Wrapper API per servizi esistenti
   [ApiController]
   [Route("api/[controller]")]
   public class AuthController : ControllerBase
   {
       private readonly IAuthenticationService _authService;
       private readonly JwtAuthenticationHandler _jwtHandler;

       [HttpPost("login")]
       public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
       {
           // Riutilizzo del servizio esistente
           var user = await _authService.LoginAsync(request.Email, request.Password);
           var token = await _jwtHandler.GenerateTokenAsync(user);
           return new LoginResponse(token, user);
       }
   }
   ```

### Approccio di Migrazione
La migrazione seguirà questi passaggi:

1. **Fase Preparatoria** (No modifiche al backend esistente)
   - Analisi delle dipendenze UI attuali
   - Verifica della separazione UI/Business Logic
   - Identificazione punti di estensione necessari
   - Piano di implementazione API

2. **Architettura Web Target**
   ```
   ┌────────────────┐     ┌────────────────┐
   │   Web Client   │ ←── │    API Layer   │
   │   (React/TS)   │     │  (ASP.NET Web  │
   └────────────────┘     │     API)       │
           ↑              └────────────────┘
           │                      ↑
           │              ┌────────────────┐
           └────────────→ │  Business Logic│ ← Riutilizzo Completo
                         │     Layer      │
                         └────────────────┘
                               ↑
                         ┌────────────────┐
                         │    Domain &    │ ← Riutilizzo Completo
                         │ Infrastructure │
                         └────────────────┘
   ```

3. **Strategia di Implementazione**
   - Creazione nuovo progetto ASP.NET Web API
   - Riferimento ai progetti esistenti (Core, Infrastructure)
   - Implementazione controller come wrapper dei servizi
   - Aggiunta autenticazione JWT mantenendo la logica esistente
   - Sviluppo frontend React/TypeScript

4. **Priorità di Migrazione**
   1. Setup infrastruttura API (no modifiche al backend)
   2. Autenticazione (estensione con JWT)
   3. Operazioni di lettura catalogo (wrapper API)
   4. Operazioni di gestione prestiti (wrapper API)
   5. Funzionalità amministrative (wrapper API)

### Vantaggi dell'Approccio
1. **Riutilizzo Massimo**
   - Nessuna riscrittura della business logic
   - Mantenimento delle regole di dominio
   - Riutilizzo dei test esistenti

2. **Rischi Minimi**
   - Backend già testato e funzionante
   - Modifiche incrementali e sicure
   - Facilità di rollback

3. **Sviluppo Parallelo**
   - UI Console rimane funzionante
   - Sviluppo web senza impatto sul backend
   - Testing parallelo delle interfacce

## 📐 Architettura del Sistema

[...resto del documento...]
