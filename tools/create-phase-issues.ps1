# Script PowerShell per creare le issue relative alle fasi di progetto

# Fase 2: Gestione Catalogo Multimediale
gh issue create --title "Epic: Gestione Catalogo Multimediale" `
    --body "## Descrizione
Implementazione del sistema di gestione del catalogo multimediale della biblioteca.

### Funzionalità Core
- [ ] CRUD completo per risorse multimediali
- [ ] Gestione metadati specifici per tipo di risorsa
- [ ] Sistema di lookup per categorie e generi
- [ ] Soft delete per mantenere storico

### Miglioramenti Proposti
- [ ] Implementazione pattern Repository
- [ ] Sistema di validazione dati
- [ ] Gestione delle versioni per le modifiche
- [ ] Cache system per ottimizzare le performance
- [ ] API standardizzate per futura implementazione web" `
    --label "epic" --label "catalog" --label "priority:high"

gh issue create --title "Implementazione CRUD Risorse Multimediali" `
    --body "## Descrizione
Implementazione delle operazioni CRUD per le risorse multimediali.

### Tasks
- [ ] Definizione modello MediaItem
- [ ] Implementazione Create
- [ ] Implementazione Read
- [ ] Implementazione Update
- [ ] Implementazione Delete (soft)
- [ ] Validazione dati
- [ ] Unit test per ogni operazione" `
    --label "catalog" --label "feature" --label "priority:high"

gh issue create --title "Sistema di Gestione Metadati" `
    --body "## Descrizione
Implementazione del sistema di gestione metadati specifici per tipo di risorsa.

### Tasks
- [ ] Definizione modelli metadati per libri
- [ ] Definizione modelli metadati per riviste
- [ ] Definizione modelli metadati per risorse digitali
- [ ] Sistema di validazione metadati
- [ ] Unit test" `
    --label "catalog" --label "feature" --label "priority:high"

gh issue create --title "Gestione Categorie e Generi" `
    --body "## Descrizione
Implementazione del sistema di lookup per categorie e generi.

### Tasks
- [ ] Definizione modello Category
- [ ] Definizione modello Genre
- [ ] Sistema di gestione gerarchie categorie
- [ ] API per lookup
- [ ] Unit test" `
    --label "catalog" --label "feature" --label "priority:medium"

# Fase 3: Sistema di Ricerca Avanzato
gh issue create --title "Epic: Sistema di Ricerca Avanzato" `
    --body "## Descrizione
Implementazione del sistema di ricerca avanzata nel catalogo.

### Funzionalità Core
- [ ] Ricerca multi-criterio
- [ ] Filtri per tipo di risorsa
- [ ] Ordinamento risultati

### Miglioramenti Proposti
- [ ] Ricerca full-text
- [ ] Filtri dinamici
- [ ] Sistema di suggerimenti
- [ ] Paginazione
- [ ] Export risultati" `
    --label "epic" --label "search" --label "priority:high"

gh issue create --title "Implementazione Ricerca Multi-criterio" `
    --body "## Descrizione
Implementazione del motore di ricerca con supporto multi-criterio.

### Tasks
- [ ] Definizione criteri di ricerca
- [ ] Implementazione query builder
- [ ] Sistema di filtri
- [ ] Sistema di ordinamento
- [ ] Unit test" `
    --label "search" --label "feature" --label "priority:high"

gh issue create --title "Sistema di Paginazione e Export" `
    --body "## Descrizione
Implementazione della paginazione dei risultati e export in vari formati.

### Tasks
- [ ] Implementazione paginazione
- [ ] Export in CSV
- [ ] Export in PDF
- [ ] Configurazione dimensione pagina
- [ ] Unit test" `
    --label "search" --label "feature" --label "priority:medium"

# Fase 4: Gestione Prestiti e Prenotazioni
gh issue create --title "Epic: Gestione Prestiti e Prenotazioni" `
    --body "## Descrizione
Implementazione del sistema di gestione prestiti e prenotazioni.

### Funzionalità Core
- [ ] Sistema di prestiti con date
- [ ] Gestione prenotazioni
- [ ] Visualizzazione disponibilità
- [ ] Report amministrativi

### Miglioramenti Proposti
- [ ] Sistema di notifiche per scadenze
- [ ] Calendario disponibilità
- [ ] QR code per gestione prestiti
- [ ] Dashboard amministrativa
- [ ] Sistema di rinnovo prestiti online" `
    --label "epic" --label "loans" --label "priority:high"

gh issue create --title "Sistema di Prestiti" `
    --body "## Descrizione
Implementazione del core system per la gestione prestiti.

### Tasks
- [ ] Definizione modello Loan
- [ ] Gestione date prestito/restituzione
- [ ] Verifica disponibilità
- [ ] Sistema di rinnovo
- [ ] Unit test" `
    --label "loans" --label "feature" --label "priority:high"

gh issue create --title "Sistema di Prenotazioni" `
    --body "## Descrizione
Implementazione del sistema di prenotazioni.

### Tasks
- [ ] Definizione modello Reservation
- [ ] Gestione code di prenotazione
- [ ] Notifiche disponibilità
- [ ] Cancellazione prenotazioni
- [ ] Unit test" `
    --label "loans" --label "feature" --label "priority:high"

# Fase 5: Sistema Preview Risorse
gh issue create --title "Epic: Sistema Preview Risorse" `
    --body "## Descrizione
Implementazione del sistema di preview per le risorse multimediali.

### Funzionalità Core
- [ ] Upload file preview
- [ ] Visualizzazione/download preview

### Miglioramenti Proposti
- [ ] Preview multiple per risorsa
- [ ] Compressione automatica immagini
- [ ] Streaming sicuro contenuti
- [ ] Verifica formato file
- [ ] Gestione copyright e DRM" `
    --label "epic" --label "preview" --label "priority:medium"

gh issue create --title "Sistema di Upload e Storage" `
    --body "## Descrizione
Implementazione del sistema di upload e storage delle preview.

### Tasks
- [ ] Sistema di upload file
- [ ] Validazione formati
- [ ] Gestione storage
- [ ] Compressione immagini
- [ ] Unit test" `
    --label "preview" --label "feature" --label "priority:medium"

# Fase 6: Workflow Proposte d'Acquisto
gh issue create --title "Epic: Workflow Proposte d'Acquisto" `
    --body "## Descrizione
Implementazione del workflow per la gestione delle proposte d'acquisto.

### Funzionalità Core
- [ ] Form proposta acquisto
- [ ] Workflow approvazione multi-livello
- [ ] Gestione ordini

### Miglioramenti Proposti
- [ ] Sistema di notifiche
- [ ] Integration con sistemi esterni
- [ ] Dashboard monitoraggio budget
- [ ] Workflow configurabile
- [ ] Integrazione fornitori" `
    --label "epic" --label "purchase" --label "priority:low"

gh issue create --title "Form e Workflow Approvazione" `
    --body "## Descrizione
Implementazione del form di proposta e del workflow di approvazione.

### Tasks
- [ ] Form proposta acquisto
- [ ] Workflow multi-livello
- [ ] Sistema notifiche
- [ ] Dashboard approvazioni
- [ ] Unit test" `
    --label "purchase" --label "feature" --label "priority:low"

gh issue create --title "Gestione Budget e Ordini" `
    --body "## Descrizione
Implementazione del sistema di gestione budget e ordini.

### Tasks
- [ ] Tracking budget
- [ ] Gestione ordini
- [ ] Integrazione fornitori
- [ ] Report finanziari
- [ ] Unit test" `
    --label "purchase" --label "feature" --label "priority:low"
