using Domain.Interfaces;
using Infra.Authentication.Models;
using Microsoft.AspNetCore.Identity;

namespace Infra.Authentication.Services;

public class CustomEmailTokenProvider(IOtpService otpService, IEmailSender emailSender) : IUserTwoFactorTokenProvider<User>
{
    public Task<string> GenerateAsync(string purpose, UserManager<User> manager, User user)
    {
        // generate and save for validating
        var generateOtp = otpService.GenerateOtp("Email" + user.Id);
        return Task.FromResult(generateOtp);
    }

    public Task<bool> ValidateAsync(string purpose, string token, UserManager<User> manager, User user)
    {
        if (!otpService.ValidateOtp("Email" + user.Id, token))
            return Task.FromResult(false);
        user.EmailConfirmed = true;
        manager.UpdateAsync(user);
        return Task.FromResult(true);
    }

    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user)
    {
        return Task.FromResult(false);
    }
}