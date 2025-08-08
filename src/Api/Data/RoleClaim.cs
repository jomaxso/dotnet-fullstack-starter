using Microsoft.AspNetCore.Identity;

namespace Api.Identity;

public class RoleClaim : IdentityRoleClaim<string>
{
    public Role Role { get; set; }
}