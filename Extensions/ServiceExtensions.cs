using Sep490_Eduseen_BE.Repositories;
using Sep490_Eduseen_BE.Repositories.impl;
using Sep490_Eduseen_BE.Services;

namespace Sep490_Eduseen_BE.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();  
            services.AddScoped<IOtpService, OtpService>();
            
            // Student Submission Services
            services.AddScoped<IStudentSubmissionRepository, StudentSubmissionRepository>();
            services.AddScoped<IStudentSubmissionService, StudentSubmissionService>();
            
            return services;
        }
    }
}
