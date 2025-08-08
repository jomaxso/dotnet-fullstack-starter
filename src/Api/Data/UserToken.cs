using Microsoft.AspNetCore.Identity;

namespace Api.Identity;

public class UserToken : IdentityUserToken<string>
{
    public User User { get; set; }
}