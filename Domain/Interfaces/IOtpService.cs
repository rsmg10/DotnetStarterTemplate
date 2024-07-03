namespace Domain.Interfaces;

public interface IOtpService
{
    public string GenerateOtp(string key, int length = 6, int expiryInSeconds = 300);
    public bool ValidateOtp(string key, string otp);
}