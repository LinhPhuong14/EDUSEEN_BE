using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Category;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.Courses.Count))
                .ForMember(dest => dest.Cover, opt => opt.MapFrom(src => src.Cover))
                .ForMember(dest => dest.HoverCover, opt => opt.MapFrom(src => src.HoverCover));
        }
    }
} 