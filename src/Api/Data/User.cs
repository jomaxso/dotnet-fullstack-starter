using Microsoft.AspNetCore.Identity;

namespace Api.Identity;

public class User : IdentityUser
{
    /// <summary>
    /// Organisationsdomäne (z.B. GMBH, INC, PTY, LLC, BAUDIS)
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// Zeitstempel der letzten Aktualisierung der Berechtigungen
    /// </summary>
    public DateTime LastPermissionUpdate { get; set; } = DateTime.UtcNow;

    public ICollection<UserClaim> Claims { get; set; } = [];

    public ICollection<UserRole> UserRoles { get; set; } = [];

    public ICollection<UserLogin> UserLogins { get; set; } = [];

    public ICollection<UserToken> UserTokens { get; set; } = [];
}