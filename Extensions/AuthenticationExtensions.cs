using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Sep490_Eduseen_BE.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration){
        services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters{
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                NameClaimType = JwtRegisteredClaimNames.Sub,
                RoleClaimType = "role"
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx => 
                {
                    ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
                    if(!string.IsNullOrEmpty(accessToken)){
                        ctx.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        })
        //.AddGoogle(options =>
        //{
        //    options.ClientId = configuration["Authentication:Google:ClientId"];
        //    options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
        //    options.CallbackPath = "/signin-google";
        //})
        ;

        return services;
    }
}