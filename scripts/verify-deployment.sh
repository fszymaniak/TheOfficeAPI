#!/bin/bash

# TheOfficeAPI - Mikrus VPS Deployment Verification Script
# This script verifies that the application is properly deployed and running

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
APP_PORT="${APP_PORT:-8080}"
APP_HOST="${APP_HOST:-localhost}"
CONTAINER_NAME="${CONTAINER_NAME:-theofficeapi}"

echo "======================================"
echo "TheOfficeAPI Deployment Verification"
echo "======================================"
echo ""

# Function to print success message
success() {
    echo -e "${GREEN}✓${NC} $1"
}

# Function to print error message
error() {
    echo -e "${RED}✗${NC} $1"
}

# Function to print warning message
warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

# Function to print info message
info() {
    echo -e "${YELLOW}ℹ${NC} $1"
}

# Check if Docker is installed
check_docker() {
    echo "Checking Docker installation..."
    if command -v docker &> /dev/null; then
        DOCKER_VERSION=$(docker --version)
        success "Docker is installed: $DOCKER_VERSION"
        return 0
    else
        error "Docker is not installed"
        return 1
    fi
}

# Check if Docker Compose is installed
check_docker_compose() {
    echo "Checking Docker Compose installation..."
    if docker compose version &> /dev/null; then
        COMPOSE_VERSION=$(docker compose version)
        success "Docker Compose is installed: $COMPOSE_VERSION"
        return 0
    else
        warning "Docker Compose (plugin) is not installed"
        return 1
    fi
}

# Check if container is running
check_container() {
    echo ""
    echo "Checking if container is running..."

    # Try docker-compose first
    if docker compose ps 2>/dev/null | grep -q "theofficeapi"; then
        success "Container is running (via docker-compose)"
        docker compose ps
        return 0
    fi

    # Try direct docker command
    if docker ps | grep -q "$CONTAINER_NAME"; then
        success "Container is running (via docker)"
        docker ps --filter "name=$CONTAINER_NAME" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
        return 0
    else
        error "Container is not running"
        info "Checking all containers:"
        docker ps -a
        return 1
    fi
}

# Check container logs for errors
check_container_logs() {
    echo ""
    echo "Checking container logs for errors..."

    # Try to get logs
    if docker logs --tail 50 "$CONTAINER_NAME" 2>/dev/null | grep -i "error\|exception\|fail" > /tmp/errors.log; then
        warning "Found potential errors in logs:"
        cat /tmp/errors.log
        rm /tmp/errors.log
    else
        success "No obvious errors in recent logs"
    fi

    echo ""
    info "Last 10 lines of logs:"
    docker logs --tail 10 "$CONTAINER_NAME" 2>&1 || docker compose logs --tail 10 theofficeapi-level0 2>&1
}

# Check if application is responding
check_application_health() {
    echo ""
    echo "Checking application health..."

    # Wait a moment for the app to be ready
    sleep 2

    # Check health endpoint
    if curl -f -s -m 5 "http://${APP_HOST}:${APP_PORT}/health" > /dev/null; then
        success "Health endpoint is responding"
        HEALTH_RESPONSE=$(curl -s "http://${APP_HOST}:${APP_PORT}/health")
        echo "Response: $HEALTH_RESPONSE"
        return 0
    else
        error "Health endpoint is not responding"
        return 1
    fi
}

# Test API Level 0 endpoint
check_api_level0() {
    echo ""
    echo "Testing API Level 0 endpoint..."

    RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "http://${APP_HOST}:${APP_PORT}/api/theOffice" \
        -H "Content-Type: application/json" \
        -d '{"action":"getAllSeasons"}' 2>&1)

    HTTP_CODE=$(echo "$RESPONSE" | tail -n1)
    BODY=$(echo "$RESPONSE" | head -n-1)

    if [ "$HTTP_CODE" = "200" ]; then
        success "API Level 0 is working (HTTP $HTTP_CODE)"
        echo "Response preview (first 200 chars):"
        echo "$BODY" | head -c 200
        echo "..."
        return 0
    else
        error "API Level 0 failed (HTTP $HTTP_CODE)"
        echo "Response: $BODY"
        return 1
    fi
}

# Check Swagger UI
check_swagger() {
    echo ""
    echo "Checking Swagger UI..."

    if curl -f -s -m 5 "http://${APP_HOST}:${APP_PORT}/swagger" > /dev/null; then
        success "Swagger UI is accessible at http://${APP_HOST}:${APP_PORT}/swagger"
        return 0
    else
        warning "Swagger UI may not be accessible (check if enabled in production)"
        return 1
    fi
}

# Check port accessibility
check_port() {
    echo ""
    echo "Checking if port $APP_PORT is accessible..."

    if command -v netstat &> /dev/null; then
        if netstat -tuln | grep -q ":$APP_PORT "; then
            success "Port $APP_PORT is listening"
            netstat -tuln | grep ":$APP_PORT "
            return 0
        else
            error "Port $APP_PORT is not listening"
            return 1
        fi
    elif command -v ss &> /dev/null; then
        if ss -tuln | grep -q ":$APP_PORT "; then
            success "Port $APP_PORT is listening"
            ss -tuln | grep ":$APP_PORT "
            return 0
        else
            error "Port $APP_PORT is not listening"
            return 1
        fi
    else
        warning "Neither netstat nor ss is available, skipping port check"
        return 1
    fi
}

# Check system resources
check_resources() {
    echo ""
    echo "Checking system resources..."

    # Memory
    if command -v free &> /dev/null; then
        echo ""
        echo "Memory usage:"
        free -h
    fi

    # Docker stats
    echo ""
    echo "Docker container resources (5 second snapshot):"
    timeout 5 docker stats --no-stream "$CONTAINER_NAME" 2>/dev/null || \
    timeout 5 docker stats --no-stream 2>/dev/null | grep -i "theoffice" || \
    warning "Could not get container stats"
}

# Check Nginx (if installed)
check_nginx() {
    echo ""
    echo "Checking Nginx configuration..."

    if command -v nginx &> /dev/null; then
        success "Nginx is installed"

        if systemctl is-active --quiet nginx; then
            success "Nginx is running"
        else
            warning "Nginx is installed but not running"
        fi

        # Test configuration
        if sudo nginx -t &> /dev/null; then
            success "Nginx configuration is valid"
        else
            error "Nginx configuration has errors"
            sudo nginx -t
        fi
    else
        info "Nginx is not installed (optional)"
    fi
}

# Check SSL certificate (if Let's Encrypt is used)
check_ssl() {
    echo ""
    echo "Checking SSL certificate..."

    if command -v certbot &> /dev/null; then
        success "Certbot is installed"

        CERTS=$(sudo certbot certificates 2>/dev/null)
        if [ -n "$CERTS" ]; then
            success "SSL certificates found:"
            echo "$CERTS"
        else
            info "No SSL certificates found"
        fi
    else
        info "Certbot is not installed (optional for SSL)"
    fi
}

# Check firewall
check_firewall() {
    echo ""
    echo "Checking firewall configuration..."

    if command -v ufw &> /dev/null; then
        UFW_STATUS=$(sudo ufw status 2>/dev/null)
        if echo "$UFW_STATUS" | grep -q "Status: active"; then
            success "UFW firewall is active"
            echo "$UFW_STATUS" | grep -E "80|443|8080"
        else
            warning "UFW firewall is not active"
        fi
    else
        info "UFW is not installed"
    fi
}

# Generate summary report
generate_summary() {
    echo ""
    echo "======================================"
    echo "Deployment Verification Summary"
    echo "======================================"
    echo ""

    TOTAL_CHECKS=0
    PASSED_CHECKS=0

    echo "Core Services:"
    [ $DOCKER_INSTALLED -eq 0 ] && success "Docker" && ((PASSED_CHECKS++)) || error "Docker"
    ((TOTAL_CHECKS++))

    [ $CONTAINER_RUNNING -eq 0 ] && success "Container" && ((PASSED_CHECKS++)) || error "Container"
    ((TOTAL_CHECKS++))

    [ $APP_HEALTH -eq 0 ] && success "Health Check" && ((PASSED_CHECKS++)) || error "Health Check"
    ((TOTAL_CHECKS++))

    [ $API_WORKING -eq 0 ] && success "API Endpoint" && ((PASSED_CHECKS++)) || error "API Endpoint"
    ((TOTAL_CHECKS++))

    echo ""
    echo "Score: $PASSED_CHECKS/$TOTAL_CHECKS checks passed"
    echo ""

    if [ $PASSED_CHECKS -eq $TOTAL_CHECKS ]; then
        success "All core checks passed! Deployment is healthy."
        echo ""
        info "Access your API at:"
        echo "  - Swagger UI: http://${APP_HOST}:${APP_PORT}/swagger"
        echo "  - Health Check: http://${APP_HOST}:${APP_PORT}/health"
        echo "  - API Endpoint: http://${APP_HOST}:${APP_PORT}/api/theOffice"
        return 0
    else
        error "Some checks failed. Please review the errors above."
        return 1
    fi
}

# Main execution
main() {
    DOCKER_INSTALLED=1
    CONTAINER_RUNNING=1
    APP_HEALTH=1
    API_WORKING=1

    check_docker && DOCKER_INSTALLED=0 || DOCKER_INSTALLED=1
    check_docker_compose

    if [ $DOCKER_INSTALLED -eq 0 ]; then
        check_container && CONTAINER_RUNNING=0 || CONTAINER_RUNNING=1

        if [ $CONTAINER_RUNNING -eq 0 ]; then
            check_container_logs
            check_port
            check_application_health && APP_HEALTH=0 || APP_HEALTH=1
            check_api_level0 && API_WORKING=0 || API_WORKING=1
            check_swagger
            check_resources
        fi
    fi

    check_nginx
    check_ssl
    check_firewall

    generate_summary

    exit $?
}

# Run main function
main
