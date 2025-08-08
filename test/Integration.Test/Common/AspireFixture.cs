using Aspire.Hosting;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

[assembly: AssemblyFixture(typeof(AspireFixture))]

namespace Integration.Test.Common;

public class AspireFixture : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(2);
    // internal PlaywrightManager Playwright { get; } = new PlaywrightManager();

    private DistributedApplication App { get; set; } = null!;
    public HttpClient WebsiteClient { get; private set; } = null!;
    public HttpClient ApiClient { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        var cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
              new CancellationTokenSource(DefaultTimeout).Token,
              TestContext.Current.CancellationToken).Token;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(cancellationToken);

        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });

        appHost.Services.ConfigureHttpClientDefaults(client =>
            client.AddStandardResilienceHandler());

        App = await appHost.BuildAsync(cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        await App.StartAsync(cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        WebsiteClient = App.CreateHttpClient(Services.Website);
        await App.ResourceNotifications.WaitForResourceHealthyAsync(
            Services.Website, cancellationToken);

        ApiClient = App.CreateHttpClient(Services.Api);
        await App.ResourceNotifications.WaitForResourceHealthyAsync(
            Services.Api, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await App.DisposeAsync();
    }

    // Add any shared methods or properties that can be used across multiple tests.
}


// public abstract class PlaywrightTestBase : IAsyncDisposable
// {


//     DistributedApplication App => AspireManager.App;
//     PlaywrightManager PlaywrightManager => AspireManager.PlaywrightManager;

//     public string? DashboardUrl { get; private set; }

//     public string DashboardLoginToken { get; private set; } = string.Empty;

//     private IBrowserContext? _browserContext;

//     public Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
//         string[]? args = null,
//         Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class
//     {
//         return AspireManager.ConfigureAsync<TEntryPoint>(args, builder =>
//         {
//             var aspNetCoreUrls = builder.Configuration["ASPNETCORE_URLS"];
//             var urls = aspNetCoreUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? [];

//             DashboardUrl = urls.FirstOrDefault();
//             DashboardLoginToken = builder.Configuration["AppHost:BrowserToken"] ?? string.Empty;

//             configureBuilder?.Invoke(builder);
//         });
//     }

//     public async Task InteractWithPageAsync(string serviceName,
//         Func<IPage, Task> test,
//         ViewportSize? viewportSize = null,
//         CancellationToken cancellationToken = default)
//     {
//         var token = CancellationTokenSource.CreateLinkedTokenSource(
//             AspireManager.ExpirationToken,
//             cancellationToken);

//         var urlSought = string.IsNullOrEmpty(serviceName)
//             ? new Uri(DashboardUrl)
//             : AspireManager.App.GetEndpoint(serviceName);

//         if (urlSought is null)
//         {
//             throw new InvalidOperationException($"Service '{serviceName}' not found in the application.");
//         }

//         await AspireManager.App.ResourceNotifications.WaitForResourceHealthyAsync(
//             serviceName,
//             token.Token).ConfigureAwait(false);

//         _browserContext ??= await AspireManager.PlaywrightManager.CreateBrowserContextAsync(viewportSize, token.Token).ConfigureAwait(false);
//     }

//     public ValueTask DisposeAsync()
//     {
//         throw new NotImplementedException();
//     }
// }