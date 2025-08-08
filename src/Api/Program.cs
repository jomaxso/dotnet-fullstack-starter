using System.Security.Claims;

using Api.Data;
using Api.Identity;

using Microsoft.AspNetCore.Identity;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Configure the cookie settings here if needed
    options.Cookie.Name = "auth_token";
    options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

// TODO: Authorization Policies
builder.Services.AddAuthorizationBuilder();

builder.AddMySqlDbContext<ApplicationDbContext>(Services.Database);

builder.Services.AddIdentityCore<User>()
    .AddRoles<Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

builder.Services.AddCors(
    options => options.AddPolicy(
        "wasm",
        policy => policy.WithOrigins(["https://localhost:7208", "https://localhost:7152"])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

builder.Services.AddOpenApi();

var app = builder.Build()
    .MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // // seed the database
    // await using var scope = app.Services.CreateAsyncScope();
    // await SeedData.InitializeAsync(scope.ServiceProvider);

    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("wasm");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("api");

api.MapIdentityApi<User>();

var accountGroup = api.MapGroup("/manage")
    .RequireAuthorization();

accountGroup.MapPost("/logout", async (SignInManager<User> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
});

accountGroup.MapGet("/User", (ClaimsPrincipal user) =>
{
    var userInfo = new
    {
        Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
        Email = user.FindFirst(ClaimTypes.Email)?.Value,
        UserName = user.FindFirst(ClaimTypes.Name)?.Value,
        Domain = user.FindFirst("domain")?.Value,
        Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value),
        ExpiryTimeStamp = DateTime.UtcNow.AddMinutes(5) // Placeholder for actual expiry logic
    };

    return TypedResults.Json(userInfo);
}).RequireAuthorization();

app.Run();