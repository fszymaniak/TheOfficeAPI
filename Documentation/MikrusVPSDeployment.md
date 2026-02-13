# Mikrus VPS Deployment Guide for TheOfficeAPI

This guide explains how to deploy TheOfficeAPI to a Mikrus VPS (mikrus.us) server.

## Which Mikrus VPS to Choose

### Recommended VPS Plans

For TheOfficeAPI (.NET 9.0 application with Docker), here are the recommended plans:

#### **Minimum Requirements (Budget Option)**
- **Mikrus VPS S** or **VPS M**
  - RAM: 512MB - 1GB
  - CPU: 1 core
  - Disk: 10GB SSD
  - Cost: ~5-10 PLN/month
  - **Best for**: Testing, development, low-traffic demos

#### **Recommended for Production (Best Value)**
- **Mikrus VPS L** or **VPS XL**
  - RAM: 2-4GB
  - CPU: 2+ cores
  - Disk: 20-40GB SSD
  - Cost: ~15-25 PLN/month
  - **Best for**: Production deployment, medium traffic
  - **Why**: .NET applications can be memory-intensive, especially with Docker overhead

#### **High Performance Option**
- **Mikrus VPS XXL** or higher
  - RAM: 8GB+
  - CPU: 4+ cores
  - Disk: 80GB+ SSD
  - Cost: ~40+ PLN/month
  - **Best for**: High-traffic production, multiple services

### What to Look For

When selecting a Mikrus VPS, ensure it has:
- ✅ **Docker support** (most Mikrus VPS plans support Docker)
- ✅ **Public IPv4 address** (for external access)
- ✅ **SSH access** (for deployment)
- ✅ **Linux OS** (Ubuntu 22.04 or Debian 11+ recommended)
- ✅ **At least 1GB RAM** (for .NET + Docker)

## Prerequisites

Before deploying, ensure you have:
- Mikrus VPS account and active server
- SSH access to your VPS
- Domain name (optional, but recommended)
- Basic Linux command-line knowledge

## Deployment Steps

### 1. Connect to Your Mikrus VPS

```bash
# Replace with your Mikrus VPS IP and username
ssh user@your-vps-ip

# Or if you have a specific port
ssh -p 2222 user@your-vps-ip
```

### 2. Install Docker and Docker Compose

```bash
# Update system packages
sudo apt update && sudo apt upgrade -y

# Install Docker
sudo apt install -y apt-transport-https ca-certificates curl software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io

# Install Docker Compose
sudo apt install -y docker-compose-plugin

# Verify installation
docker --version
docker compose version

# Add your user to docker group (to run docker without sudo)
sudo usermod -aG docker $USER
newgrp docker
```

### 3. Install Git

```bash
sudo apt install -y git
git --version
```

### 4. Clone the Repository

```bash
# Clone the project
cd ~
git clone https://github.com/fszymaniak/TheOfficeAPI.git
cd TheOfficeAPI

# Or if you need a specific branch
git checkout main
```

### 5. Configure the Application

Create a production environment configuration:

```bash
# Create a .env file for production settings
cat > .env << 'EOF'
ASPNETCORE_ENVIRONMENT=Production
MATURITY_LEVEL=Level0
PORT=8080
ASPNETCORE_URLS=http://+:8080
EOF
```

### 6. Build and Deploy with Docker

#### Option A: Using Docker Compose (Recommended)

```bash
# Build and start the container
docker compose up -d --build

# View logs
docker compose logs -f theofficeapi-level0

# Check container status
docker compose ps
```

#### Option B: Using Docker Directly

```bash
# Build the Docker image
docker build -t theofficeapi:latest .

# Run the container
docker run -d \
  --name theofficeapi \
  --restart unless-stopped \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e MATURITY_LEVEL=Level0 \
  theofficeapi:latest

# View logs
docker logs -f theofficeapi

# Check container status
docker ps
```

### 7. Configure Nginx Reverse Proxy (Recommended)

For production deployment, use Nginx as a reverse proxy:

```bash
# Install Nginx
sudo apt install -y nginx

# Create Nginx configuration
sudo nano /etc/nginx/sites-available/theofficeapi
```

Add the following configuration:

```nginx
server {
    listen 80;
    server_name your-domain.com www.your-domain.com;  # Replace with your domain

    location / {
        proxy_pass http://localhost:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Real-IP $remote_addr;

        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # Health check endpoint
    location /health {
        proxy_pass http://localhost:8080/health;
        access_log off;
    }
}
```

Enable the site and restart Nginx:

```bash
# Enable the site
sudo ln -s /etc/nginx/sites-available/theofficeapi /etc/nginx/sites-enabled/

# Test Nginx configuration
sudo nginx -t

# Restart Nginx
sudo systemctl restart nginx
sudo systemctl enable nginx
```

### 8. Configure SSL with Let's Encrypt (Optional but Recommended)

```bash
# Install Certbot
sudo apt install -y certbot python3-certbot-nginx

# Obtain SSL certificate
sudo certbot --nginx -d your-domain.com -d www.your-domain.com

# Test automatic renewal
sudo certbot renew --dry-run
```

### 9. Configure Firewall

```bash
# Allow SSH, HTTP, and HTTPS
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Enable firewall
sudo ufw enable
sudo ufw status
```

## Verification

### 1. Check if the Application is Running

```bash
# Check Docker container status
docker ps

# View application logs
docker logs theofficeapi

# Or with docker-compose
docker compose logs theofficeapi-level0
```

### 2. Test the API Endpoints

```bash
# Test health endpoint
curl http://localhost:8080/health

# Test API Level 0 endpoint
curl -X POST http://localhost:8080/api/theOffice \
  -H "Content-Type: application/json" \
  -d '{"action":"getAllSeasons"}'

# Test from external access (replace with your domain or IP)
curl http://your-domain.com/health
```

### 3. Access Swagger UI

Open your browser and navigate to:
- Local: `http://your-vps-ip:8080/swagger`
- With domain: `http://your-domain.com/swagger`
- With SSL: `https://your-domain.com/swagger`

### 4. Monitor Application

```bash
# Check CPU and memory usage
docker stats

# View real-time logs
docker logs -f theofficeapi

# Check Nginx access logs
sudo tail -f /var/log/nginx/access.log
```

## Maintenance

### Updating the Application

```bash
# Pull latest changes
cd ~/TheOfficeAPI
git pull origin main

# Rebuild and restart containers
docker compose down
docker compose up -d --build

# Or with Docker directly
docker stop theofficeapi
docker rm theofficeapi
docker build -t theofficeapi:latest .
docker run -d \
  --name theofficeapi \
  --restart unless-stopped \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e MATURITY_LEVEL=Level0 \
  theofficeapi:latest
```

### Backup and Restore

```bash
# Export Docker image
docker save theofficeapi:latest | gzip > theofficeapi-backup.tar.gz

# Import Docker image
docker load < theofficeapi-backup.tar.gz
```

### View Logs

```bash
# Docker logs
docker logs theofficeapi --tail 100 -f

# Nginx logs
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log
```

## Troubleshooting

### Container Won't Start

```bash
# Check Docker logs
docker logs theofficeapi

# Check if port is already in use
sudo netstat -tulpn | grep 8080

# Remove and recreate container
docker rm -f theofficeapi
docker compose up -d --force-recreate
```

### High Memory Usage

```bash
# Check memory usage
free -h
docker stats

# Set memory limits in docker-compose.yaml
# Add under service configuration:
# mem_limit: 512m
# mem_reservation: 256m
```

### Connection Issues

```bash
# Check firewall
sudo ufw status

# Check if application is listening
sudo netstat -tulpn | grep 8080

# Check Nginx configuration
sudo nginx -t
sudo systemctl status nginx
```

## Performance Optimization

### 1. Enable Docker Logging Limits

Add to `docker-compose.yaml`:

```yaml
services:
  theofficeapi-level0:
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"
```

### 2. Configure Nginx Caching

Add to Nginx configuration:

```nginx
# Add inside http block in /etc/nginx/nginx.conf
proxy_cache_path /var/cache/nginx levels=1:2 keys_zone=api_cache:10m max_size=100m inactive=60m;

# Add inside location block
proxy_cache api_cache;
proxy_cache_valid 200 5m;
proxy_cache_bypass $http_cache_control;
add_header X-Cache-Status $upstream_cache_status;
```

### 3. Enable Compression

Nginx already has compression enabled by default for common content types.

## Security Best Practices

1. **Keep system updated**:
   ```bash
   sudo apt update && sudo apt upgrade -y
   ```

2. **Use strong SSH keys** (disable password authentication)

3. **Enable automatic security updates**:
   ```bash
   sudo apt install -y unattended-upgrades
   sudo dpkg-reconfigure -plow unattended-upgrades
   ```

4. **Monitor application logs** regularly

5. **Use environment variables** for sensitive configuration (never commit secrets)

6. **Enable SSL/TLS** with Let's Encrypt

7. **Set up fail2ban** to prevent brute force attacks:
   ```bash
   sudo apt install -y fail2ban
   sudo systemctl enable fail2ban
   sudo systemctl start fail2ban
   ```

## Cost Estimation

Based on Mikrus VPS pricing (approximate):

| Plan | RAM | CPU | Disk | Monthly Cost | Suitable For |
|------|-----|-----|------|--------------|--------------|
| VPS S | 512MB | 1 core | 10GB | ~5 PLN | Testing only |
| VPS M | 1GB | 1 core | 15GB | ~10 PLN | Light traffic |
| VPS L | 2GB | 2 cores | 20GB | ~15 PLN | **Recommended** |
| VPS XL | 4GB | 2 cores | 40GB | ~25 PLN | Production |
| VPS XXL | 8GB | 4 cores | 80GB | ~40 PLN | High traffic |

*Note: Prices are approximate and may vary. Check mikrus.us for current pricing.*

## Support and Resources

- **Mikrus Support**: https://mikrus.us/
- **TheOfficeAPI GitHub**: https://github.com/fszymaniak/TheOfficeAPI
- **Docker Documentation**: https://docs.docker.com/
- **Nginx Documentation**: https://nginx.org/en/docs/

## Next Steps

1. Choose appropriate VPS plan based on expected traffic
2. Set up domain name (optional)
3. Configure monitoring and alerting
4. Set up automated backups
5. Implement CI/CD pipeline for automatic deployments

For additional deployment options, see:
- [Railway Deployment](RailwayDeployment.md)
- [Docker Setup](DockerSetup.md)
