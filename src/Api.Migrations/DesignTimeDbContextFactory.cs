using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Api.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Migrations;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
file sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private const string DummyConnectionString = "Server=localhost;Port=38085;Database=database;Uid=root;Pwd=;";

    // Connection String für Design-Time - sollte mit der echten Container-Konfiguration übereinstimmen
    private static readonly Version FallbackVersion = new(11, 3, 2);

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        Console.WriteLine(args.Length > 0
            ? $"DesignTimeDbContextFactory called with arguments: {string.Join(", ", args)}"
            : "DesignTimeDbContextFactory called without arguments");

        var serverVersion = GetServerVersion(DummyConnectionString);

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseMySql(
            DummyConnectionString,
            serverVersion,
            b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)
        );

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static ServerVersion GetServerVersion(string connectionString)
    {
        try
        {
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            Console.WriteLine($"Detected MySQL version: {serverVersion}");

            return serverVersion;
        }
        catch (Exception ex)
        {
            // var serverVersion = new MySqlServerVersion("11.3.2-mariadb");
            var serverVersion = new MySqlServerVersion(FallbackVersion);
            Console.WriteLine($"Could not detect MySQL version ({ex.Message}), using fallback: {serverVersion}");

            return serverVersion;
        }
    }
}