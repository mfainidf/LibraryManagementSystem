# Script PowerShell per creare le issue della migrazione web

# Epic Web Migration
gh issue create --title "Epic: Migrazione da Console UI a Web UI" `
    --body "## Descrizione
Implementazione della migrazione dall'interfaccia console all'interfaccia web, mantenendo la business logic esistente.

### Obiettivi
- [ ] Creazione Web API layer
- [ ] Sviluppo interfaccia web
- [ ] Mantenimento della business logic esistente
- [ ] Coesistenza temporanea console/web
- [ ] Testing completo delle funzionalità

### Fasi
1. Preparazione e analisi
2. Setup infrastruttura web
3. Migrazione graduale feature
4. Testing e validazione
5. Deployment" `
    --label "epic" --label "web-migration" --label "priority:high"

# Fase 1: Preparazione
gh issue create --title "Analisi e Preparazione Migrazione Web" `
    --body "## Descrizione
Analisi dell'architettura esistente e preparazione per la migrazione web.

### Tasks
- [ ] Analisi dipendenze UI attuali
- [ ] Identificazione punti di accoppiamento
- [ ] Definizione interfacce di astrazione
- [ ] Piano di refactoring
- [ ] Documentazione architettura target" `
    --label "web-migration" --label "analysis" --label "priority:high"

# Fase 2: Setup Web API
gh issue create --title "Setup Web API Layer" `
    --body "## Descrizione
Implementazione del layer API per esporre le funzionalità esistenti via REST.

### Tasks
- [ ] Setup progetto ASP.NET Web API
- [ ] Configurazione routing
- [ ] Implementazione middleware autenticazione
- [ ] Setup Swagger/OpenAPI
- [ ] Configurazione CORS
- [ ] Unit test API endpoints" `
    --label "web-migration" --label "api" --label "priority:high"

# Fase 3: Setup Web Client
gh issue create --title "Setup Web Client React/TypeScript" `
    --body "## Descrizione
Setup del progetto client web con React e TypeScript.

### Tasks
- [ ] Setup progetto React con TypeScript
- [ ] Configurazione build system
- [ ] Setup routing client-side
- [ ] Implementazione stato applicazione
- [ ] Setup testing framework
- [ ] Configurazione CI/CD" `
    --label "web-migration" --label "frontend" --label "priority:high"

# Fase 4: Autenticazione Web
gh issue create --title "Migrazione Sistema Autenticazione" `
    --body "## Descrizione
Migrazione del sistema di autenticazione all'interfaccia web.

### Tasks
- [ ] Implementazione JWT authentication
- [ ] Form login/registrazione
- [ ] Gestione sessione client
- [ ] Protezione route
- [ ] Test end-to-end autenticazione" `
    --label "web-migration" --label "authentication" --label "priority:high"

# Fase 5: Features Base
gh issue create --title "Migrazione Features Base" `
    --body "## Descrizione
Migrazione delle funzionalità base all'interfaccia web.

### Tasks
- [ ] Lista catalogo
- [ ] Dettaglio item
- [ ] Ricerca base
- [ ] Gestione profilo utente
- [ ] Test funzionalità migrate" `
    --label "web-migration" --label "feature" --label "priority:high"

# Fase 6: Features Avanzate
gh issue create --title "Migrazione Features Avanzate" `
    --body "## Descrizione
Migrazione delle funzionalità avanzate all'interfaccia web.

### Tasks
- [ ] Gestione prestiti
- [ ] Prenotazioni
- [ ] Report e dashboard
- [ ] Funzioni amministrative
- [ ] Test funzionalità avanzate" `
    --label "web-migration" --label "feature" --label "priority:medium"

# Fase 7: Testing e Deployment
gh issue create --title "Testing e Deployment Web UI" `
    --body "## Descrizione
Testing completo e preparazione al deployment della versione web.

### Tasks
- [ ] Test end-to-end
- [ ] Test prestazioni
- [ ] Test sicurezza
- [ ] Setup ambiente staging
- [ ] Piano di deployment
- [ ] Documentazione deployment" `
    --label "web-migration" --label "testing" --label "deployment" --label "priority:high"
