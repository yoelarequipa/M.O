using Microsoft.AspNetCore.Identity;

namespace MarmolesOeste.Web.Data;

public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;
}
