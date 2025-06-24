using Sep490_Eduseen_BE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Profiles;
using Sep490_Eduseen_BE.Repositories.impl;
using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Services;
using System.Text;
using Sep490_Eduseen_BE.Extensions;
using Sep490_Eduseen_BE.Hubs;

namespace Sep490_Eduseen_BE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSwaggerServices();
            builder.Services.AddDatabaseServices(builder.Configuration);
            builder.Services.AddSignalR();
            builder.Services.AddAuthenticationServices(builder.Configuration);
            builder.Services.AddDependencyInjectionServices();
            builder.Services.AddAutoMapperServices();
            builder.Services.AddCorsServices(builder.Configuration, builder.Environment);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddApplicationServices();
            builder.Services.AddMemoryCache();
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseCorsPolicy(builder.Environment);
            app.UseSwaggerServices(builder.Environment);
            app.MapHub<ReviewHub>("/reviewhub");
            app.Run();
        }
    }
}
