using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Frontend.Identity;

public interface IAccountManagement
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);

    Task MarkUserAsLoggedOutAsync(CancellationToken cancellationToken = default);
}

public sealed class CookieAuthenticationStateProvider(
    IHttpClientFactory clientFactory,
    ILocalStorageService localStorage,
    ILogger<CookieAuthenticationStateProvider> logger)
        : AuthenticationStateProvider, IAccountManagement
{
    private const string SessionKey = "UserSession";

    public const string AuthenticatedClientName = "AuthenticatedClient";

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly HttpClient _httpClient = clientFactory.CreateClient(AuthenticatedClientName);

    private bool authenticated = false;

    private readonly ClaimsPrincipal _unauthenticated = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        authenticated = false;
        
        var user = _unauthenticated;

        try
        {
            // TODO: https://github.com/dotnet/blazor-samples/blob/main/9.0/BlazorWebAssemblyStandaloneWithIdentity/BlazorWasmAuth/Identity/CookieAuthenticationStateProvider.cs

            var userInfo = await localStorage.GetItemAsync<UserInfo>(SessionKey);

            if (userInfo is { IsExpired: false })
            {
                var currentIdentity = CreateClaimsIdentity(userInfo);
                var currentPrincipal = new ClaimsPrincipal(currentIdentity);

                return new AuthenticationState(currentPrincipal);
            }

            // the user info endpoint is secured, so if the user isn't logged in this will fail
            using var userResponse = await _httpClient.GetAsync("api/manage/user");

            // throw if user info wasn't retrieved
            userResponse.EnsureSuccessStatusCode();

            var newUserInfo = await userResponse.Content.ReadFromJsonAsync<UserInfo>(_jsonSerializerOptions);

            if (newUserInfo is null || newUserInfo.IsExpired)
            {
                logger.LogWarning("User info cannot be retrieved or is expired when fetching authentication state.");
                await localStorage.RemoveItemAsync(SessionKey);
                return new AuthenticationState(_unauthenticated);
            }

            // using var rolesResponse = await _httpClient.GetAsync("api/roles");

            // rolesResponse.EnsureSuccessStatusCode();

            // var roles = await rolesResponse.Content.ReadFromJsonAsync<RoleClaim[]>(_jsonSerializerOptions);

            // if (roles?.Length > 0)
            // {
            //     foreach (var role in roles)
            //     {
            //         if (!string.IsNullOrEmpty(role.Type) && !string.IsNullOrEmpty(role.Value))
            //         {
            //             claims.Add(new Claim(role.Type, role.Value, role.ValueType, role.Issuer, role.OriginalIssuer));
            //         }
            //     }
            // }

            await localStorage.SetItemAsync(SessionKey, newUserInfo);
            var newIdentity = CreateClaimsIdentity(newUserInfo);

            var newPrincipal = new ClaimsPrincipal(newIdentity);

            return new AuthenticationState(newPrincipal);
        }
        catch (Exception ex) when (ex is HttpRequestException { StatusCode: HttpStatusCode.Unauthorized })
        {
            logger.LogError(ex, "Unauthorized access when fetching authentication state.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error fetching the authentication state.");
        }

        await localStorage.RemoveItemAsync(SessionKey);
        return new AuthenticationState(_unauthenticated);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/login?useCookies=true", new
            {
                email = request.Email,
                password = request.Password
            }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return new LoginResponse();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login failed for unknown reasons");
        }

        return new LoginResponse("Invalid email and/or password.");
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        await localStorage.RemoveItemAsync(SessionKey, cancellationToken);
        var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("api/manage/logout", emptyContent, cancellationToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOutAsync(CancellationToken cancellationToken = default)
    {
        await localStorage.RemoveItemAsync(SessionKey, cancellationToken);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_unauthenticated)));
    }

    private static ClaimsIdentity CreateClaimsIdentity(UserInfo user)
    {
        var claims = user.Roles
            .Select(role => new Claim(ClaimTypes.Role, role))
            .Prepend(new(ClaimTypes.Name, user.UserName))
            .Prepend(new(ClaimTypes.NameIdentifier, user.Id))
            .Prepend(new(ClaimTypes.Email, user.Email));

        if (!string.IsNullOrEmpty(user.Domain))
        {
            claims = claims.Prepend(new("domain", user.Domain));
        }

        return new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
    }
}

public sealed record UserInfo
{
    public required string Id { get; init; }
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required string? Domain { get; init; }
    public List<string> Roles { get; init; } = [];
    public DateTime ExpiryTimeStamp { get; init; } = DateTime.UtcNow.AddMinutes(60);
    public bool IsExpired => ExpiryTimeStamp < DateTime.UtcNow;
}

public sealed record LoginRequest(string Email, string Password);

public record LoginResponse(params IReadOnlyList<string> Errors)
{
    public bool Succeeded => Errors.Any() is false;
}

public sealed class CookieDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);

        var response = await base.SendAsync(request, cancellationToken);

        // if (response.StatusCode == HttpStatusCode.Unauthorized)
        // {
        //     await accountManagement.MarkUserAsLoggedOutAsync(cancellationToken);
        // }

        return response;
    }
}