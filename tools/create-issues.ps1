# Script PowerShell per creare le issue su GitHub

# Core e Infrastruttura
gh issue create --title "Configurazione Iniziale del Progetto" `
    --body "## Descrizione
Setup iniziale della struttura del progetto con cartelle core, infrastructure, tests

### Tasks
- [ ] Creazione struttura cartelle
- [ ] Setup .gitignore
- [ ] Configurazione soluzione .NET
- [ ] Setup dei riferimenti tra progetti" `
    --label "setup" --label "infrastructure" --label "priority:high"

gh issue create --title "Setup Database e Migrations" `
    --body "## Descrizione
Implementazione della struttura del database e migrations

### Tasks
- [ ] Configurazione Entity Framework
- [ ] Creazione migration iniziale
- [ ] Setup connection string
- [ ] Implementazione DbContext" `
    --label "database" --label "setup" --label "priority:high"

gh issue create --title "Implementazione Modelli di Dominio" `
    --body "## Descrizione
Implementazione delle entità principali del sistema

### Tasks
- [ ] User model
- [ ] MediaItem model
- [ ] Loan model
- [ ] Implementazione relazioni" `
    --label "domain" --label "models" --label "priority:high"

gh issue create --title "Repository Pattern Implementation" `
    --body "## Descrizione
Implementazione dei repository per l'accesso ai dati

### Tasks
- [ ] IRepository<T> interface
- [ ] Repository base implementation
- [ ] Unit of Work pattern
- [ ] Repository specifici per entità" `
    --label "infrastructure" --label "pattern" --label "priority:high"

# Autenticazione e Sicurezza
gh issue create --title "Implementazione ConsoleManager Thread-Safe" `
    --body "## Descrizione
Implementazione del gestore thread-safe delle operazioni console

### Tasks
- [ ] Singleton pattern
- [ ] Lock mechanism
- [ ] IDisposable implementation
- [ ] Error handling" `
    --label "console" --label "thread-safety" --label "priority:high"

gh issue create --title "Sistema di Autenticazione" `
    --body "## Descrizione
Implementazione del sistema di autenticazione JWT

### Tasks
- [ ] JWT service
- [ ] Password hashing
- [ ] Token validation
- [ ] User authentication flow" `
    --label "authentication" --label "security" --label "priority:high"

gh issue create --title "Menu di Autenticazione" `
    --body "## Descrizione
Implementazione dell'interfaccia utente per l'autenticazione

### Tasks
- [ ] Login UI
- [ ] Registration UI
- [ ] Password change
- [ ] Admin management UI" `
    --label "ui" --label "authentication" --label "priority:high"

# API e Servizi
gh issue create --title "Implementazione API Endpoints" `
    --body "## Descrizione
Implementazione degli endpoint REST API

### Tasks
- [ ] Media endpoints
- [ ] Loan endpoints
- [ ] Authentication endpoints
- [ ] Swagger documentation" `
    --label "api" --label "endpoints" --label "priority:medium"

gh issue create --title "Sistema di Ricerca" `
    --body "## Descrizione
Implementazione del sistema di ricerca

### Tasks
- [ ] Search criteria
- [ ] Filtri
- [ ] Ordinamento
- [ ] Paginazione" `
    --label "search" --label "feature" --label "priority:medium"

# Logging e Monitoring
gh issue create --title "Sistema di Logging" `
    --body "## Descrizione
Implementazione del sistema di logging

### Tasks
- [ ] Serilog setup
- [ ] Application Insights
- [ ] Log levels
- [ ] Log storage" `
    --label "logging" --label "monitoring" --label "priority:medium"

gh issue create --title "Metriche e Monitoring" `
    --body "## Descrizione
Implementazione del sistema di metriche

### Tasks
- [ ] MetricsCollector
- [ ] Eventi di business
- [ ] Performance metrics
- [ ] Health checks" `
    --label "monitoring" --label "metrics" --label "priority:medium"

# Background Jobs
gh issue create --title "Job Scheduler" `
    --body "## Descrizione
Implementazione dei job in background

### Tasks
- [ ] OverdueLoansJob
- [ ] MediaAvailabilityJob
- [ ] Job scheduler
- [ ] Error handling" `
    --label "jobs" --label "background" --label "priority:low"

# Caching
gh issue create --title "Sistema di Cache" `
    --body "## Descrizione
Implementazione del sistema di caching

### Tasks
- [ ] CacheService
- [ ] Cache invalidation
- [ ] Distributed cache
- [ ] Cache policies" `
    --label "cache" --label "performance" --label "priority:low"

# Testing
gh issue create --title "Unit Tests" `
    --body "## Descrizione
Implementazione dei test unitari

### Tasks
- [ ] Authentication tests
- [ ] Repository tests
- [ ] Service tests
- [ ] Console manager tests" `
    --label "testing" --label "quality" --label "priority:high"

gh issue create --title "Integration Tests" `
    --body "## Descrizione
Implementazione dei test di integrazione

### Tasks
- [ ] API tests
- [ ] Database tests
- [ ] Authentication flow
- [ ] E2E scenarios" `
    --label "testing" --label "integration" --label "priority:high"

# Deployment
gh issue create --title "Containerization" `
    --body "## Descrizione
Setup della containerizzazione

### Tasks
- [ ] Dockerfile
- [ ] Docker compose
- [ ] Environment variables
- [ ] Build scripts" `
    --label "deployment" --label "docker" --label "priority:low"

gh issue create --title "Kubernetes Setup" `
    --body "## Descrizione
Configurazione Kubernetes

### Tasks
- [ ] Deployment files
- [ ] Service definition
- [ ] Scaling rules
- [ ] Health probes" `
    --label "deployment" --label "kubernetes" --label "priority:low"
