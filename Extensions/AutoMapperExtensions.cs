using Sep490_Eduseen_BE.Profiles;

namespace Sep490_Eduseen_BE.Extensions;

public static class AutoMapperExtensions
{
    public static IServiceCollection AddAutoMapperServices(this IServiceCollection services){
        services.AddAutoMapper(typeof(CourseProfile));
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddAutoMapper(typeof(CategoryProfile));
        services.AddAutoMapper(typeof(ReviewProfile));
        services.AddAutoMapper(typeof(StudentSubmissionProfile));
        return services;
    }
}