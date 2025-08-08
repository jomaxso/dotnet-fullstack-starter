using Microsoft.AspNetCore.Identity;

namespace Api.Identity;

public class UserLogin : IdentityUserLogin<string>
{
    public User User { get; set; }
}