using System.Net.Http.Json;

// using Api.Features;


namespace Integration.Test;

public class IntegrationTest1(AspireFixture fixture)
{

    // [Fact(DisplayName = "Integration Test 1")]
    // public async Task Test1Async()
    // {
    //     // Arrange
    //     var client = fixture.ApiClient;
    //
    //     // Act
    //     var result = await client.GetFromJsonAsync<GetFirstActivityLogEntry.Response>(
    //         "api/first",
    //         TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(result);
    // }
    //
    //  [Fact(DisplayName = "Integration Test 2")]
    // public async Task Test2Async()
    // {
    //     // Arrange
    //     var client = fixture.ApiClient;
    //
    //     // Act
    //     var result = await client.GetFromJsonAsync<GetSecondActivityLogEntry.Response>(
    //         "api/second",
    //         TestContext.Current.CancellationToken);
    //
    //     // Assert
    //     Assert.NotNull(result);
    // }
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