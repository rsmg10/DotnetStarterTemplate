using Domain.Dto;
using Infra.Authentication.Models;
using Infra.Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthenticationController(IUserManagementService userManagementService) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<CoreResponse> SignUp(SignupCommand command)
    {
        return await userManagementService.SignUp(command);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<string> ResendOtp(string email)
    {
        return await userManagementService.ResendOtp(email);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ContentResponse<SignInResponse>> SignIn(string email, string password)
    {
        try
        {
            var res =  await userManagementService.SignIn(email, password);
            return new ContentResponse<SignInResponse>(res);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost]
    public async Task ConfirmOtp(string email, string otp)
    {
        await userManagementService.ConfirmOtp(email, otp);
    }

    [HttpPost]
    public  async Task SignOut()
    {
        await userManagementService.SignOut();
    }

    [HttpPost]
    public async Task<CoreResponse> ChangePassword(string email, string oldPassword, string newPassword)
    {
        return await userManagementService.ChangePassword(email, oldPassword, newPassword);
    }

    [HttpPost]
    public async Task<CoreResponse> ChangeEmail(string email, string newEmail)
    {
        return await userManagementService.ChangeEmail(email, newEmail);
    }

    [HttpPost]
    public async Task<CoreResponse> ChangePhoneNumber(string email, string newPhoneNumber)
    {
        return await userManagementService.ChangePhoneNumber(email, newPhoneNumber);
    }

    [HttpPost]
    public async Task<CoreResponse> UpdateProfile(string firstName, string lastName)
    {
        return await userManagementService.UpdateProfile(firstName, lastName);
    }
     
    [HttpPost]
    public async Task<SignInResponse> RefreshAccessToken(string accessToken, string refreshToken)
    {
        return await userManagementService.RefreshAccessToken(accessToken, refreshToken);
    }

    [HttpPost]
    public async Task<CoreResponse> ConfirmEmail(string email, string token)
    {
        return await userManagementService.ConfirmEmail(email, token);
    }
    
}