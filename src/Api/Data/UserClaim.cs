using Microsoft.AspNetCore.Identity;

namespace Api.Identity;

public class UserClaim : IdentityUserClaim<string>
{
    public User User { get; set; }
}