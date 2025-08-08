using System.Reflection;

using Api.Data;
using Api.Identity;
using Api.Migrations;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults();

builder.Services.AddHostedService<ApiDbInitializer<ApplicationDbContext>>();

// Identity Services für Seeding hinzufügen
builder.Services.AddIdentity<User, Role>(options =>
{
    // Development-freundliche Passwort-Einstellungen
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddDbContextPool<ApplicationDbContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString(Services.Database);

    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
});

builder.EnrichMySqlDbContext<ApplicationDbContext>();

var host = builder.Build()
    .MapDefaultEndpoints();

host.Run();