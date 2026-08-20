#!/bin/bash

echo "🧪 Testing AdminDashboard-API"
echo "=============================="
echo ""

# Test 1: Health Check
echo "1️⃣ Testing Health Endpoint..."
HEALTH=$(curl -s http://localhost:5200/api/health)
if [[ $HEALTH == *"healthy"* ]]; then
    echo "   ✅ Health check PASSED"
    echo "   Response: $HEALTH"
else
    echo "   ❌ Health check FAILED"
    exit 1
fi
echo ""

# Test 2: Swagger UI
echo "2️⃣ Testing Swagger UI..."
SWAGGER=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5200/index.html)
if [[ $SWAGGER == "200" ]]; then
    echo "   ✅ Swagger UI accessible"
else
    echo "   ⚠️  Swagger returned: $SWAGGER"
fi
echo ""

# Test 3: CORS Headers
echo "3️⃣ Testing CORS Configuration..."
CORS=$(curl -s -I http://localhost:5200/api/health | grep -i "access-control")
if [[ ! -z "$CORS" ]]; then
    echo "   ✅ CORS headers present"
    echo "   $CORS"
else
    echo "   ⚠️  No CORS headers found"
fi
echo ""

# Test 4: Create Event (will fail without Cosmos DB, but tests endpoint)
echo "4️⃣ Testing Events Endpoint..."
EVENT_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST http://localhost:5200/api/events \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Event",
    "description": "Testing API",
    "startDate": "2026-09-01T10:00:00Z",
    "endDate": "2026-09-01T18:00:00Z",
    "type": 0,
    "isBookable": true
  }')

HTTP_CODE=$(echo "$EVENT_RESPONSE" | tail -1)
BODY=$(echo "$EVENT_RESPONSE" | head -n -1)

if [[ $HTTP_CODE == "500" ]]; then
    echo "   ⚠️  POST /api/events returned 500 (expected without Cosmos DB)"
    echo "   This means the endpoint exists but needs database connection"
elif [[ $HTTP_CODE == "201" ]]; then
    echo "   ✅ Event created successfully!"
    echo "   Response: $BODY"
else
    echo "   📋 Status: $HTTP_CODE"
fi
echo ""

echo "=============================="
echo "✅ API is running and responding"
echo ""
echo "📍 Access points:"
echo "   Health:  http://localhost:5200/api/health"
echo "   Swagger: http://localhost:5200"
echo "   Events:  http://localhost:5200/api/events"
echo ""
echo "⚠️  Note: Event creation will fail until Cosmos DB is configured"
echo "   To set up Cosmos DB, see SETUP_GUIDE.md"
