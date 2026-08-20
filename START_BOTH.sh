#!/bin/bash

echo "🚀 Starting AdminDashboard Ecosystem"
echo "===================================="
echo ""

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET SDK not found. Please install .NET 10.0${NC}"
    exit 1
fi

echo -e "${GREEN}✅ .NET SDK found: $(dotnet --version)${NC}"

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo -e "${RED}❌ Node.js not found. Please install Node.js${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Node.js found: $(node --version)${NC}"
echo ""

# Start AdminDashboard-API in background
echo -e "${YELLOW}📦 Starting AdminDashboard-API (port 5200)...${NC}"
cd /Users/lena.matyjasik/Documents/AdminDashboard-API/src/Oslofjord.AdminDashboard.Api
dotnet run > /tmp/admindashboard-api.log 2>&1 &
API_PID=$!
echo -e "${GREEN}✅ AdminDashboard-API started (PID: $API_PID)${NC}"
echo "   Log: tail -f /tmp/admindashboard-api.log"
echo ""

# Wait a bit for API to start
echo "⏳ Waiting for API to initialize..."
sleep 5

# Check if API is running
if curl -s http://localhost:5200/api/health > /dev/null 2>&1; then
    echo -e "${GREEN}✅ AdminDashboard-API is healthy!${NC}"
else
    echo -e "${YELLOW}⚠️  API is starting... (this is OK if using Cosmos DB)${NC}"
fi
echo ""

# Start AdminDashboard frontend in background
echo -e "${YELLOW}🎨 Starting AdminDashboard Frontend (port 3002)...${NC}"
cd /Users/lena.matyjasik/Documents/AdminDashboard
npm run dev > /tmp/admindashboard-frontend.log 2>&1 &
FRONTEND_PID=$!
echo -e "${GREEN}✅ AdminDashboard Frontend started (PID: $FRONTEND_PID)${NC}"
echo "   Log: tail -f /tmp/admindashboard-frontend.log"
echo ""

echo "⏳ Waiting for frontend to initialize..."
sleep 10

echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}🎉 Both services are running!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo "📍 URLs:"
echo "   API:      http://localhost:5200"
echo "   Swagger:  http://localhost:5200"
echo "   Frontend: http://localhost:3002"
echo ""
echo "📋 Process IDs:"
echo "   API PID:      $API_PID"
echo "   Frontend PID: $FRONTEND_PID"
echo ""
echo "📝 Logs:"
echo "   API:      tail -f /tmp/admindashboard-api.log"
echo "   Frontend: tail -f /tmp/admindashboard-frontend.log"
echo ""
echo "🛑 To stop both services:"
echo "   kill $API_PID $FRONTEND_PID"
echo "   or run: ./STOP_BOTH.sh"
echo ""

# Save PIDs for stop script
echo "$API_PID" > /tmp/admindashboard-api.pid
echo "$FRONTEND_PID" > /tmp/admindashboard-frontend.pid

echo "Press Ctrl+C to view logs, or just browse to http://localhost:3002"
echo ""

# Follow logs
trap "echo ''; echo 'Logs stopped. Services still running in background.'; exit 0" INT
tail -f /tmp/admindashboard-api.log -f /tmp/admindashboard-frontend.log
