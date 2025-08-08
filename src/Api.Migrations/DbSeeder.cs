using Api.Data;
using Api.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Migrations;

public static class DbSeeder
{
    public static async Task SeedDevelopmentDataAsync(
        IServiceProvider serviceProvider,
        IHostEnvironment hostEnvironment,
        CancellationToken cancellationToken = default)
    {
        // Seeding nur in Development-Umgebung
        if (!hostEnvironment.IsDevelopment())
            return;

        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiDbInitializer<ApplicationDbContext>>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Api.Identity.User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        logger.LogInformation("Starting development data seeding...");

        try
        {
            await SeedRolesAsync(roleManager, logger, cancellationToken);
            await SeedUsersAsync(userManager, logger, cancellationToken);

            logger.LogInformation("Development data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during development data seeding");
            throw;
        }
    }

    private static async Task SeedRolesAsync(RoleManager<Role> roleManager, ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding roles...");

        var roles = new[]
        {
            "Administrator",
            "ProductManager",
            "Developer",
            "Viewer",
            "QualityAssurance"
        };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new Role
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Created role: {RoleName}", roleName);
                }
                else
                {
                    logger.LogWarning("Failed to create role {RoleName}: {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<Api.Identity.User> userManager, ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding users...");

        var users = new[]
        {
            new { Email = "admin@my-company.dev", UserName = "admin@my-company.dev", Password = "Admin123!", Roles = new[] { "Administrator" } },
            new { Email = "pm@my-company.dev", UserName = "pm@my-company.dev", Password = "PM123!", Roles = new[] { "ProductManager" } },
            new { Email = "dev@my-company.dev", UserName = "dev@my-company.dev", Password = "Dev123!", Roles = new[] { "Developer" } },
            new { Email = "qa@my-company.dev", UserName = "qa@my-company.dev", Password = "QA123!", Roles = new[] { "QualityAssurance" } },
            new { Email = "viewer@my-company.dev", UserName = "viewer@my-company.dev", Password = "View123!", Roles = new[] { "Viewer" } }
        };

        foreach (var userData in users)
        {
            var existingUser = await userManager.FindByEmailAsync(userData.Email);
            if (existingUser == null)
            {
                var user = new Api.Identity.User
                {
                    UserName = userData.UserName,
                    Email = userData.Email,
                    EmailConfirmed = true,
                    Domain = "development"
                };

                var result = await userManager.CreateAsync(user, userData.Password);
                if (result.Succeeded)
                {
                    logger.LogInformation("Created user: {Email}", userData.Email);

                    // Rollen zuweisen
                    foreach (var role in userData.Roles)
                    {
                        await userManager.AddToRoleAsync(user, role);
                        logger.LogInformation("Added user {Email} to role {Role}", userData.Email, role);
                    }
                }
                else
                {
                    logger.LogWarning("Failed to create user {Email}: {Errors}",
                        userData.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}