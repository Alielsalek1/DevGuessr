#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo "--------------------------------------------------"
echo "Starting Smoke Tests against Nginx Gateway"
echo "Target: https://localhost"
echo "--------------------------------------------------"

# 1. Verify SSL and Connectivity
echo -n "[Test 1] SSL Connectivity to Gateway... "
# -k for insecure (self-signed cert), -s for silent, -o to discard body, -w for status code
STATUS_CODE=$(curl -k -s -o /dev/null -w "%{http_code}" https://localhost/api/v1/internal-auth/login)

# We expect a 405 (Method Not Allowed) because we are doing a GET on a POST endpoint, 
# or a 400/415 depending on content type. Just checking if we get a response from the API.
if [[ "$STATUS_CODE" -ge 200 && "$STATUS_CODE" -lt 500 ]]; then
    echo -e "${GREEN}PASS${NC} (Status: $STATUS_CODE)"
else
    echo -e "${RED}FAIL${NC} (Status: $STATUS_CODE)"
    echo "Hint: Ensure Nginx and the API container are running (docker-compose up)"
fi

# 2. Verify Infrastructure Hardening (Port check)
echo -n "[Test 2] Port Hardening (Checking DB Port 5432)... "
# Try to connect to postgres port on localhost. It should fail if ports are successfully hidden.
if ! nc -z -w 2 localhost 5432 2>/dev/null; then
    echo -e "${GREEN}PASS${NC} (Port 5432 is closed to host - SECURE)"
else
    echo -e "${RED}FAIL${NC} (Port 5432 is exposed! Check docker-compose.yml)${NC}"
fi

echo -n "[Test 3] Port Hardening (Checking Redis Port 6379)... "
if ! nc -z -w 2 localhost 6379 2>/dev/null; then
    echo -e "${GREEN}PASS${NC} (Port 6379 is closed to host - SECURE)"
else
    echo -e "${RED}FAIL${NC} (Port 6379 is exposed! Check docker-compose.yml)${NC}"
fi

# 3. Verify Nginx Caching Header
echo -n "[Test 4] Nginx Caching Logic... "
# We perform a GET request and check for the X-Cache-Status header we added in nginx.conf
CACHE_HEADER=$(curl -k -I -s https://localhost/ | grep -i "X-Cache-Status")
if [[ -n "$CACHE_HEADER" ]]; then
    echo -e "${GREEN}PASS${NC} (Found: $CACHE_HEADER)"
else
    echo -e "${RED}FAIL${NC} (Cache header X-Cache-Status not found)"
fi

echo "--------------------------------------------------"
echo "Smoke tests complete."
echo "--------------------------------------------------"
