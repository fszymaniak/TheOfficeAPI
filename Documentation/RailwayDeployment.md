# Railway Deployment Guide

This document explains how to deploy TheOfficeAPI to Railway and configure the Richardson Maturity Model level.

## Environment Variables

Railway deployment uses the following environment variables:

### Required Variables

| Variable | Default | Options | Description |
|----------|---------|---------|-------------|
| `MATURITY_LEVEL` | `Level0` | `Level0`, `Level1`, `Level2` | Determines which Richardson Maturity Model level to run |
| `PORT` | `8080` | any port | Port number (automatically set by Railway) |
| `ASPNETCORE_ENVIRONMENT` | `Production` | `Development`, `Production` | ASP.NET Core environment |

## Swagger Versioning

The API Swagger documentation version aligns with the Richardson Maturity Model level:

| MATURITY_LEVEL | Swagger Version | Swagger JSON URL |
|----------------|----------------|------------------|
| `Level0` | v0 | `/swagger/v0/swagger.json` |
| `Level1` | v1 | `/swagger/v1/swagger.json` |
| `Level2` | v2 | `/swagger/v2/swagger.json` |

## Changing Maturity Levels on Railway

To change the API maturity level on Railway:

### Via Railway Dashboard

1. Go to your Railway project dashboard
2. Select your service
3. Navigate to **Variables** tab
4. Add or update the `MATURITY_LEVEL` variable:
   - For Level 0: Set `MATURITY_LEVEL=Level0`
   - For Level 1: Set `MATURITY_LEVEL=Level1`
   - For Level 2: Set `MATURITY_LEVEL=Level2`
5. Click **Deploy** to apply changes

### Via Railway CLI

```bash
# Set to Level 0
railway variables set MATURITY_LEVEL=Level0

# Set to Level 1
railway variables set MATURITY_LEVEL=Level1

# Set to Level 2
railway variables set MATURITY_LEVEL=Level2
```

## Verification

After deployment, verify the correct maturity level is running:

1. Check Swagger UI: `https://your-app.railway.app/swagger`
2. Check the API title - it should show "The Office API - Level X"
3. Verify Swagger JSON is accessible: `https://your-app.railway.app/swagger/vX/swagger.json` (where X matches your level)

## Troubleshooting

### Swagger JSON Returns 404

- Verify `MATURITY_LEVEL` environment variable is set correctly
- Check Railway logs for startup errors
- Ensure the Swagger version in the URL matches your maturity level

### API Returns 500 Error

- Check Railway logs: `railway logs`
- Verify all environment variables are set
- Ensure the application built successfully

## Default Configuration

The Dockerfile sets `MATURITY_LEVEL=Level0` by default. If you don't explicitly set this variable in Railway, the API will run at Richardson Maturity Model Level 0 with Swagger v0.

## Production URL

Live API: https://theofficeapi-production-5d8f.up.railway.app
- Swagger UI: https://theofficeapi-production-5d8f.up.railway.app/swagger
- Current Level: Check the Swagger UI title for the active maturity level
