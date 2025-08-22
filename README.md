# Library Management System

Un sistema completo per la gestione di una biblioteca, sviluppato come progetto di apprendimento.

## Funzionalità Principali

- Gestione utenti e autenticazione
- Gestione catalogo multimediale (libri, DVD, etc.)
- Sistema di prestiti e prenotazioni
- Preview delle risorse
- Workflow di proposte d'acquisto

## Tecnologie Utilizzate

- .NET 9.0
- Entity Framework Core
- SQLite
- xUnit per testing

## Struttura del Progetto

- `Library.Core`: Domain models e interfaces
- `Library.Infrastructure`: Implementazione della persistenza e servizi
- `Library.Console`: Interfaccia utente console

## Come Iniziare

1. Clona il repository
2. Naviga nella directory del progetto
3. Esegui `dotnet build`
4. Naviga in `src/Library.Console`
5. Esegui `dotnet run`

## Testing

Per eseguire i test:

```bash
dotnet test
```

## Stato del Progetto

- [x] Fase 1: Sistema di Autenticazione
- [ ] Fase 2: Gestione Catalogo
- [ ] Fase 3: Sistema di Ricerca
- [ ] Fase 4: Gestione Prestiti
- [ ] Fase 5: Sistema Preview
- [ ] Fase 6: Workflow Proposte
