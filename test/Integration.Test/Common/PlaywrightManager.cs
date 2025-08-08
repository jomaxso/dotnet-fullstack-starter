using System.Diagnostics;

using Microsoft.Playwright;

namespace Integration.Test.Common;

public class PlaywrightManager : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    private static bool IsDebugging => Debugger.IsAttached;

    private static bool IsHeadless => IsDebugging is false;

    private IPlaywright? _playwright;

    internal IBrowser Browser { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        Assertions.SetDefaultExpectTimeout(DefaultTimeout.Milliseconds);

        _playwright = await Playwright.CreateAsync();

        var options = new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless,
            // Timeout = DefaultTimeout.Milliseconds,
            // Args = new[] { "--disable-web-security", "--disable-features=IsolateOrigins,site-per-process" }
        };

        Browser = await _playwright.Chromium
            .LaunchAsync(options)
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await Browser.CloseAsync();
        _playwright?.Dispose();
    }
}