# .NET 9 Aspire Fullstack Starter

A modern fullstack starter built with .NET 9 Aspire as a distributed application. This project provides a complete foundation for building scalable web applications with clean architecture principles.

## 📋 Table of Contents

- [⚡ Quick Start](#-quick-start)
- [📋 Prerequisites](#-prerequisites)
- [🎯 Getting Started](#-getting-started)
- [🏗️ What's Included](#-whats-included)
- [💻 Development](#-development)
- [🚀 Production Deployment](#-production-deployment)
- [🔧 Using This Project](#-using-this-project)
- [🚨 Troubleshooting](#-troubleshooting)
- [📖 Documentation](#-documentation)
- [🌐 Learn More](#-learn-more)

## ⚡ Quick Start

**Want to try it immediately? Just run:**

```powershell
aspire run
```

That's it! 🎉 All services will start automatically.

**Your apps will be available at:**
- **Frontend**: https://localhost:5002 ← **Start here!**
- **API Docs**: https://localhost:5001/scalar 
- **Dashboard**: https://localhost:17178 (monitor all services)

> **Note**: First run automatically sets up the database with sample data.

## 📋 Prerequisites

**You need these installed first:**

1. **[.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)**  or later
2. **[Docker Desktop](https://www.docker.com/products/docker-desktop)** or **[Podman](https://podman.io/docs/installation)**
3. **[Node.js](https://nodejs.org/)** (required for integration testing withPlaywright and browser automations)
4. **[Aspire CLI](https://learn.microsoft.com/en-us/dotnet/aspire/cli/install)**: `dotnet tool install -g Aspire.Cli`

> 💡 **Alternative Aspire install**: Native installation available via [official docs](https://learn.microsoft.com/en-us/dotnet/aspire/cli/install)

## 🎯 Getting Started

**Step-by-step setup:**

1. **Clone this repository**
   ```powershell
   git clone <your-repo-url>
   cd dotnet-fullstack-starter
   ```

2. **Install prerequisites** (see above) 

3. **Start everything**
   ```powershell
   aspire run
   ```

4. **Explore your app**
   - Open **Frontend** at https://localhost:5002
   - Try the **API** at https://localhost:5001/scalar
   - Monitor with **Dashboard** at https://localhost:17178

> 📚 **Need more details?** Check our [Getting Started Guide](doc/getting-started.md) for step-by-step instructions.

## 🏗️ What's Included

- ✅ **Authentication**: ASP.NET Core Identity with cookie authentication
- ✅ **Database**: MySQL with Entity Framework Core and automatic migrations
- ✅ **Modern UI**: Blazor WebAssembly with MudBlazor components
- ✅ **API Docs**: Scalar API documentation interface
- ✅ **Health Checks**: Built-in service monitoring
- ✅ **Testing**: Integration tests with Playwright and MCP support
- ✅ **Production Ready**: Docker deployment with Aspire manifests

## 💻 Development

```powershell
# Start development environment
aspire run

# Run all tests
dotnet test

# Run only integration tests
dotnet test test/Integration.Test

# Install Playwright browsers (first time only)
pwsh test/Integration.Test/bin/Debug/net9.0/playwright.ps1 install

# Create database migration
cd src/Api.Migrations
dotnet ef migrations add YourMigrationName
```

**Services running after `aspire run`:**
- **Aspire Dashboard**: https://localhost:17178
- **API + Docs**: https://localhost:5001/scalar  
- **Frontend**: https://localhost:5002
- **DbGate**: Available via dashboard

## 🚀 Production Deployment

```powershell
# Generate deployment files
aspire publish --output-path ./publish

# Deploy with Docker Compose
cd ./publish
docker-compose up -d
```

> 📖 **Detailed deployment instructions**: [Deployment Guide](doc/deployment.md)

## 🔧 Using This Project

1. **Use as Base**: Clone this repository for new projects
2. **Rename Solution**: Update `DotnetFullstackStarter.slnx` and namespaces  
3. **Add Your Domain**: Modify entities in `src/Api/Domain/`
4. **Customize UI**: Build your pages in `src/Frontend/`
5. **Deploy**: Use `aspire publish` for production

> 🏗️ **Architecture details**: [Architecture Overview](doc/architecture.md)
> 💻 **Development workflows**: [Development Guide](doc/development.md)

## 🚨 Troubleshooting

**Common issues:**
- **Aspire CLI not found**: `dotnet tool install -g Aspire.Cli`
- **Docker not running**: Start Docker Desktop
- **Port conflicts**: Check what's using ports 5001, 5002, 17178
- **Database issues**: Check DbGate via Aspire Dashboard

> 🔧 **Complete troubleshooting guide**: [Troubleshooting](doc/troubleshooting.md)

## 📖 Documentation

📚 **Complete guides available in the `/doc` folder:**

- **[Getting Started Guide](doc/getting-started.md)** - Step-by-step setup and first steps
- **[Architecture Overview](doc/architecture.md)** - Understanding the project structure
- **[Development Guide](doc/development.md)** - Best practices and workflows
- **[Deployment Guide](doc/deployment.md)** - Production deployment instructions
- **[Troubleshooting](doc/troubleshooting.md)** - Common issues and solutions

## 🌐 Learn More

**Integration Testing with Playwright:**
- Tests use Playwright for browser automation
- MCP (Model Context Protocol) support for AI-driven testing
- Run `pwsh test/Integration.Test/bin/Debug/net9.0/playwright.ps1 install` to install browsers
- Tests are located in `test/Integration.Test/`

**Documentation:**
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Blazor WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [MudBlazor Components](https://mudblazor.com/)
- [Playwright Testing](https://playwright.dev/dotnet/)
