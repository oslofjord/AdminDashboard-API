#!/bin/bash

echo "🛑 Stopping AdminDashboard Ecosystem"
echo "===================================="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Stop API
if [ -f /tmp/admindashboard-api.pid ]; then
    API_PID=$(cat /tmp/admindashboard-api.pid)
    if ps -p $API_PID > /dev/null 2>&1; then
        echo "🛑 Stopping AdminDashboard-API (PID: $API_PID)..."
        kill $API_PID
        echo -e "${GREEN}✅ API stopped${NC}"
    else
        echo "ℹ️  API was not running"
    fi
    rm /tmp/admindashboard-api.pid
else
    echo "ℹ️  No API PID file found"
fi

# Stop Frontend
if [ -f /tmp/admindashboard-frontend.pid ]; then
    FRONTEND_PID=$(cat /tmp/admindashboard-frontend.pid)
    if ps -p $FRONTEND_PID > /dev/null 2>&1; then
        echo "🛑 Stopping AdminDashboard Frontend (PID: $FRONTEND_PID)..."
        kill $FRONTEND_PID
        echo -e "${GREEN}✅ Frontend stopped${NC}"
    else
        echo "ℹ️  Frontend was not running"
    fi
    rm /tmp/admindashboard-frontend.pid
else
    echo "ℹ️  No Frontend PID file found"
fi

echo ""
echo -e "${GREEN}✅ All services stopped${NC}"
