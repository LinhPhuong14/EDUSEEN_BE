using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Auth;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<User, AuthResponseDTO>()
                .ForMember(dest => dest.IsAuthSuccessful, opt => opt.Ignore());
        }
    }
}