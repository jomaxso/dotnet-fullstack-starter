using System.Diagnostics;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Api.Migrations;

public class ApiDbInitializer<TContext>(
    IServiceProvider serviceProvider,
    IHostEnvironment hostEnvironment,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService where TContext : DbContext
{
    private readonly ActivitySource _activitySource = new(hostEnvironment.ApplicationName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity(hostEnvironment.ApplicationName, ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            await EnsureDatabaseAsync(dbContext, cancellationToken);
            await RunMigrationAsync(dbContext, cancellationToken);

            // Seed development data
            await SeedDevelopmentDataAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(TContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(dbContext, static async (ctx, ct) =>
        {
            var dbCreator = ctx.GetService<IRelationalDatabaseCreator>();
            var logger = ctx.GetService<ILogger<ApiDbInitializer<TContext>>>();

            logger.LogInformation("Ensuring database exists...");

            if (await dbCreator.ExistsAsync(ct) is false)
            {
                logger.LogInformation("Creating database...");
                await dbCreator.CreateAsync(ct);
            }

            logger.LogInformation("Database exists.");
        }, cancellationToken);
    }

    private static async Task RunMigrationAsync(TContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(dbContext, static async (ctx, ct) =>
        {
            var logger = ctx.GetService<ILogger<ApiDbInitializer<TContext>>>();

            logger.LogInformation("Checking for pending migrations...");

            var migrations = await ctx.Database.GetPendingMigrationsAsync(ct);

            if (migrations.Any() is false)
            {
                logger.LogInformation("No pending migrations found. Database is up to date.");
                return;
            }

            logger.LogInformation("Pending migrations found. Applying migrations...");
            await using var transaction = await ctx.Database.BeginTransactionAsync(ct);
            await ctx.Database.MigrateAsync(ct);
            await transaction.CommitAsync(ct);

            logger.LogInformation("Migrations applied successfully.");
        }, cancellationToken);
    }

    private async Task SeedDevelopmentDataAsync(CancellationToken cancellationToken)
    {
        await DbSeeder.SeedDevelopmentDataAsync(serviceProvider, hostEnvironment, cancellationToken);
    }


}