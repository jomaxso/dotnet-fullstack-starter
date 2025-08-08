namespace Api;

/// <summary>
/// Defines an interface for API endpoints.
/// This interface is used to map endpoints in the ASP.NET Core routing system.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Maps the endpoint to the specified route builder.
    /// This method is used to register the endpoint with the ASP.NET Core routing system.
    /// It allows the endpoint to be accessible via HTTP requests.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to which the endpoint will be mapped.</param>
    /// <remarks>
    /// This method is typically called during the application's startup configuration.
    /// It ensures that the endpoint is properly registered and can handle incoming requests.
    /// </remarks>
    static abstract void MapEndpoint(IEndpointRouteBuilder endpoints);
}