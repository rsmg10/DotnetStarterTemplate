namespace Infra.Authentication.Models;

public class SignInResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTimeOffset Expiry { get; set; }
}