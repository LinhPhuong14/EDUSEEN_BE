using Sep490_Eduseen_BE.Profiles;

namespace Sep490_Eduseen_BE.Extensions;

public static class AutoMapperExtensions
{
    public static IServiceCollection AddAutoMapperServices(this IServiceCollection services){
        services.AddAutoMapper(typeof(MappingProfile));
        return services;
    }
}