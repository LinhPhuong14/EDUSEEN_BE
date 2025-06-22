using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration){
        services.AddDbContext<Sep490EduseenContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    //    .EnableSensitiveDataLogging();
            });
            return services;
    }
}