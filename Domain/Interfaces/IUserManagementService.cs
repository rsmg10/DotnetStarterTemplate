using Domain.Dto;
using Infra.Authentication.Models;

namespace Infra.Authentication.Services;

public interface IUserManagementService
{
    Task<CoreResponse> SignUp(SignupCommand command);
    Task ConfirmOtp(string email, string otp);
    Task<string> ResendOtp(string email);
    Task<SignInResponse> SignIn(string email, string password);
    Task SignOut();
    Task<CoreResponse> ChangePassword(string email, string oldPassword, string newPassword);
    Task<CoreResponse> ChangeEmail(string email, string newEmail);
    Task<CoreResponse> ChangePhoneNumber(string email, string newPhoneNumber);
    Task<CoreResponse> UpdateProfile( string firstName, string lastName);
    Task<CoreResponse> ConfirmEmail(string email, string token);
    Task<SignInResponse> RefreshAccessToken(string accessToken, string refreshToken);
}