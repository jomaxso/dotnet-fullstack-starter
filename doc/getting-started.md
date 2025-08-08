# Getting Started Guide

Welcome to the .NET 9 Aspire Fullstack Starter! This guide will help you get up and running quickly with your new project.

## 🚀 Quick Setup

### 1. Prerequisites Check

Before you begin, ensure you have the following installed:

- **.NET SDK 9.0** or higher
- **Docker Desktop** or **Podman**
- **Aspire CLI**
- **Node.js** (required for integration testing with Playwright and browser automations)

### 2. First Run

```powershell
# Clone or download this project
git clone <your-repo-url>
cd your-project-name

# Start all services
aspire run
```

That's it! Your application will be running with all services orchestrated automatically.

## 🔍 What's Running?

After starting with `aspire run`, you'll have access to:

| Service | URL | Description |
|---------|-----|-------------|
| **Aspire Dashboard** | https://localhost:17178 | Central dashboard for all services |
| **API Documentation** | https://localhost:5001/scalar | Interactive API documentation |
| **Frontend Application** | https://localhost:5002 | Your main application |
| **Database Admin** | Via Dashboard | MySQL administration interface |

## 🎯 Your First Steps

### 1. Explore the Application

1. Open the **Web Application** at https://localhost:5002
2. Check out the **API Documentation** at https://localhost:5001/scalar
3. Monitor services in the **Aspire Dashboard** at https://localhost:17178

### 2. Understanding the Architecture

The project follows a clean architecture pattern:

```
Your Project/
├── AppHost/           # Aspire orchestration
├── src/
│   ├── Api/          # Backend API
│   ├── Api.Migrations/ # Database migrations
│   ├── ServiceDefaults/ # Shared configuration
│   └── Frontend/     # Frontend application
└── test/             # Integration tests
```

### 3. Making Your First Changes

#### Add a New API Endpoint

1. Navigate to `src/Api/Endpoints/`
2. Create a new endpoint class
3. Register it in `EndpointRegistration.cs`

#### Add a New Page

1. Navigate to `src/Frontend/Pages/`
2. Create a new Blazor component
3. Add navigation if needed

#### Database Changes

1. Add/modify entities in `src/Api/Domain/`
2. Create a new migration:
   ```powershell
   cd src/Api.Migrations
   dotnet ef migrations add YourMigrationName
   ```
3. Restart the application - migrations run automatically

## 🔐 Authentication

The project includes ASP.NET Core Identity with:

- User registration and login
- Cookie-based authentication
- Cross-service authentication between API and Frontend

### Testing Authentication

1. Navigate to the Frontend Application
2. Click on "Register" to create a new account
3. Login with your credentials

## 🗄️ Database

### Viewing Data

Use the **DbGate** interface (accessible via the Aspire Dashboard) to:
- Browse database tables
- Execute queries
- View migration history

### Adding Sample Data

You can add initial data in `src/Api.Migrations/DbSeeder.cs`.

## 🧪 Testing

Run the integration tests to verify everything works:

```powershell
dotnet test
```

The tests use Playwright for browser automation and test real user workflows.

## 🚀 Next Steps

### Customize for Your Project

1. **Rename the solution**: Update `DotnetFullstackStarter.slnx`
2. **Update namespaces**: Replace starter namespaces with your project names
3. **Add your domain models**: Create entities in `src/Api/Domain/`
4. **Build your UI**: Add pages and components in `src/Frontend/`
5. **Configure services**: Modify `AppHost.cs` as needed

### Add More Services

The Aspire architecture makes it easy to add:
- Redis caching
- Message queues
- External APIs
- Additional databases

### Production Deployment

When ready for production:

```powershell
aspire publish --output-path ./publish
```

This generates Docker Compose files for easy deployment.

## 🆘 Need Help?

- Check the [Troubleshooting Guide](troubleshooting.md)
- Review [Architecture Documentation](architecture.md)
- See [Deployment Guide](deployment.md)

## 📚 Learn More

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Blazor WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [MudBlazor Components](https://mudblazor.com/)
