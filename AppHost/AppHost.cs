using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("dotnet-fullstack-starter")
    .WithDashboard();

var mySql = builder.AddMySql("mysql")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDbGate(x => x.WithLifetime(ContainerLifetime.Persistent))
    .PublishAsConnectionString();

var db = mySql.AddDatabase(Services.Database, "database")
    .WithClearCommand("clear-database", "Clear Database");

var apiMigrations = builder.AddProject<Projects.Api_Migrations>(Services.ApiMigrations)
    .WithReference(db)
    .WaitFor(db);

var api = builder.AddProject<Projects.Api>(Services.Api)
    .WithExternalHttpEndpoints()
    .WithUrlForEndpoint("https", e =>
    {
        e.DisplayText = "API Documentation";
        e.Url = "/scalar";
    })
    .WithReference(db)
    .WaitFor(db)
    .WaitForCompletion(apiMigrations);

builder.AddProject<Projects.Frontend>(Services.Website)
    .WithExternalHttpEndpoints()
    .WithUrlForEndpoint("https", e => e.DisplayText = "Website")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();

internal static class DatabaseResourceBuilderExtensions
{
    public static IResourceBuilder<MySqlDatabaseResource> WithClearCommand(
        this IResourceBuilder<MySqlDatabaseResource> builder, string commandName, string displayName)
    {
        var commandOptions = new CommandOptions
        {
            UpdateState = OnUpdateResourceState,
            IconName = "AnimalRabbitOff",
            IconVariant = IconVariant.Filled
        };

        builder.WithCommand(
            name: commandName,
            displayName: displayName,
            executeCommand: context => OnRunClearDatabaseCommandAsync(builder, context),
            commandOptions: commandOptions);

        return builder;
    }

    private static async Task<ExecuteCommandResult> OnRunClearDatabaseCommandAsync(
        IResourceBuilder<MySqlDatabaseResource> builder,
        ExecuteCommandContext context)
    {
        await Task.Delay(1000, context.CancellationToken);
        return CommandResults.Success();
    }

    private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Updating resource state: {ResourceSnapshot}",
                context.ResourceSnapshot);
        }

        return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
            ? ResourceCommandState.Enabled
            : ResourceCommandState.Disabled;
    }
}