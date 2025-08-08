namespace Api.Domain.Common;

/// <summary>
/// Represents a base class for domain entities with a generic identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier for the domain entity.</typeparam>
public abstract class DomainEntity<TId> : DomainEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public abstract TId Id { get; protected init; }
}

/// <summary>
/// Represents a base class for domain entities that maintains common properties.
/// </summary>
public abstract class DomainEntity
{
    /// <summary>
    /// Represents the timestamp indicating when the entity or object was created.
    /// This property is initialized with the current UTC date and time during object creation
    /// and cannot be modified thereafter.
    /// </summary>
    public DateTime Created { get; private init => field = value.ToUniversalTime(); } = DateTime.UtcNow;

    /// <summary>
    /// Represents the timestamp indicating when the entity or object was last modified.
    /// This property is updated whenever the entity is modified and reflects the current UTC date and time
    /// at the time of modification.
    /// </summary>
    public DateTime Changed { get; set => field = value.ToUniversalTime(); }
}