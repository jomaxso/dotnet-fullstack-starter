namespace Api.Abstractions;

/// <summary>
/// Extension methods for mapping endpoints.
/// This class provides a method to map endpoints that implement the <see cref="IEndpoint"/>
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Maps an endpoint to the specified route builder.
    /// This method is used to register endpoints that implement the <see cref="IEndpoint"/> interface.
    /// It allows for a clean and consistent way to add endpoints to the application.
    /// </summary>
    /// <typeparam name="T">The type of the endpoint to map. It must implement <see cref="IEndpoint"/>.</typeparam>
    /// <param name="endpoints">The endpoint route builder to which the endpoint will be mapped.</param>
    /// <remarks>
    /// This method is typically used in the application's startup configuration to register endpoints.
    /// It ensures that all endpoints implementing the <see cref="IEndpoint"/> interface are properly registered.
    /// </remarks>
    public static IEndpointRouteBuilder WithEndpoint<T>(this IEndpointRouteBuilder endpoints) where T : IEndpoint
    {
        T.MapEndpoint(endpoints);
        return endpoints;
    }
}