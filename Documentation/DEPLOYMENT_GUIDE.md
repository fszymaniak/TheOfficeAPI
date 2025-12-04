# TheOfficeAPI - Przewodnik Deploymentu

Ten dokument zawiera instrukcje deploymentu aplikacji TheOfficeAPI na:
1. **Raspberry Pi 5** (z Docker lub natywnie)
2. **Mikr.us** (polski VPS)

## Wymagania aplikacji

- **.NET 9.0 Runtime** lub Docker
- **Brak bazy danych** - aplikacja przechowuje dane w pamięci
- **Minimalne wymagania**: 256MB RAM (512MB+ zalecane)
- **Port**: 8080 (domyślny)

---

# Część 1: Deployment na Raspberry Pi 5

## Opcja A: Z Docker (Zalecane)

### 1.1. Przygotowanie Raspberry Pi

```bash
# Aktualizacja systemu
sudo apt update && sudo apt upgrade -y

# Instalacja Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Dodanie użytkownika do grupy docker (żeby nie używać sudo)
sudo usermod -aG docker $USER

# Wyloguj się i zaloguj ponownie, lub:
newgrp docker

# Sprawdź instalację
docker --version
docker compose version
```

### 1.2. Klonowanie repozytorium

```bash
# Utwórz katalog na projekty
mkdir -p ~/projects && cd ~/projects

# Klonuj repozytorium
git clone https://github.com/fszymaniak/TheOfficeAPI.git
cd TheOfficeAPI
```

### 1.3. Build i uruchomienie z Docker

#### Opcja 1: Docker Compose (najprostsza)

```bash
# Build i uruchomienie
docker compose up -d --build

# Sprawdź logi
docker compose logs -f

# Sprawdź status
docker compose ps
```

#### Opcja 2: Ręczny Docker build

```bash
# Build obrazu (dla ARM64 - Raspberry Pi 5)
docker build -t theoffice-api:latest .

# Uruchomienie
docker run -d \
  --name theoffice-api \
  -p 5000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e MATURITY_LEVEL=Level0 \
  --restart unless-stopped \
  theoffice-api:latest

# Sprawdź czy działa
docker logs theoffice-api
curl http://localhost:5000/health
```

### 1.4. Konfiguracja autostartu

Docker z flagą `--restart unless-stopped` automatycznie uruchomi kontener po restarcie RPi.

Alternatywnie, dla docker-compose:

```bash
# Utwórz plik systemd service
sudo nano /etc/systemd/system/theoffice-api.service
```

Zawartość:
```ini
[Unit]
Description=TheOfficeAPI Docker Compose
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/home/pi/projects/TheOfficeAPI
ExecStart=/usr/bin/docker compose up -d
ExecStop=/usr/bin/docker compose down
User=pi

[Install]
WantedBy=multi-user.target
```

```bash
# Włącz autostart
sudo systemctl enable theoffice-api
sudo systemctl start theoffice-api
```

---

## Opcja B: Bez Docker (Natywnie)

### 1.5. Instalacja .NET 9.0 na Raspberry Pi 5

```bash
# Metoda 1: Skrypt instalacyjny Microsoft (zalecane)
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 9.0 --runtime aspnetcore

# Dodaj do PATH (dodaj do ~/.bashrc)
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$DOTNET_ROOT' >> ~/.bashrc
source ~/.bashrc

# Sprawdź instalację
dotnet --info
```

```bash
# Metoda 2: Przez apt (jeśli dostępne)
# Dodaj repozytorium Microsoft
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt update
sudo apt install -y aspnetcore-runtime-9.0
```

### 1.6. Build i uruchomienie natywne

```bash
cd ~/projects/TheOfficeAPI

# Opcja 1: Uruchom bezpośrednio (development)
dotnet run --project src/TheOfficeAPI --configuration Release

# Opcja 2: Publish i uruchom (production - zalecane)
dotnet publish src/TheOfficeAPI/TheOfficeAPI.csproj \
  -c Release \
  -o ./publish \
  --self-contained false

# Uruchom
cd publish
ASPNETCORE_ENVIRONMENT=Production \
MATURITY_LEVEL=Level0 \
ASPNETCORE_URLS=http://0.0.0.0:5000 \
dotnet TheOfficeAPI.dll
```

### 1.7. Konfiguracja jako systemd service (dla natywnego uruchomienia)

```bash
sudo nano /etc/systemd/system/theoffice-api.service
```

Zawartość:
```ini
[Unit]
Description=TheOfficeAPI .NET Application
After=network.target

[Service]
Type=notify
User=pi
WorkingDirectory=/home/pi/projects/TheOfficeAPI/publish
ExecStart=/home/pi/.dotnet/dotnet TheOfficeAPI.dll
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
Environment=MATURITY_LEVEL=Level0
Environment=DOTNET_ROOT=/home/pi/.dotnet

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl daemon-reload
sudo systemctl enable theoffice-api
sudo systemctl start theoffice-api
sudo systemctl status theoffice-api
```

### 1.8. Dostęp z sieci lokalnej

```bash
# Znajdź IP Raspberry Pi
hostname -I

# Np. jeśli IP to 192.168.1.100:
# API: http://192.168.1.100:5000
# Swagger: http://192.168.1.100:5000/swagger
# Health: http://192.168.1.100:5000/health
```

### 1.9. (Opcjonalnie) Reverse Proxy z Nginx

```bash
sudo apt install nginx -y

sudo nano /etc/nginx/sites-available/theoffice-api
```

Zawartość:
```nginx
server {
    listen 80;
    server_name theoffice.local;  # lub twoja domena/IP

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
sudo ln -s /etc/nginx/sites-available/theoffice-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

---

# Część 2: Deployment na Mikr.us

## Informacje o Mikr.us

Mikr.us to polski provider tanich VPS-ów opartych na OpenVZ/LXC.

### Wybór planu

| Plan | RAM | Cena roczna | Rekomendacja |
|------|-----|-------------|--------------|
| **Mikrus 1.0** | 256MB | ~35 PLN | ❌ Za mało dla .NET |
| **Mikrus 2.0** | 512MB | ~59 PLN | ⚠️ Minimum, może być ciasno |
| **Mikrus 3.0** | 1GB | ~119 PLN | ✅ **Zalecany** |
| **Mikrus 4.0** | 2GB | ~179 PLN | ✅ Komfortowo |

**Rekomendacja**: **Mikrus 3.0** (1GB RAM) - .NET 9.0 Runtime potrzebuje ~200-300MB RAM, aplikacja ~50-100MB, więc 1GB da komfortowy zapas.

> **Uwaga**: Mikr.us używa wirtualizacji OpenVZ/LXC, więc **Docker może nie działać** lub być ograniczony. Poniższy tutorial zakłada uruchomienie natywne .NET.

---

## 2.1. Pierwsze kroki po zakupie Mikrusa

Po zakupie otrzymasz:
- IP serwera
- Hasło root
- Port SSH (zwykle 22 lub niestandardowy)

```bash
# Połącz się przez SSH
ssh root@TWOJE_IP -p PORT

# Zmień hasło root (zalecane)
passwd

# Aktualizacja systemu
apt update && apt upgrade -y
```

## 2.2. Konfiguracja użytkownika (opcjonalne, ale zalecane)

```bash
# Utwórz użytkownika dla aplikacji
adduser theoffice
usermod -aG sudo theoffice

# Przełącz się na nowego użytkownika
su - theoffice
```

## 2.3. Instalacja .NET 9.0 Runtime

```bash
# Metoda 1: Skrypt Microsoft (działa na większości dystrybucji)
cd ~
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 9.0 --runtime aspnetcore

# Dodaj do PATH
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$DOTNET_ROOT' >> ~/.bashrc
source ~/.bashrc

# Sprawdź
dotnet --info
```

```bash
# Metoda 2: Manualna instalacja (jeśli skrypt nie działa)
# Sprawdź architekturę
uname -m  # x86_64 dla większości VPS

# Pobierz runtime dla Linux x64
cd /tmp
wget https://download.visualstudio.microsoft.com/download/pr/aspnetcore-runtime-9.0.0-linux-x64.tar.gz

# Rozpakuj
mkdir -p $HOME/.dotnet
tar -xzf aspnetcore-runtime-9.0.0-linux-x64.tar.gz -C $HOME/.dotnet

# Ustaw PATH jak wyżej
```

## 2.4. Deployment aplikacji

### Opcja A: Build lokalnie i upload (zalecane dla słabych VPS)

**Na swoim komputerze:**

```bash
cd TheOfficeAPI

# Publish dla Linux x64
dotnet publish src/TheOfficeAPI/TheOfficeAPI.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained false \
  -o ./publish-linux

# Spakuj
tar -czvf theoffice-api.tar.gz -C publish-linux .

# Upload na serwer
scp -P PORT theoffice-api.tar.gz root@TWOJE_IP:/home/theoffice/
```

**Na serwerze Mikrus:**

```bash
cd /home/theoffice
mkdir -p app
tar -xzvf theoffice-api.tar.gz -C app
rm theoffice-api.tar.gz

# Test uruchomienia
cd app
ASPNETCORE_URLS=http://0.0.0.0:5000 dotnet TheOfficeAPI.dll
```

### Opcja B: Klonowanie i build na serwerze

```bash
# Zainstaluj git
sudo apt install git -y

# Zainstaluj .NET SDK (potrzebny do buildu)
./dotnet-install.sh --channel 9.0  # pełny SDK, nie tylko runtime

# Klonuj repozytorium
cd ~
git clone https://github.com/fszymaniak/TheOfficeAPI.git
cd TheOfficeAPI

# Publish
dotnet publish src/TheOfficeAPI/TheOfficeAPI.csproj \
  -c Release \
  -o ~/app \
  --no-restore

# Usuń SDK (opcjonalnie, oszczędza miejsce)
# rm -rf ~/.dotnet
# Zainstaluj tylko runtime
# ./dotnet-install.sh --channel 9.0 --runtime aspnetcore
```

## 2.5. Konfiguracja systemd service

```bash
sudo nano /etc/systemd/system/theoffice-api.service
```

Zawartość (dostosuj ścieżki):
```ini
[Unit]
Description=TheOfficeAPI
After=network.target

[Service]
Type=notify
User=theoffice
WorkingDirectory=/home/theoffice/app
ExecStart=/home/theoffice/.dotnet/dotnet /home/theoffice/app/TheOfficeAPI.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=theoffice-api

# Environment
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
Environment=MATURITY_LEVEL=Level0
Environment=DOTNET_ROOT=/home/theoffice/.dotnet

# Limity (dostosuj do planu Mikrusa)
MemoryMax=512M
CPUQuota=80%

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl daemon-reload
sudo systemctl enable theoffice-api
sudo systemctl start theoffice-api

# Sprawdź status
sudo systemctl status theoffice-api

# Sprawdź logi
sudo journalctl -u theoffice-api -f
```

## 2.6. Konfiguracja firewalla

```bash
# Zainstaluj ufw (jeśli nie ma)
sudo apt install ufw -y

# Podstawowa konfiguracja
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow ssh
sudo ufw allow 5000/tcp  # lub 80 jeśli używasz nginx

sudo ufw enable
sudo ufw status
```

## 2.7. (Opcjonalnie) Reverse Proxy z Nginx + SSL

```bash
sudo apt install nginx certbot python3-certbot-nginx -y

sudo nano /etc/nginx/sites-available/theoffice-api
```

Zawartość:
```nginx
server {
    listen 80;
    server_name twoja-domena.pl;  # lub IP

    location / {
        proxy_pass http://127.0.0.1:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
sudo ln -s /etc/nginx/sites-available/theoffice-api /etc/nginx/sites-enabled/
sudo rm /etc/nginx/sites-enabled/default  # usuń domyślną konfigurację
sudo nginx -t
sudo systemctl reload nginx

# SSL (jeśli masz domenę)
sudo certbot --nginx -d twoja-domena.pl
```

## 2.8. Monitoring i zarządzanie

```bash
# Sprawdź zużycie zasobów
htop
free -h

# Logi aplikacji
sudo journalctl -u theoffice-api --since "1 hour ago"

# Restart aplikacji
sudo systemctl restart theoffice-api

# Status
sudo systemctl status theoffice-api
```

## 2.9. Automatyczne aktualizacje aplikacji

Utwórz skrypt aktualizacji:

```bash
nano ~/update-api.sh
```

Zawartość:
```bash
#!/bin/bash
set -e

cd ~/TheOfficeAPI
git pull origin main

dotnet publish src/TheOfficeAPI/TheOfficeAPI.csproj \
  -c Release \
  -o ~/app \
  --no-restore

sudo systemctl restart theoffice-api
echo "Update completed!"
```

```bash
chmod +x ~/update-api.sh

# Uruchom aktualizację
./update-api.sh
```

---

# Część 3: Porównanie platform

| Aspekt | Raspberry Pi 5 | Mikr.us |
|--------|---------------|---------|
| **Koszt** | ~350 PLN jednorazowo + prąd | 59-179 PLN/rok |
| **Docker** | ✅ Pełne wsparcie | ⚠️ Ograniczone/brak |
| **Dostępność** | 🏠 Zależna od sieci domowej | 🌐 24/7 w datacenter |
| **IP publiczne** | ❌ Wymaga port forwarding | ✅ Tak |
| **Wydajność** | 🚀 Bardzo dobra (4x ARM64) | 📊 Zależna od planu |
| **Skalowalność** | ❌ Ograniczona | ✅ Można zmienić plan |
| **Backup** | 🔧 Ręczny | 🔧 Ręczny |

## Rekomendacja końcowa

- **Do nauki/developmentu**: Raspberry Pi 5 (pełna kontrola, Docker)
- **Do prostego hostingu**: Mikrus 3.0 (tani, publiczny IP)
- **Do produkcji**: Rozważ cloud (Azure, AWS, DigitalOcean) lub railway.app

---

# Troubleshooting

## Problem: Aplikacja nie startuje

```bash
# Sprawdź logi
sudo journalctl -u theoffice-api -n 50

# Sprawdź czy port jest wolny
sudo netstat -tlnp | grep 5000

# Sprawdź uprawnienia
ls -la ~/app/
```

## Problem: Brak pamięci (Mikrus)

```bash
# Sprawdź zużycie
free -h

# Włącz swap (jeśli możliwe na OpenVZ)
sudo fallocate -l 512M /swapfile
sudo chmod 600 /swapfile
sudo mkswap /swapfile
sudo swapon /swapfile
```

## Problem: Timeout przy starcie .NET

Dodaj do service file:
```ini
TimeoutStartSec=120
```

---

# Przydatne komendy

```bash
# Health check
curl http://localhost:5000/health

# API info
curl http://localhost:5000/

# Swagger (w przeglądarce)
# http://TWOJE_IP:5000/swagger
```
