using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;

using Frontend;
using Frontend.Identity;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<CookieDelegatingHandler>();

builder.Services.AddHttpClient(CookieAuthenticationStateProvider.AuthenticatedClientName, opt =>
    opt.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:7208"))
        .AddHttpMessageHandler<CookieDelegatingHandler>();

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.Configuration["FrontendUrl"] ?? "https://localhost:5002") });

// .AddHttpMessageHandler<CookieDelegatingHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthenticatedClient"));

builder.Services.AddMudServices();

await builder.Build().RunAsync();

