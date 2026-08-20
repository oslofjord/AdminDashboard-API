# AdminDashboard-API

**Backend for Frontend (BFF)** for the AdminDashboard application. Acts as a proxy between the AdminDashboard frontend and the events-central-api.

## Architecture

```
AdminDashboard Frontend (port 3002)
         ↓
AdminDashboard-API (port 5200) ← This service (BFF/Proxy)
         ↓
events-central-api (port 5100) → Cosmos DB, Momentus, RMS
```

### Three-Tier Microservices System

```
External Systems (Momentus, RMS, Persons.API)
         ↓
events-central-api (port 5100) → Cosmos DB
         ↓
AdminDashboard-API (port 5200) ← This service
         ↓
AdminDashboard Frontend (port 3002)
```

## Purpose

AdminDashboard-API is a **lightweight proxy/BFF** that:
- ✅ Fetches data from events-central-api
- ✅ Provides admin-specific endpoints
- ✅ Handles CORS for AdminDashboard frontend
- ✅ Adds admin-specific business logic/transformations
- ✅ NO direct database connection
- ✅ NO direct external API calls (Momentus/RMS)
- ✅ Simple HTTP proxy to events-central-api

## Features

### Core Functionality
- ✅ **Event Management**: Proxy CRUD operations to events-central-api
- ✅ **Event Enrichment**: Forward enrichment requests
- ✅ **Momentus Import**: Proxy import requests
- ✅ **Room Types**: Fetch from events-central-api
- ✅ **Additions**: Fetch from events-central-api
- ✅ **Packages**: Fetch from events-central-api

### Technical Features
- ✅ ASP.NET Core Web API (.NET 10.0)
- ✅ HTTP Client for events-central-api communication
- ✅ RESTful API with Swagger/OpenAPI documentation
- ✅ Dependency Injection
- ✅ CORS configuration for AdminDashboard frontend
- ✅ Application Insights telemetry (optional)
- ✅ Docker support

## Project Structure

```
AdminDashboard-API/
├── src/
│   ├── Oslofjord.AdminDashboard.Api/           # Main API project (BFF)
│   │   ├── Controllers/                        # API endpoints
│   │   │   ├── EventsController.cs             # Proxy to events-central-api
│   │   │   └── HealthController.cs             # Health check
│   │   ├── Services/                           # HTTP clients
│   │   │   └── CentralApiService.cs            # HTTP client for events-central-api
│   │   ├── Configuration/                      # Settings
│   │   │   └── CentralApiSettings.cs           # events-central-api URL config
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
├── Dockerfile
└── AdminDashboard-Api.slnx
```

## Setup

### Prerequisites
- .NET 10.0 SDK
- events-central-api running on port 5100

### Local Development

1. **Install dependencies:**
```bash
dotnet restore
```

2. **Configure settings:**

The API is pre-configured to connect to `http://localhost:5100` (events-central-api).

Update `appsettings.Development.json` if needed:
```json
{
  "CentralApi": {
    "BaseUrl": "http://localhost:5100",
    "ApiKey": "",
    "TimeoutSeconds": 30
  }
}
```

3. **Run the API:**
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
docker run -p 5200:5200 -e CentralApi__BaseUrl=http://host.docker.internal:5100 admindashboard-api
```

## API Endpoints

All endpoints proxy requests to events-central-api (port 5100):

### Health Check
- `GET /api/health` - Service health status

### Events (Proxied to events-central-api)
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

**Note:** This request is proxied to `http://localhost:5100/api/events`

## Integration with AdminDashboard Frontend

Update AdminDashboard's `.env.local`:
```env
NEXT_PUBLIC_API_URL=http://localhost:5200
```

The frontend connects to this API (port 5200), which then proxies to events-central-api (port 5100).

## Tech Stack

- **.NET 10.0** / C#
- **ASP.NET Core Web API**
- **HttpClient** for events-central-api communication
- **Swagger/OpenAPI** (API documentation)
- **Docker** (containerization)

## Configuration

### Development
```json
{
  "CentralApi": {
    "BaseUrl": "http://localhost:5100",
    "ApiKey": "",
    "TimeoutSeconds": 30
  }
}
```

### Production
Set via environment variables:
```
CentralApi__BaseUrl=https://events-central-api.yourdomain.com
CentralApi__ApiKey=your-api-key
CentralApi__TimeoutSeconds=30
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
