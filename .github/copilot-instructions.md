# .NET 9 Aspire Fullstack Starter - AI Coding Agent Instructions

## Architecture Overview

This is a modern .NET 9 **Aspire** distributed application following clean architecture principles:

### Key Components
- **AppHost**: Aspire orchestration host managing MySQL, API, migrations, and frontend
- **API**: Minimal API with ASP.NET Core Identity and Entity Framework Core MySQL backend  
- **Frontend**: Blazor WebAssembly UI using MudBlazor components with cookie authentication
- **Api.Migrations**: Automated EF Core database migrations and seeding
- **ServiceDefaults**: Shared Aspire configuration and service definitions

## Development Workflows

### Building & Running
```powershell
# Run the Aspire distributed application (recommended)
aspire run

# Alternative: Run with .NET CLI 
dotnet run --project AppHost
```

### Service Architecture in AppHost.cs
- **MySQL Database**: MariaDB with persistent volumes and auto-migration on startup
- **DbGate**: MySQL admin tool for database management  
- **API Migrations**: `Api.Migrations` project runs EF Core migrations automatically
- **API**: Minimal API with ASP.NET Core Identity at `/api` with Scalar docs at `/scalar`
- **Frontend**: Blazor WebAssembly client with cookie-based authentication

## Critical Patterns

### Aspire Service Integration
- All projects reference `ServiceDefaults` for shared configuration
- Use `builder.AddServiceDefaults()` for telemetry, health checks, service discovery
- Database connections via `builder.AddMySqlDbContext<ApplicationDbContext>(Services.Database)`
- Service names defined in `Services.cs` constants

### Authentication Architecture (API + Frontend)
- **API**: ASP.NET Core Identity with cookie authentication (`IdentityConstants.ApplicationScheme`)
- **Frontend**: Blazor WASM with `CookieAuthenticationStateProvider` using localStorage + HTTP cookies
- **Pattern**: Frontend calls `/api/login?useCookies=true`, stores `UserInfo` in localStorage, uses delegating handler for cookie inclusion
- **Session**: 30-minute sliding expiration with automatic refresh from `/api/manage/user`

### Data Access Patterns
- **Entity Framework**: `ApplicationDbContext` with clean domain models in `Api/Domain/`
- **Identity**: Custom `User`, `Role` entities extending ASP.NET Core Identity
- **Migrations**: Automatic execution on startup via `Api.Migrations` project
- **Configuration**: Standard appsettings.json with Aspire service discovery

### UI Component Standards
- **MudBlazor**: Primary component library for UI elements
- **Authentication**: Uses `CookieAuthenticationStateProvider` with localStorage persistence
- **HTTP Client**: `CookieDelegatingHandler` ensures cookies are included in all requests

## Project-Specific Conventions

### File Organization
- **Domain models**: `src/Api/Domain/` (shared between API and migrations)
- **Data access**: `ApplicationDbContext` in `src/Api/Data/`
- **Frontend components**: `src/Frontend/` with standard Blazor structure
- **Identity**: Custom authentication logic in `src/Frontend/Identity/`

### Configuration Management
- **Service names**: Defined in `ServiceDefaults/Services.cs`
- **Database**: MariaDB/MySQL with Entity Framework migrations
- **Connection strings**: Managed by Aspire service discovery

### API Endpoint Patterns
```csharp
// Standard API group pattern
var api = app.MapGroup("api");
api.MapIdentityApi<User>();

// Protected endpoint pattern
accountGroup.MapGet("/User", (ClaimsPrincipal user) => { /* ... */ })
    .RequireAuthorization();
```

## Integration Points

### Database Architecture
- **Single MySQL database** with Entity Framework Core
- **Migrations**: `Api.Migrations` project handles schema changes automatically
- **Identity**: ASP.NET Core Identity tables with custom user/role entities

### Frontend-API Communication
- **Cookie authentication**: Frontend uses cookies for API authentication
- **Service discovery**: Aspire handles service-to-service communication
- **CORS**: Configured for localhost development with credential support

### Testing Infrastructure
- **Integration tests**: Using Playwright for browser automation
- **AspireFixture**: Manages test application lifecycle
- **Test projects**: Located in `test/` directory

## Development Guidelines

- **Database changes**: Always use EF migrations, never direct schema modifications
- **Authentication**: Frontend stores user info in localStorage with automatic refresh
- **Project references**: Use Aspire service discovery, avoid hardcoded URLs
- **Testing**: Write integration tests using the provided AspireFixture
- **Service registration**: Add all services via `builder.AddServiceDefaults()`

## Troubleshooting Common Issues

### Project Reference Mismatches
- **Solution file**: References `src/WebApp/WebApp.csproj` but actual project is `src/Frontend/Frontend.csproj`
- **Fix**: Update solution file or rebuild with `dotnet build` to resolve compilation errors

### Database Connection Issues
- **Check**: Ensure Docker is running for MySQL container
- **Verify**: Database initialization through `Api.Migrations` project
- **Debug**: Use DbGate via Aspire Dashboard to inspect database state

### Authentication Debugging
- **Frontend**: Check browser localStorage for `UserSession` key
- **API**: Verify cookie authentication with 30-minute sliding expiration
- **Network**: Use browser DevTools to inspect `/api/manage/user` calls

## Key Files for Understanding
- `AppHost/AppHost.cs` - Service orchestration and dependencies
- `src/ServiceDefaults/Extensions.cs` - Shared Aspire configuration
- `src/Api/Program.cs` - API setup with Identity and endpoints
- `src/Frontend/Identity/CookieAuthenticationStateProvider.cs` - Custom auth implementation
- `src/Api/Data/ApplicationDbContext.cs` - Database schema and relationships
- `test/Integration.Test/Common/AspireFixture.cs` - Test infrastructure setup
