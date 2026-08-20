# AdminDashboard-API

Backend API for the Admin Dashboard application - **.NET 10.0 / C# microservice** that integrates with the Oslofjord ecosystem.

## Architecture

```
External Systems (Momentus, RMS)
         ↓
AdminDashboard-API (port 5200) ← This service
    ↓               ↓
Cosmos DB      AdminDashboard
              (port 3002)
```

### Three-Tier Microservices System

```
External Systems (Momentus, RMS, Persons.API)
         ↓
Central API (port 5100) → Cosmos DB
    ↙              ↘
Admin-API         Kabuki-API
(port 5200)       (port 5000)
    ↓                 ↓
AdminDashboard    Kabuki
(port 3002)       (port 3001)
```

## Features

### Core Functionality
- ✅ **Event Management**: Create, read, update, delete enriched events
- ✅ **Event Enrichment**: Add descriptions, image galleries, custom properties
- ✅ **Momentus Integration**: Import and sync events from Momentus API
- ✅ **RMS Integration**: Room availability and booking management
- ✅ **Room Types**: Manage accommodation types and pricing
- ✅ **Additions**: Extra services (food, equipment, transport)
- ✅ **Packages**: Bundled offerings with events, rooms, and additions

### Technical Features
- ✅ Azure Cosmos DB for persistent storage
- ✅ RESTful API with Swagger/OpenAPI documentation
- ✅ Dependency Injection
- ✅ Repository pattern for data access
- ✅ External API service integration
- ✅ CORS configuration for frontend apps
- ✅ Application Insights telemetry
- ✅ Docker support
- ✅ .NET 10.0 / C#

## Project Structure

```
AdminDashboard-API/
├── src/
│   ├── Oslofjord.AdminDashboard.Api/           # Main API project
│   │   ├── Controllers/                        # API endpoints
│   │   │   ├── EventsController.cs
│   │   │   └── HealthController.cs
│   │   ├── Services/                           # Business logic
│   │   │   ├── EventService.cs
│   │   │   ├── MomentusService.cs
│   │   │   └── RmsService.cs
│   │   ├── Data/                               # Data access
│   │   │   └── CosmosDbRepository.cs
│   │   ├── Configuration/                      # Settings
│   │   │   ├── CosmosDbSettings.cs
│   │   │   └── ExternalApiSettings.cs
│   │   ├── appsettings.json
│   │   └── Program.cs
│   ├── Oslofjord.AdminDashboard.Contracts/     # Shared models & DTOs
│   │   ├── Models/
│   │   │   ├── EnrichedEvent.cs
│   │   │   ├── RoomType.cs
│   │   │   ├── Addition.cs
│   │   │   └── Package.cs
│   │   └── Dtos/
│   │       ├── EventDtos.cs
│   │       └── ResourceDtos.cs
│   └── Oslofjord.AdminDashboard.Client/        # Client library (future)
├── tests/                                       # Unit/integration tests
├── k8s/                                         # Kubernetes manifests
├── Dockerfile
└── AdminDashboard-Api.slnx
```

## Setup

### Prerequisites
- .NET 10.0 SDK
- Azure Cosmos DB (or Cosmos DB Emulator for local development)
- Access to Momentus, RMS, and other Oslofjord APIs

### Local Development

1. **Install dependencies:**
```bash
dotnet restore
```

2. **Configure settings:**

Create or update `appsettings.Development.json`:
```json
{
  "CosmosDb": {
    "EndpointUri": "https://localhost:8081/",
    "PrimaryKey": "your-cosmos-emulator-key",
    "DatabaseName": "AdminDashboardDb"
  },
  "ExternalApis": {
    "MomentusApiUrl": "http://localhost:5101",
    "RmsApiUrl": "http://localhost:5102",
    "PersonsApiUrl": "http://localhost:5103",
    "EventsApiUrl": "http://localhost:5104"
  }
}
```

3. **Run Cosmos DB Emulator:**
```bash
# Windows
C:\Program Files\Azure Cosmos DB Emulator\CosmosDB.Emulator.exe

# Docker
docker run -p 8081:8081 -p 10251:10251 -p 10252:10252 -p 10253:10253 -p 10254:10254 \
  mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator
```

4. **Create Cosmos DB Database & Containers:**

Use Azure Portal, Cosmos DB Emulator UI, or run this script to create the database:
```bash
# Database: AdminDashboardDb
# Containers:
# - EnrichedEvents (partition key: /id)
# - RoomTypes (partition key: /id)
# - Additions (partition key: /id)
# - Packages (partition key: /id)
```

5. **Run the API:**
```bash
cd src/Oslofjord.AdminDashboard.Api
dotnet run
```

API will be available at: **http://localhost:5200**
Swagger UI: **http://localhost:5200**

### Docker

Build and run with Docker:
```bash
docker build -t admindashboard-api .
docker run -p 5200:5200 admindashboard-api
```

## API Endpoints

### Health Check
- `GET /api/health` - Service health status

### Events
- `GET /api/events` - Get all events
- `GET /api/events/{id}` - Get event by ID
- `POST /api/events` - Create new event
- `PUT /api/events/{id}` - Update event
- `DELETE /api/events/{id}` - Delete event
- `POST /api/events/{id}/enrich` - Enrich event with additional data
- `POST /api/events/import` - Import event from Momentus

### Example: Create Event
```bash
curl -X POST http://localhost:5200/api/events \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Summer Conference 2026",
    "description": "Annual summer conference",
    "startDate": "2026-06-15T09:00:00Z",
    "endDate": "2026-06-17T17:00:00Z",
    "location": "Oslo",
    "type": 0,
    "isBookable": true,
    "maxParticipants": 100,
    "basePrice": 2500.00
  }'
```

### Example: Import from Momentus
```bash
curl -X POST http://localhost:5200/api/events/import \
  -H "Content-Type: application/json" \
  -d '{
    "momentusId": "evt-12345",
    "autoEnrich": true
  }'
```

## Integration with AdminDashboard Frontend

Update AdminDashboard's `.env.local`:
```env
NEXT_PUBLIC_API_URL=http://localhost:5200
```

The frontend will now use this API instead of in-memory data.

## Tech Stack

- **.NET 10.0** / C#
- **ASP.NET Core Web API**
- **Azure Cosmos DB** (NoSQL database)
- **Microsoft.Azure.Cosmos** SDK
- **AutoMapper** (object mapping)
- **Application Insights** (monitoring)
- **Swagger/OpenAPI** (API documentation)
- **Docker** (containerization)

## Deployment

### Azure
- Deploy as Azure App Service
- Connect to Azure Cosmos DB
- Configure Application Insights
- Set environment variables for production

### Kubernetes
Kubernetes manifests will be added to the `k8s/` directory.

## Environment Variables

Production environment variables:
```
CosmosDb__EndpointUri=https://your-account.documents.azure.com:443/
CosmosDb__PrimaryKey=your-production-key
CosmosDb__DatabaseName=AdminDashboardDb
ExternalApis__MomentusApiUrl=https://momentus-api.yourdomain.com
ExternalApis__RmsApiUrl=https://rms-api.yourdomain.com
ExternalApis__MomentusApiKey=your-momentus-key
ExternalApis__RmsApiKey=your-rms-key
```

## Next Steps

- [ ] Add authentication/authorization (JWT)
- [ ] Implement RoomTypes, Additions, Packages controllers
- [ ] Add unit tests
- [ ] Add integration tests
- [ ] Set up CI/CD pipeline
- [ ] Add rate limiting
- [ ] Add request validation
- [ ] Implement caching strategy
- [ ] Add logging with Serilog
- [ ] Create Kubernetes manifests

## Contributing

Follow the Oslofjord engineering standards:
- Use C# for backend services
- Use Azure-native databases
- Keep external dependencies minimal
- Write clear, maintainable code

## License

Internal Oslofjord project
