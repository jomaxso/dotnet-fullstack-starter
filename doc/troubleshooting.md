# Troubleshooting Guide

This guide helps you resolve common issues when working with the .NET 9 Aspire Fullstack Starter.

## 🚨 Common Issues

### Aspire & Container Issues

#### ❌ Aspire CLI Not Found
```
Error: 'aspire' is not recognized as an internal or external command
```

**Solution:**
```powershell
# Install Aspire CLI
dotnet tool install -g Aspire.Cli

# Verify installation
aspire --version

# If still not working, restart your terminal
# Or add to PATH: %USERPROFILE%\.dotnet\tools
```

#### ❌ Docker Desktop Not Running
```
Error: Docker daemon is not running
```

**Solution:**
```powershell
# Start Docker Desktop
# Or restart Docker service
net stop docker
net start docker

# Verify Docker is running
docker --version
docker ps
```

#### ❌ Container Port Conflicts
```
Error: Port 5001 is already in use
```

**Solution:**
```powershell
# Find what's using the port
netstat -ano | findstr :5001

# Kill the process (replace PID)
taskkill /PID <process-id> /F

# Or change ports in AppHost.cs
.WithHttpEndpoint(port: 5011, targetPort: 8080)
```

#### ❌ Aspire Dashboard Not Accessible
```
Dashboard at https://localhost:17178 not loading
```

**Solution:**
```powershell
# Check if Aspire is running
aspire run --help

# Check firewall settings
# Allow .NET applications through Windows Firewall

# Try different browser
# Clear browser cache

# Check logs
aspire run --verbosity detailed
```

### Database Issues

#### ❌ Database Connection Failed
```
Error: Unable to connect to any of the specified MySQL hosts
```

**Solution:**
```powershell
# Check if MySQL container is running
docker ps | findstr mysql

# Check container logs
docker logs <mysql-container-id>

# Verify connection string in appsettings
# Check DbGate connection via Aspire Dashboard

# Reset database container
docker-compose down
docker volume rm <mysql-volume>
aspire run
```

#### ❌ Migration Fails
```
Error: A connection was successfully established with the server, but then an error occurred during the login process
```

**Solution:**
```powershell
# Check MySQL is ready
docker exec <mysql-container> mysql -u root -p -e "SELECT 1"

# Wait for MySQL to fully start
# Add wait logic in migrations project

# Manual migration
cd src/Api.Migrations
dotnet ef database update --verbose
```

#### ❌ Seeding Data Fails
```
Error: Cannot insert duplicate key in object 'dbo.Users'
```

**Solution:**
```csharp
// In DbSeeder.cs, check if data exists first
if (!context.Users.Any())
{
    context.Users.AddRange(seedUsers);
    await context.SaveChangesAsync();
}
```

### API Issues

#### ❌ API Returns 404 for All Endpoints
```
Error: 404 Not Found for /api/users
```

**Solution:**
```csharp
// Check endpoint registration in Program.cs
app.MapEndpoints(); // Ensure this line exists

// Verify endpoint implementation
public class GetUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", async () => 
        {
            // Implementation
        });
    }
}
```

#### ❌ CORS Errors in Browser
```
Error: CORS policy: No 'Access-Control-Allow-Origin' header
```

**Solution:**
```csharp
// In API Program.cs
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:5002") // Frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

app.UseCors();
```

#### ❌ Authentication Issues
```
Error: 401 Unauthorized
```

**Solution:**
```csharp
// Check middleware order in Program.cs
app.UseAuthentication();
app.UseAuthorization();

// Verify cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
```

### Frontend Issues

#### ❌ Blazor WebAssembly Fails to Load
```
Error: Failed to start platform. Fetching from the cache.
```

**Solution:**
```powershell
# Clear browser cache completely
# Check browser console for detailed errors

# Verify published files
ls src/Frontend/bin/Release/net9.0/publish/wwwroot/_framework/

# Check if API is accessible
curl https://localhost:5001/health
```

#### ❌ MudBlazor Components Not Rendering
```
Components appear unstyled or broken
```

**Solution:**
```html
<!-- In App.razor, ensure MudBlazor CSS is loaded -->
<link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />

<!-- In wwwroot/index.html -->
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
```

#### ❌ Authentication State Not Persisting
```
User gets logged out on page refresh
```

**Solution:**
```csharp
// Check CookieAuthenticationStateProvider implementation
// Verify localStorage is working

// In browser console
localStorage.getItem('user-info')

// Clear localStorage if corrupted
localStorage.clear()
```

### Performance Issues

#### ❌ Slow Application Startup
```
Application takes too long to start
```

**Solution:**
```powershell
# Check container resource usage
docker stats

# Increase container resources in Docker Desktop
# Settings > Resources > Advanced

# Optimize Entity Framework
# Use AsNoTracking for read-only queries
# Add database indexes
```

#### ❌ High Memory Usage
```
Application consuming too much memory
```

**Solution:**
```csharp
// Optimize Entity Framework queries
var users = await context.Users
    .AsNoTracking()
    .Select(u => new UserDto { Id = u.Id, Name = u.Name })
    .ToListAsync();

// Dispose resources properly
using var scope = serviceProvider.CreateScope();

// Monitor garbage collection
GC.Collect();
```

## 🔍 Diagnostic Commands

### Container Diagnostics

```powershell
# List all containers
docker ps -a

# Check container logs
docker logs <container-name> --tail 50 -f

# Execute commands in container
docker exec -it <container-name> bash

# Check container resource usage
docker stats

# Inspect container configuration
docker inspect <container-name>
```

### Database Diagnostics

```powershell
# Connect to MySQL
docker exec -it <mysql-container> mysql -u root -p

# Check database status
SHOW DATABASES;
USE yourapp;
SHOW TABLES;
DESCRIBE Users;

# Check migration history
SELECT * FROM __EFMigrationsHistory;

# Check connection count
SHOW STATUS WHERE variable_name = 'Threads_connected';
```

### Application Diagnostics

```powershell
# Check application health
curl https://localhost:5001/health

# View detailed health
curl https://localhost:5001/health | jq

# Check application logs
docker logs <api-container> | grep -E "(Error|Exception|Warning)"

# Monitor application metrics
# Use Aspire Dashboard for real-time monitoring
```

### Network Diagnostics

```powershell
# Check port usage
netstat -ano | findstr :5001
netstat -ano | findstr :5002

# Test connectivity between containers
docker exec <api-container> ping mysql-container
docker exec <frontend-container> curl http://api-container:8080/health

# Check DNS resolution
nslookup localhost
```

## 🛠️ Debug Tools

### Visual Studio / VS Code

```json
// launch.json for debugging
{
    "name": "Debug AppHost",
    "type": "coreclr",
    "request": "launch",
    "program": "${workspaceFolder}/AppHost/bin/Debug/net9.0/AppHost.exe",
    "args": [],
    "cwd": "${workspaceFolder}/AppHost",
    "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
    }
}
```

### Browser Developer Tools

```javascript
// Check localStorage state
console.log(localStorage.getItem('user-info'));

// Check authentication cookies
document.cookie;

// Monitor network requests
// Open Network tab in DevTools
// Filter by API calls
```

### Database Tools

1. **DbGate** - Available via Aspire Dashboard
2. **MySQL Workbench** - External GUI tool
3. **Command Line** - Direct MySQL CLI access

### Logging

```csharp
// Enhanced logging in Program.cs
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// In Development, use verbose logging
if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
```

## 📊 Performance Monitoring

### Built-in Monitoring

```csharp
// Add Application Insights (optional)
builder.Services.AddApplicationInsightsTelemetry();

// Add custom metrics
builder.Services.AddSingleton<IMetrics, Metrics>();

// Health checks with details
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddCheck("custom-check", () => 
    {
        // Custom health logic
        return HealthCheckResult.Healthy("All systems operational");
    });
```

### External Monitoring Tools

```yaml
# docker-compose.monitoring.yml
version: '3.8'
services:
  prometheus:
    image: prom/prometheus
    ports:
      - "9090:9090"
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml

  grafana:
    image: grafana/grafana
    ports:
      - "3000:3000"
    environment:
      GF_SECURITY_ADMIN_PASSWORD: admin
```

## 🆘 Getting Help

### Log Analysis

```powershell
# Windows Event Logs
Get-EventLog -LogName Application -Source ".NET Runtime" -Newest 10

# Container logs with timestamps
docker logs <container> --timestamps

# Follow logs in real-time
docker logs <container> -f --tail 100
```

### Common Log Patterns

```
# Database connection issues
"Failed to connect to MySQL"
"Connection timeout expired"

# Authentication issues
"Unable to obtain user information"
"Cookie authentication failed"

# Startup issues
"Application startup exception"
"Service registration failed"
```

### Support Resources

1. **Aspire Documentation**: https://learn.microsoft.com/en-us/dotnet/aspire/
2. **GitHub Issues**: Report bugs and feature requests
3. **Stack Overflow**: Tag questions with `aspire` and `dotnet`
4. **Microsoft Q&A**: Official Microsoft support forums

### Creating Bug Reports

Include the following information:

```
### Environment
- OS: Windows 11
- .NET Version: 9.0.x
- Aspire Version: 9.4.0
- Docker Version: 24.0.x

### Steps to Reproduce
1. Run `aspire run`
2. Navigate to https://localhost:5002
3. Click login button
4. Error occurs

### Expected Behavior
User should be redirected to login page

### Actual Behavior
Application throws exception

### Logs
```
[Include relevant log output]
```

### Additional Context
Any additional information about the problem
```

## 🔧 Quick Fixes

### Reset Everything

```powershell
# Nuclear option - reset all containers and data
docker-compose down -v
docker system prune -a
aspire run
```

### Clear Caches

```powershell
# Clear .NET caches
dotnet clean
dotnet restore

# Clear browser caches
# Clear localStorage: localStorage.clear()

# Clear Docker build cache
docker builder prune
```

### Restart Services

```powershell
# Restart specific service
docker-compose restart api

# Restart all services
docker-compose restart

# Or restart through Aspire
# Stop aspire run (Ctrl+C)
# Start again: aspire run
```
