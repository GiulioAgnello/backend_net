# Lemura Backend API

API REST backend per la gestione di prenotazioni e strutture ricettive.

## Panoramica

Lemura Backend è un'API ASP.NET Core 10.0 che fornisce servizi di autenticazione, gestione prenotazioni, gestioni strutture e sincronizzazione calendari iCal.

## Tecnologie

- **Framework:** ASP.NET Core 10.0
- **Database:** SQLite
- **Autenticazione:** JWT Bearer
- **Security:** BCrypt password hashing
- **API Documentation:** Swagger/OpenAPI
- **ORM:** Entity Framework Core 10.0

## Architettura

### Struttura Moduli

```
Controllers/     → Endpoint API
Models/          → Entità dati
Data/            → Context database
Services/        → Logica di business
Migrations/      → Versionamento database
```

### Moduli Principali

- **Auth:** Autenticazione e gestione utenti
- **Bookings:** Gestione prenotazioni
- **Rooms:** Gestione strutture e camere
- **Reviews:** Sistema recensioni
- **Contact:** Gestione contatti
- **iCalSync:** Sincronizzazione calendari

## Prerequisiti

- .NET 10.0 SDK
- SQLite

## Installazione

```bash
# Clonare il repository
git clone <repository-url>
cd lemuraBack.Api

# Installare dipendenze
dotnet restore

# Applicare migrazioni database
dotnet ef database update

# Eseguire l'applicazione
dotnet run
```

## Configurazione

### Variabili Ambiente

```json
{
  "Jwt:Secret": "your-secret-key"
}
```

Default: `LeMuraAngeli2024SecretKeyVeryLong!`

### CORS

API configurata per accettare richieste da qualsiasi origine. Modificare in produzione secondo necessità.

## Endpoint API

### Base URL

- **Development HTTP:** `http://localhost:5214`
- **Development HTTPS:** `https://localhost:7278`

### Documentazione

Swagger disponibile su: `/swagger`

## Database

Database SQLite: `lemura.db`

### Schema

- **Users:** Utenti con autenticazione JWT
- **Rooms:** Strutture e camere disponibili
- **Bookings:** Prenotazioni
- **Reviews:** Recensioni e valutazioni
- **Contacts:** Contatti

## Build e Deploy

```bash
# Build Release
dotnet build -c Release

# Publish
dotnet publish -c Release -o ./publish
```

## Troubleshooting

Se l'applicazione non parte:

1. Verificare porte 5214/7278 disponibili
2. Controllare configurazione appsettings.json
3. Eseguire `dotnet ef database update` per migrazioni

## License

Proprietario. Tutti i diritti riservati.
s
