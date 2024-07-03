using System.Text;
using Infra.Authentication.Models;
using Infra.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infra.Authentication;

public static class ServiceRegistration
{
    public static IdentityBuilder AddUserAuth(this IServiceCollection services, IConfiguration configuration)
    { 
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddTransient<ITokenService, TokenService>();
        
        services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer
            .AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters() {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            });

        services.AddScoped<CustomEmailTokenProvider>();
        services.AddScoped<IUserTwoFactorTokenProvider<User>, CustomEmailTokenProvider>();
       return  services.AddIdentity<User, RoleType>(x =>
        {
            x.Lockout = new LockoutOptions()
            {
                AllowedForNewUsers = true,
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1),
                MaxFailedAccessAttempts = 3
            };
            x.User = new UserOptions()
            {
                RequireUniqueEmail = true,
                AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+",
            };
            x.Password = new PasswordOptions()
            {
                RequiredLength = 6,
                RequireNonAlphanumeric = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // x.Stores = new StoreOptions()
            // {
            //     SchemaVersion = new Version("1.0.0.0"),
            //     ProtectPersonalData = true,
            //     MaxLengthForKeys = 128
            // };
            x.ClaimsIdentity = new ClaimsIdentityOptions() { };
            x.Tokens = new TokenOptions()
            {
            //     AuthenticatorIssuer = "",
            //     AuthenticatorTokenProvider = "",
              ChangeEmailTokenProvider = "email",
            EmailConfirmationTokenProvider = "email",
            //     PasswordResetTokenProvider = "",
            //     ChangePhoneNumberTokenProvider = "",
            ProviderMap = new Dictionary<string, TokenProviderDescriptor>()
            {
                // { "", new TokenProviderDescriptor(typeof(TokenProviderDescriptor)) },
                { "email", new TokenProviderDescriptor(typeof(CustomEmailTokenProvider)) } 
            },
            };
            x.SignIn = new SignInOptions()
            {
                RequireConfirmedEmail = true,
                // RequireConfirmedPhoneNumber = true,
                // RequireConfirmedAccount = true
            };
        });
       
       

    }
}