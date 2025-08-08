# Development Guide

This guide covers development workflows, best practices, and advanced topics for the .NET 9 Aspire Fullstack Starter.

## 🚀 Development Workflow

### Daily Development Routine

```powershell
# 1. Start development environment
aspire run

# 2. Make changes to code
# 3. Test changes (auto-reload enabled)
# 4. Run tests
dotnet test

# 5. Commit changes
git add .
git commit -m "feat: your changes"
```

### Project Setup for New Developers

1. **Clone and Setup**
   ```powershell
   git clone <your-repo>
   cd your-project
   aspire run
   ```

2. **Verify Installation**
   - Check Aspire Dashboard: https://localhost:17178
   - Test API: https://localhost:5001/scalar
   - Test Frontend: https://localhost:5002

3. **Development Tools**
   - Database: Use DbGate from Aspire Dashboard
   - API Testing: Use Scalar interface
   - Logs: Monitor in Aspire Dashboard

## 🏗️ Code Organization

### Domain-Driven Design

```
src/Api/Domain/
├── Common/           # Shared base classes
├── Entities/         # Domain entities
├── ValueObjects/     # Value objects
├── Interfaces/       # Domain interfaces
└── Services/         # Domain services
```

### API Endpoint Organization

```csharp
// src/Api/Endpoints/Users/GetUser.cs
public class GetUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id}", async (int id, IUserService userService) =>
        {
            var user = await userService.GetByIdAsync(id);
            return user is not null ? Results.Ok(user) : Results.NotFound();
        })
        .WithTags("Users")
        .WithOpenApi();
    }
}
```

### Blazor Component Structure

```
src/Frontend/Components/Users/UserCard.razor
@using MudBlazor

<MudCard>
    <MudCardContent>
        <MudText Typo="Typo.h6">@User.Name</MudText>
        <MudText Typo="Typo.body2">@User.Email</MudText>
    </MudCardContent>
    <MudCardActions>
        <MudButton Variant="Variant.Text" Color="Color.Primary" OnClick="EditUser">
            Edit
        </MudButton>
    </MudCardActions>
</MudCard>

@code {
    [Parameter] public User User { get; set; } = null!;
    
    private void EditUser()
    {
        // Navigation logic
    }
}
```

## 🗄️ Database Development

### Adding New Entities

1. **Create Entity**
   ```csharp
   // src/Api/Domain/Entities/Product.cs
   public class Product
   {
       public int Id { get; set; }
       public string Name { get; set; } = string.Empty;
       public decimal Price { get; set; }
       public DateTime CreatedAt { get; set; }
   }
   ```

2. **Update DbContext**
   ```csharp
   // src/Api/Data/ApplicationDbContext.cs
   public DbSet<Product> Products { get; set; }
   
   protected override void OnModelCreating(ModelBuilder builder)
   {
       base.OnModelCreating(builder);
       
       builder.Entity<Product>(entity =>
       {
           entity.Property(e => e.Name).HasMaxLength(200);
           entity.Property(e => e.Price).HasPrecision(18, 2);
       });
   }
   ```

3. **Create Migration**
   ```powershell
   cd src/Api.Migrations
   dotnet ef migrations add AddProduct
   ```

4. **Restart Application**
   ```powershell
   # Migrations run automatically
   aspire run
   ```

### Database Best Practices

- **Always use migrations** for schema changes
- **Use configurations** for complex entity setups
- **Seed important data** in DbSeeder.cs
- **Use value converters** for enums and complex types
- **Index frequently queried columns**

### Sample Entity Configuration

```csharp
// src/Api/Data/Configurations/ProductConfiguration.cs
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(p => p.Price)
            .HasPrecision(18, 2);
            
        builder.HasIndex(p => p.Name);
        
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
```

## 🎨 UI Development

### MudBlazor Best Practices

1. **Use MudBlazor Components**
   ```razor
   <MudDataGrid Items="@products" Filterable="true" SortMode="SortMode.Multiple">
       <Columns>
           <PropertyColumn Property="x => x.Name" Title="Product Name" />
           <PropertyColumn Property="x => x.Price" Title="Price" Format="C" />
       </Columns>
   </MudDataGrid>
   ```

2. **Consistent Styling**
   ```razor
   <MudContainer MaxWidth="MaxWidth.Large">
       <MudPaper Class="pa-6">
           <MudText Typo="Typo.h4" GutterBottom="true">
               Your Page Title
           </MudText>
           <!-- Your content -->
       </MudPaper>
   </MudContainer>
   ```

3. **Form Validation**
   ```razor
   <EditForm Model="@model" OnValidSubmit="@HandleSubmit">
       <DataAnnotationsValidator />
       <MudValidationSummary />
       
       <MudTextField @bind-Value="model.Name" 
                     Label="Name" 
                     Required="true"
                     For="@(() => model.Name)" />
                     
       <MudButton ButtonType="ButtonType.Submit" 
                  Variant="Variant.Filled" 
                  Color="Color.Primary">
           Save
       </MudButton>
   </EditForm>
   ```

### Authentication in Components

```razor
@attribute [Authorize]
@inject AuthenticationStateProvider AuthenticationStateProvider

<AuthorizeView>
    <Authorized>
        <p>Hello, @context.User.Identity?.Name!</p>
        <!-- Authenticated content -->
    </Authorized>
    <NotAuthorized>
        <MudButton Href="/login" Color="Color.Primary">
            Please log in
        </MudButton>
    </NotAuthorized>
</AuthorizeView>
```

## 🔌 API Development

### Endpoint Pattern

```csharp
public class CreateUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users", async (CreateUserRequest request, 
                                   IUserService userService,
                                   IValidator<CreateUserRequest> validator) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            
            var user = await userService.CreateAsync(request);
            return Results.Created($"/users/{user.Id}", user);
        })
        .WithTags("Users")
        .WithOpenApi()
        .Produces<User>(StatusCodes.Status201Created)
        .ProducesValidationProblem();
    }
}
```

### Service Registration

```csharp
// src/Api/Program.cs
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
```

### Error Handling

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "An error occurred",
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}
```

## 🧪 Testing Strategy

### Integration Tests with Playwright

The project includes comprehensive integration tests using Playwright for browser automation with MCP (Model Context Protocol) support for AI-driven testing.

#### Setting Up Playwright Tests

1. **Install Playwright Browsers** (first time only):
   ```powershell
   # After building the test project
   dotnet build test/Integration.Test
   
   # Install browsers
   pwsh test/Integration.Test/bin/Debug/net9.0/playwright.ps1 install
   ```

2. **Running Integration Tests**:
   ```powershell
   # Run all integration tests
   dotnet test test/Integration.Test
   
   # Run specific test
   dotnet test test/Integration.Test --filter "DisplayName~HomePage"
   
   # Run with detailed output
   dotnet test test/Integration.Test --logger "console;verbosity=detailed"
   ```

#### Writing Playwright Tests

```csharp
// test/Integration.Test/ExampleIntegrationTest.cs
public class ExampleIntegrationTest(AspireFixture fixture) : IClassFixture<AspireFixture>
{
    [Fact(DisplayName = "Should navigate to user page and display users")]
    public async Task UserPage_Should_Display_Users()
    {
        // Arrange
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions 
        { 
            Headless = true,
            SlowMo = 100 // Slow down for debugging
        });
        
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        });
        
        var page = await context.NewPageAsync();

        // Get the frontend URL from AspireFixture
        var frontendUrl = fixture.WebsiteClient.BaseAddress?.ToString() ?? "/";
        
        // Act
        await page.GotoAsync(frontendUrl);
        await page.ClickAsync("text=Users");
        await page.WaitForSelectorAsync("[data-testid='user-list']");
        
        // Assert
        var userCount = await page.Locator("[data-testid='user-item']").CountAsync();
        Assert.True(userCount > 0, "Should display at least one user");
        
        var pageTitle = await page.TextContentAsync("h1");
        Assert.Equal("Users", pageTitle);
    }
}
```

#### Best Practices for Playwright Tests

1. **Use Data Test IDs**:
   ```razor
   <!-- In your Blazor components -->
   <MudButton data-testid="save-button" OnClick="SaveUser">Save</MudButton>
   <MudDataGrid data-testid="user-list" Items="@users">
       <!-- User items with data-testid="user-item" -->
   </MudDataGrid>
   ```

2. **Wait for Elements**:
   ```csharp
   // Wait for specific elements
   await page.WaitForSelectorAsync("[data-testid='loading-spinner']", new PageWaitForSelectorOptions 
   { 
       State = WaitForSelectorState.Hidden 
   });
   
   // Wait for network idle
   await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
   ```

3. **Handle Authentication in Tests**:
   ```csharp
   // Login helper method
   private async Task LoginAsync(IPage page, string email, string password)
   {
       await page.GotoAsync($"{fixture.WebsiteClient.BaseAddress}/login");
       await page.FillAsync("[data-testid='email-input']", email);
       await page.FillAsync("[data-testid='password-input']", password);
       await page.ClickAsync("[data-testid='login-button']");
       await page.WaitForURLAsync("**/dashboard");
   }
   ```

4. **Test Data Management**:
   ```csharp
   public class UserManagementTests(AspireFixture fixture) : IClassFixture<AspireFixture>
   {
       [Fact]
       public async Task CreateUser_Should_Add_User_To_List()
       {
           // Arrange - Create test data via API
           var apiClient = fixture.CreateClient("api");
           var testUser = new CreateUserRequest { Name = "Test User", Email = "test@example.com" };
           
           // Act - Use browser to create user via UI
           var playwright = await Playwright.CreateAsync();
           await using var browser = await playwright.Chromium.LaunchAsync();
           var page = await browser.NewPageAsync();
           
           // Navigate and interact with UI
           await page.GotoAsync($"{fixture.WebsiteClient.BaseAddress}/users/create");
           await page.FillAsync("[data-testid='name-input']", testUser.Name);
           await page.FillAsync("[data-testid='email-input']", testUser.Email);
           await page.ClickAsync("[data-testid='save-button']");
           
           // Assert - Verify via both UI and API
           await page.WaitForURLAsync("**/users");
           var userExists = await page.IsVisibleAsync($"text={testUser.Name}");
           Assert.True(userExists);
           
           // Verify via API
           var response = await apiClient.GetAsync("/api/users");
           var users = await response.Content.ReadFromJsonAsync<List<User>>();
           Assert.Contains(users, u => u.Email == testUser.Email);
       }
   }
   ```

#### MCP Integration for AI-Driven Testing

The project supports MCP (Model Context Protocol) for AI-assisted test development:

1. **Use AI to Generate Tests**:
   - Describe the functionality you want to test
   - AI can generate Playwright test code using the project patterns
   - AI understands the AspireFixture setup and can create appropriate test scenarios

2. **AI-Assisted Debugging**:
   - Provide test failure screenshots or error messages
   - AI can suggest fixes based on the Playwright APIs and project structure
   - AI can help optimize selectors and test strategies

3. **Test Maintenance**:
   - AI can help update tests when UI changes
   - Suggest better selectors and more robust test patterns
   - Help with test data setup and cleanup

#### Debugging Integration Tests

1. **Visual Debugging**:
   ```csharp
   // Run in headed mode for debugging
   await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions 
   { 
       Headless = false,
       SlowMo = 1000,
       Devtools = true
   });
   ```

2. **Screenshots on Failure**:
   ```csharp
   [Fact]
   public async Task TestMethod()
   {
       try
       {
           // Your test code
       }
       catch
       {
           await page.ScreenshotAsync(new PageScreenshotOptions 
           { 
               Path = $"test-failure-{DateTime.Now:yyyyMMdd-HHmmss}.png" 
           });
           throw;
       }
   }
   ```

3. **Video Recording**:
   ```csharp
   var context = await browser.NewContextAsync(new BrowserNewContextOptions
   {
       RecordVideoDir = "test-videos/",
       RecordVideoSize = new RecordVideoSize { Width = 1920, Height = 1080 }
   });
   ```

### Unit Tests Structure

```csharp
// test/Api.Tests/Services/UserServiceTests.cs
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;
    
    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }
    
    [Fact]
    public async Task CreateAsync_ValidUser_ReturnsUser()
    {
        // Arrange
        var request = new CreateUserRequest { Name = "Test User" };
        var user = new User { Id = 1, Name = "Test User" };
        
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                          .ReturnsAsync(user);
        
        // Act
        var result = await _userService.CreateAsync(request);
        
        // Assert
        Assert.Equal(user.Name, result.Name);
    }
}
```

### Integration Tests

```csharp
// test/Integration.Test/UserEndpointsTests.cs
public class UserEndpointsTests : IClassFixture<AspireFixture>
{
    private readonly AspireFixture _fixture;
    
    public UserEndpointsTests(AspireFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task GetUsers_ReturnsOk()
    {
        // Arrange
        var client = _fixture.CreateClient("api");
        
        // Act
        var response = await client.GetAsync("/users");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<User>>();
        Assert.NotNull(users);
    }
}
```

## 🔧 Development Tools

### Debugging

1. **Aspire Dashboard**: Monitor all services and logs
2. **Browser DevTools**: Debug Blazor WebAssembly
3. **DbGate**: Query and inspect database
4. **Scalar**: Test API endpoints

### Hot Reload

- **Blazor**: Automatic UI refresh on component changes
- **API**: Automatic restart on code changes
- **CSS**: Instant styling updates

### Performance Monitoring

```csharp
// Add to Program.cs for detailed monitoring
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();
```

## 📦 Package Management

### Adding New Packages

```powershell
# Add to API project
dotnet add src/Api package Serilog.AspNetCore

# Add to Frontend project  
dotnet add src/Frontend package Blazored.LocalStorage

# Add to all projects
dotnet add src/*/  package Microsoft.Extensions.Logging
```

### Recommended Packages

**API Enhancements:**
- `FluentValidation` - Input validation
- `Serilog` - Structured logging
- `AutoMapper` - Object mapping
- `MediatR` - CQRS pattern

**Frontend Enhancements:**
- `Blazored.LocalStorage` - Browser storage
- `Blazored.Toast` - Notifications
- `ChartJs.Blazor` - Charts and graphs

## 🚀 Performance Optimization

### Entity Framework

```csharp
// Use AsNoTracking for read-only queries
var users = await context.Users
    .AsNoTracking()
    .Where(u => u.IsActive)
    .ToListAsync();

// Use projections for limited data
var userSummaries = await context.Users
    .Select(u => new UserSummary { Id = u.Id, Name = u.Name })
    .ToListAsync();
```

### Blazor WebAssembly

```csharp
// Use virtualization for large lists
<MudVirtualize Items="@largeList" Context="item">
    <ItemContent>
        <UserCard User="@item" />
    </ItemContent>
</MudVirtualize>

// Lazy load components
@page "/users"
@using Microsoft.AspNetCore.Components.Web

<div class="page-loading" style="@LoadingDisplay">
    <MudProgressCircular Indeterminate="true" />
</div>

<div style="@ContentDisplay">
    @if (users != null)
    {
        <UserList Users="@users" />
    }
</div>
```

## 🔄 Deployment Pipeline

### Local Development

```powershell
# Development workflow
git checkout -b feature/new-feature
# Make changes
dotnet test
aspire run
# Verify changes
git commit -m "feat: implement new feature"
git push origin feature/new-feature
```

### Production Deployment

```powershell
# Create production build
aspire publish --output-path ./publish

# Deploy to production
scp -r ./publish/* user@server:/deployment/
ssh user@server "cd /deployment && docker-compose up -d"
```
