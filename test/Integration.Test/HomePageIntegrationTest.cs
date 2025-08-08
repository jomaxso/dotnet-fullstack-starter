using Microsoft.Playwright;

namespace Integration.Test;

public class HomePageIntegrationTest(AspireFixture fixture) : IClassFixture<AspireFixture>
{
    [Fact(DisplayName = "Home page should display 'Hello, world!'")]
    public async Task HomePage_Should_Display_HelloWorld()
    {
        // Arrange
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Use the AspireFixture to get the WebApp2 URL
        var webAppUrl = fixture.WebsiteClient.BaseAddress?.ToString() ?? "/";
        await page.GotoAsync(webAppUrl);

        // Act
        var h1Text = await page.TextContentAsync("h1");

        // Assert
        Assert.Equal("Hello, world!", h1Text);
    }
}