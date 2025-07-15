using Microsoft.AspNetCore.Identity;

namespace Data.SQL.Entities;

public class User : IdentityUser
{
    public string Name { get; set; }

    public string LastName { get; set; }

    public List<string> Roles { get; set; }
}
