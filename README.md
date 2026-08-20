# AdminDashboard-API

Backend API for the Admin Dashboard application. Acts as an intermediary between the AdminDashboard frontend and the Central API (oslofjord-gateway-api).

## Architecture

```
AdminDashboard (port 3002)
         ↓
AdminDashboard-API (port 5200) ← This service
         ↓
Central API (port 5100) → Cosmos DB
```

## Features

- RESTful API for event management
- CORS configuration for AdminDashboard frontend
- Event enrichment operations
- Proxy to Central API with admin-specific logic
- TypeScript for type safety
- Express.js framework

## Setup

1. Install dependencies:
```bash
npm install
```

2. Create `.env` file:
```bash
cp .env.example .env
```

3. Configure environment variables in `.env`:
- `PORT=5200`
- `CENTRAL_API_URL=http://localhost:5100`
- `ALLOWED_ORIGINS=http://localhost:3002`

## Development

Start the development server:
```bash
npm run dev
```

## Production

Build and run:
```bash
npm run build
npm start
```

## API Endpoints

### Health Check
- `GET /api/health` - Service health status

### Events
- `GET /api/events` - Get all events
- `GET /api/events/:id` - Get event by ID
- `POST /api/events` - Create new event
- `PUT /api/events/:id` - Update event
- `DELETE /api/events/:id` - Delete event
- `POST /api/events/:id/enrich` - Enrich event with additional data

## Tech Stack

- Node.js
- Express.js
- TypeScript
- Axios (HTTP client)
- dotenv (Environment configuration)
