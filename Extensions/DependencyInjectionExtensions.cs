using Sep490_Eduseen_BE.Services;
using Sep490_Eduseen_BE.Models;
using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Repositories.impl;
using Sep490_Eduseen_BE.Services;

namespace Sep490_Eduseen_BE.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDependencyInjectionServices(this IServiceCollection services){

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IRepositoryAsync<User>, RepositoryAsync<User>>();
        services.AddScoped<IRepositoryAsync<Role>, RepositoryAsync<Role>>();
        services.AddScoped<IRepositoryAsync<EmailConfirmationToken>, RepositoryAsync<EmailConfirmationToken>>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped(typeof(RepositoryAsync<>));
        services.AddScoped<ICourseTeacherService, CourseTeacherService>();
        services.AddScoped<IReviewService, ReviewService>();

        return services;
    }
}