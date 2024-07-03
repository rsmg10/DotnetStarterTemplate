using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Domain.Interfaces;
using OtpNet;

namespace Infra.Tools.Otp;

public class OtpService() : IOtpService
{
    private readonly ConcurrentDictionary<string, (string otp, DateTime expiresAt)> _otpStore = new();

    public string GenerateOtp(string key, int length = 6, int expiryInSeconds = 300)
    {
        var otpKey = Encoding.UTF8.GetBytes(key);
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(otpKey);
        } 
        var totp = new Totp(otpKey, mode: OtpHashMode.Sha1, totpSize: length); 
        var otp = totp.ComputeTotp();

        var expiresAt = DateTime.UtcNow.AddSeconds(expiryInSeconds);
        _otpStore[key] = (otp, expiresAt);

        return otp;
    } 
 
    public bool ValidateOtp(string key, string otp)
    {
        if (_otpStore.TryGetValue(key, out var otpEntry))
        {
            if (otpEntry.expiresAt > DateTime.UtcNow && otpEntry.otp == otp)
            {
                _otpStore.TryRemove(key, out _);
                return true;
            }
        }
        return false;
    }
    
}
 

