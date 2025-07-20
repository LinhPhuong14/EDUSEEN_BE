using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Review;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}".Trim()))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.CourseDescription, opt => opt.MapFrom(src => src.Course.Description))
                .ForMember(dest => dest.UserAvatarUrl, opt => opt.MapFrom(src => src.Student.AvatarUrl))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
} 