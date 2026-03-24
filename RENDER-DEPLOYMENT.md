# Render Deployment Guide

## Prerequisites
- Render account (https://render.com)
- Docker image ready (created via Dockerfile)
- SQL Server instance (can use Azure SQL or any cloud SQL Server)

## Deployment Steps

### 1. Push to GitHub
```bash
git push origin main
```

### 2. Create Web Service on Render
1. Go to [Render Dashboard](https://dashboard.render.com)
2. Click **New +** → **Web Service**
3. Select **Deploy from a Git repository**
4. Enter your GitHub repository URL
5. Click **Connect**

### 3. Configure Web Service

**Name**: `dbfetcher` (or your preferred name)

**Environment**: Select runtime based on your setup
- If deploying with Docker: **Docker**

**Build Command**: 
```bash
# Docker will automatically build from Dockerfile
```

**Start Command**:
```bash
# Not needed for Docker - uses ENTRYPOINT from Dockerfile
```

### 4. Set Environment Variables
In the Render dashboard, add these environment variables in **Environment**:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__Default=Server=your-azure-sql-server.database.windows.net;Database=InvoicesDB;User Id=username;Password=password;TrustServerCertificate=True;Encrypt=True;
```

**Note**: Replace with your actual SQL Server connection details

### 5. Port Configuration
- Port: `5129` (already exposed in Dockerfile)
- The application will listen on `http://0.0.0.0:5129`

### 6. Deploy
Click **Deploy** and wait for the build and deployment to complete.

## Testing After Deployment
Once deployed, access your API:
- Base URL: `https://your-service-name.onrender.com`
- Swagger UI: `https://your-service-name.onrender.com/swagger`

## Troubleshooting

### Database Connection Issues
- Verify SQL Server connection string is correct
- Ensure your SQL Server firewall allows connections from Render's IP ranges
- Check database exists and migrations have run

### Port Issues
- Render automatically forwards requests to port 5129
- The app listens on `0.0.0.0:5129` (all interfaces)

### Logs
- View real-time logs in Render dashboard under **Logs**
- Check for connection string and environment variable issues

## Local Testing with Docker

```bash
# Build locally
docker build -t dbfetcher:latest .

# Run locally
docker run -p 5129:5129 \
  -e "ConnectionStrings__Default=Server=localhost;Database=InvoicesDB;Integrated Security=True;TrustServerCertificate=True;" \
  dbfetcher:latest
```

Then access: `http://localhost:5129/swagger`

## Notes
- The app runs in **Production** environment by default in Docker
- HTTPS is handled by Render (your app uses HTTP internally)
- Swagger UI is available in Production for testing
- CORS is configured to allow any origin (adjust as needed for security)
