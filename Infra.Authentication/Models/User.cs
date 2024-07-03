using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Infra.Authentication.Models;


public class User : IdentityUser<Guid>
{
    [MaxLength(100)]
    public string? FirstName { get; set; }
 
    [MaxLength(100)]
    public string? LastName { get; set; }

    public string? RefreshToken { get; set; }
    public DateTimeOffset RefreshTokenExpiry { get; set; }
}