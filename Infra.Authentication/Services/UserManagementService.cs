using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Domain.Dto;
using Domain.Interfaces;
using Infra.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infra.Authentication.Services;

public class UserManagementService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    RoleManager<RoleType> roleManager,
    IEmailSender emailSender,
    IOtpService otpService,
    IRequestContext request,
    IConfiguration configuration, 
    ITokenService tokenService) : IUserManagementService
{
    // sign up 
    // sign in 
    // sign out
    // reset password
    // change password
    // change email
    // change phone number
    // update profile
    //https://github.com/rsmg10/DotnetStarterTemplate.git
    public async Task<CoreResponse> SignUp(SignupCommand command)
    {
        
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is not null) throw new Exception("Email Already in Use");

        user = new User
        {
            UserName = Regex.Match(command.Email, @"^([^@]+)@").Value,
            Email = command.Email,
            EmailConfirmed = false,
            LockoutEnabled = true,
            TwoFactorEnabled = false,
            SecurityStamp = Guid.NewGuid().ToString(),
            LockoutEnd = DateTimeOffset.Now.AddSeconds(30),
        };

        await userManager.AddPasswordAsync(user, command.Password);
        var role = await GetRole(Role.Consumer);
        var res = await userManager.CreateAsync(user, command.Password);
        var generateEmailConfirmationTokenAsync = await  userManager.GenerateEmailConfirmationTokenAsync(user);
        await emailSender.SendEmailAsync(user.Email!, "OTP", generateEmailConfirmationTokenAsync);
        var s = await userManager.GetSecurityStampAsync(user);
        // await userManager.CreateAsync(user);
        await userManager.AddToRoleAsync(user, role.Name!);
        return new CoreResponse("User Created Successfully");
    } 
    
    public async Task ConfirmOtp(string email, string otp)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) throw new Exception("User Not Found");
        var confirmEmailAsync = await userManager.ConfirmEmailAsync(user, otp);
        if(!confirmEmailAsync.Succeeded) throw new Exception("Invalid OTP");
    }

    public async Task<string> ResendOtp(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) throw new Exception("User Not Found");
       
        var generateEmailConfirmationTokenAsync = await  userManager.GenerateEmailConfirmationTokenAsync(user);
        await emailSender.SendEmailAsync(user.Email!, "OTP", generateEmailConfirmationTokenAsync);
        return otpService.GenerateOtp(user.Id.ToString());
    }
    
    public async Task<SignInResponse> SignIn(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) throw new Exception("User Not Found");
        if(!await userManager.CheckPasswordAsync(user, password)) throw new Exception();
        var passwordSignin =  await signInManager.PasswordSignInAsync(user, password, false, false);
        if (!passwordSignin.Succeeded) throw new Exception();
        
        var userRoles = await userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }
        var token = tokenService.GenerateAccessToken(authClaims);  
        var generateRefreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = generateRefreshToken;
        user.RefreshTokenExpiry = DateTimeOffset.Now.AddHours(1);
        await userManager.UpdateAsync(user);
       return new SignInResponse
       {
           Token = token.token,
           RefreshToken = generateRefreshToken,
           Expiry = token.expires
       };  
    }
  
    public async Task SignOut()
    {
        await signInManager.SignOutAsync();
    }

    public async Task<CoreResponse> ChangePassword(string email, string oldPassword, string newPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) throw new Exception("User Not Found");
        var res = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        if(!res.Succeeded) throw new Exception();
        return new CoreResponse("Password Changed Successfully");
    }
    
    public async Task<CoreResponse> ChangeEmail(string email, string newEmail)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) throw new Exception("User Not Found");
        user.EmailConfirmed = false;
        var confirmationToken =  await userManager.GenerateEmailConfirmationTokenAsync(user);
        var result =  await userManager.SetEmailAsync(user, email);
        if(!result.Succeeded) throw new Exception();
        await emailSender.SendEmailAsync(user.Email!, "OTP", confirmationToken);
        return new CoreResponse("Email Changed Successfully");
    }
    
    public async Task<CoreResponse> ConfirmEmail(string email, string token)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) throw new Exception("User Not Found");
        user.EmailConfirmed = false;
        var result =  await userManager.ConfirmEmailAsync(user, token);
        if(!result.Succeeded) throw new Exception();
        return new CoreResponse("Email Confirmed Successfully");
    }

    public async Task<SignInResponse> RefreshAccessToken(string accessToken, string refreshToken)
    {
        var principle = tokenService.GetPrincipalFromExpiredToken(accessToken);
        var username = principle.Claims.First(x => x.Type == ClaimTypes.Name).Value;
        if (string.IsNullOrEmpty(username))
            throw new ArgumentException("invalid token");
        var user = await userManager.Users.FirstOrDefaultAsync(x =>
            x.UserName!.Equals(username, StringComparison.OrdinalIgnoreCase) && x.RefreshToken!.Equals(refreshToken) && x.RefreshTokenExpiry > DateTimeOffset.Now);
        if (user is null) throw new Exception("User Not Found, Sign in Again");
        var token = tokenService.GenerateAccessToken(principle.Claims);
        refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTimeOffset.Now.AddHours(1);
        await userManager.UpdateAsync(user);
        return new SignInResponse
        {
            Token = token.token,
            RefreshToken = refreshToken,
            Expiry = token.expires
        };
    }

    public async Task<CoreResponse> ChangePhoneNumber(string email, string newPhoneNumber)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) throw new Exception("User Not Found");
        var res = await userManager.SetPhoneNumberAsync(user, newPhoneNumber);
        if(!res.Succeeded) throw new Exception();
        return new CoreResponse("Phone Number Changed Successfully");
    }

    public async Task<CoreResponse> UpdateProfile(string firstName, string lastName)
    {
        var user = await userManager.FindByIdAsync(request.GetId().ToString());
        if (user is null) throw new Exception("User Not Found");
        user.FirstName = firstName;
        user.LastName = lastName;
        var result =  await userManager.UpdateAsync(user);
        if(!result.Succeeded) throw new Exception();
        return new CoreResponse("Profile Updated Successfully");
    }
    private async Task<RoleType> GetRole(Role consumer)
    {
        var role = await roleManager.FindByNameAsync(consumer.ToString());
        if (role is null)
        {
            role = new RoleType
            {
                Name = consumer.ToString()
            };
            
            await roleManager.CreateAsync(role);
        }
        return role;
    }
}