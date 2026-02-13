#!/bin/bash

# TheOfficeAPI - Mikrus VPS Quick Deployment Script
# This script automates the deployment process on a Mikrus VPS

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
REPO_URL="${REPO_URL:-https://github.com/fszymaniak/TheOfficeAPI.git}"
REPO_DIR="${REPO_DIR:-$HOME/TheOfficeAPI}"
BRANCH="${BRANCH:-main}"
APP_PORT="${APP_PORT:-8080}"
MATURITY_LEVEL="${MATURITY_LEVEL:-Level0}"

# Function to print colored output
print_header() {
    echo ""
    echo -e "${BLUE}======================================"
    echo -e "$1"
    echo -e "======================================${NC}"
    echo ""
}

print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}

print_info() {
    echo -e "${YELLOW}ℹ${NC} $1"
}

# Function to check if command exists
command_exists() {
    command -v "$1" &> /dev/null
}

# Function to install Docker
install_docker() {
    print_header "Installing Docker"

    if command_exists docker; then
        print_success "Docker is already installed"
        docker --version
        return 0
    fi

    print_info "Installing Docker..."

    # Update package index
    sudo apt-get update

    # Install prerequisites
    sudo apt-get install -y \
        apt-transport-https \
        ca-certificates \
        curl \
        gnupg \
        lsb-release \
        software-properties-common

    # Add Docker's official GPG key
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg

    # Set up the stable repository
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

    # Install Docker Engine
    sudo apt-get update
    sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin

    # Add current user to docker group
    sudo usermod -aG docker $USER

    print_success "Docker installed successfully"
    docker --version

    print_info "You may need to log out and back in for docker group changes to take effect"
}

# Function to install Git
install_git() {
    print_header "Checking Git Installation"

    if command_exists git; then
        print_success "Git is already installed"
        git --version
        return 0
    fi

    print_info "Installing Git..."
    sudo apt-get update
    sudo apt-get install -y git

    print_success "Git installed successfully"
    git --version
}

# Function to clone or update repository
setup_repository() {
    print_header "Setting Up Repository"

    if [ -d "$REPO_DIR" ]; then
        print_info "Repository directory already exists. Updating..."
        cd "$REPO_DIR"
        git fetch origin
        git checkout "$BRANCH"
        git pull origin "$BRANCH"
        print_success "Repository updated"
    else
        print_info "Cloning repository..."
        git clone "$REPO_URL" "$REPO_DIR"
        cd "$REPO_DIR"
        git checkout "$BRANCH"
        print_success "Repository cloned"
    fi

    print_info "Current directory: $(pwd)"
    print_info "Current branch: $(git branch --show-current)"
}

# Function to create environment file
create_env_file() {
    print_header "Creating Environment Configuration"

    cd "$REPO_DIR"

    cat > .env << EOF
ASPNETCORE_ENVIRONMENT=Production
MATURITY_LEVEL=$MATURITY_LEVEL
PORT=$APP_PORT
ASPNETCORE_URLS=http://+:$APP_PORT
EOF

    print_success "Environment file created"
    cat .env
}

# Function to stop existing containers
stop_existing_containers() {
    print_header "Stopping Existing Containers"

    cd "$REPO_DIR"

    if docker compose ps -q theofficeapi-level0 &> /dev/null; then
        print_info "Stopping containers via docker-compose..."
        docker compose down
        print_success "Containers stopped"
    elif docker ps -a | grep -q theofficeapi; then
        print_info "Stopping containers via docker..."
        docker stop theofficeapi 2>/dev/null || true
        docker rm theofficeapi 2>/dev/null || true
        print_success "Containers stopped"
    else
        print_info "No existing containers to stop"
    fi
}

# Function to build and start containers
deploy_application() {
    print_header "Building and Deploying Application"

    cd "$REPO_DIR"

    print_info "Building Docker image..."
    docker compose build --no-cache

    print_info "Starting containers..."
    docker compose up -d

    print_success "Application deployed successfully"
}

# Function to show deployment info
show_deployment_info() {
    print_header "Deployment Information"

    echo "Container Status:"
    docker compose ps

    echo ""
    echo "Container Logs (last 20 lines):"
    docker compose logs --tail 20 theofficeapi-level0

    echo ""
    print_success "Deployment completed!"
    echo ""
    print_info "Access your application:"
    echo "  - Health Check: http://localhost:$APP_PORT/health"
    echo "  - Swagger UI: http://localhost:$APP_PORT/swagger"
    echo "  - API Endpoint: http://localhost:$APP_PORT/api/theOffice"
    echo ""
    print_info "Useful commands:"
    echo "  - View logs: docker compose logs -f theofficeapi-level0"
    echo "  - Stop app: docker compose down"
    echo "  - Restart app: docker compose restart"
    echo "  - Verify deployment: ./scripts/verify-deployment.sh"
}

# Function to setup Nginx (optional)
setup_nginx() {
    print_header "Nginx Setup (Optional)"

    read -p "Do you want to install and configure Nginx as reverse proxy? (y/n): " -n 1 -r
    echo

    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_info "Skipping Nginx setup"
        return 0
    fi

    if ! command_exists nginx; then
        print_info "Installing Nginx..."
        sudo apt-get update
        sudo apt-get install -y nginx
    fi

    print_success "Nginx is installed"

    read -p "Enter your domain name (or press Enter to skip): " DOMAIN_NAME

    if [ -z "$DOMAIN_NAME" ]; then
        print_info "Skipping Nginx configuration"
        return 0
    fi

    print_info "Creating Nginx configuration for $DOMAIN_NAME..."

    sudo tee /etc/nginx/sites-available/theofficeapi > /dev/null << EOF
server {
    listen 80;
    server_name $DOMAIN_NAME www.$DOMAIN_NAME;

    location / {
        proxy_pass http://localhost:$APP_PORT;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_set_header X-Real-IP \$remote_addr;

        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    location /health {
        proxy_pass http://localhost:$APP_PORT/health;
        access_log off;
    }
}
EOF

    # Enable site
    sudo ln -sf /etc/nginx/sites-available/theofficeapi /etc/nginx/sites-enabled/

    # Test configuration
    if sudo nginx -t; then
        print_success "Nginx configuration is valid"
        sudo systemctl restart nginx
        sudo systemctl enable nginx
        print_success "Nginx configured and restarted"

        echo ""
        print_info "Your application is now accessible at: http://$DOMAIN_NAME"
        echo ""
        print_info "To enable SSL/HTTPS, run:"
        echo "  sudo apt-get install -y certbot python3-certbot-nginx"
        echo "  sudo certbot --nginx -d $DOMAIN_NAME -d www.$DOMAIN_NAME"
    else
        print_error "Nginx configuration has errors"
        sudo nginx -t
    fi
}

# Function to setup firewall
setup_firewall() {
    print_header "Firewall Setup (Optional)"

    read -p "Do you want to configure UFW firewall? (y/n): " -n 1 -r
    echo

    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_info "Skipping firewall setup"
        return 0
    fi

    if ! command_exists ufw; then
        print_info "Installing UFW..."
        sudo apt-get update
        sudo apt-get install -y ufw
    fi

    print_info "Configuring firewall rules..."
    sudo ufw allow 22/tcp comment 'SSH'
    sudo ufw allow 80/tcp comment 'HTTP'
    sudo ufw allow 443/tcp comment 'HTTPS'

    print_info "Enabling firewall..."
    sudo ufw --force enable

    print_success "Firewall configured"
    sudo ufw status
}

# Main execution
main() {
    print_header "TheOfficeAPI - Mikrus VPS Deployment"

    print_info "Starting deployment with the following configuration:"
    echo "  Repository: $REPO_URL"
    echo "  Branch: $BRANCH"
    echo "  Directory: $REPO_DIR"
    echo "  Port: $APP_PORT"
    echo "  Maturity Level: $MATURITY_LEVEL"
    echo ""

    # Check if running as root
    if [ "$EUID" -eq 0 ]; then
        print_error "Please do not run this script as root. Run as a regular user with sudo privileges."
        exit 1
    fi

    # Install dependencies
    install_git
    install_docker

    # Setup and deploy
    setup_repository
    create_env_file
    stop_existing_containers
    deploy_application
    show_deployment_info

    # Optional components
    setup_nginx
    setup_firewall

    print_header "Deployment Complete!"

    echo ""
    print_success "Your TheOfficeAPI is now running!"
    echo ""
    print_info "Next steps:"
    echo "  1. Verify deployment: ./scripts/verify-deployment.sh"
    echo "  2. Check logs: docker compose logs -f theofficeapi-level0"
    echo "  3. Access Swagger UI: http://your-server-ip:$APP_PORT/swagger"
    echo ""
}

# Run main function
main
