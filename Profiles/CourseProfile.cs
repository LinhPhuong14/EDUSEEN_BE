using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Course;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Profiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? $"{src.Teacher.FirstName} {src.Teacher.LastName}" : null));

            CreateMap<Course, CourseDetailDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? $"{src.Teacher.FirstName} {src.Teacher.LastName}" : null));
            
            // Admin mappings
            CreateMap<Course, AdminCourseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? $"{src.Teacher.FirstName} {src.Teacher.LastName}" : null))
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))
                .ForMember(dest => dest.TeacherEmail, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.Email : null))
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Enrollments != null ? src.Enrollments.Count : 0))
                .ForMember(dest => dest.SectionCount, opt => opt.MapFrom(src => src.Sections != null ? src.Sections.Count : 0))
                .ForMember(dest => dest.LectureCount, opt => opt.MapFrom(src => src.Sections != null ? src.Sections.Sum(s => s.Lectures != null ? s.Lectures.Count : 0) : 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews != null && src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : (double?)null))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.UpdatedAt != null ? "Active" : "Inactive"))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.UpdatedAt != null))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => (string)null));

            CreateMap<Course, AdminCourseDetailDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? $"{src.Teacher.FirstName} {src.Teacher.LastName}" : null))
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))
                .ForMember(dest => dest.TeacherEmail, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.Email : null))
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Enrollments != null ? src.Enrollments.Count : 0))
                .ForMember(dest => dest.SectionCount, opt => opt.MapFrom(src => src.Sections != null ? src.Sections.Count : 0))
                .ForMember(dest => dest.LectureCount, opt => opt.MapFrom(src => src.Sections != null ? src.Sections.Sum(s => s.Lectures != null ? s.Lectures.Count : 0) : 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews != null && src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : (double?)null))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.UpdatedAt != null ? "Active" : "Inactive"))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.UpdatedAt != null))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => (string)null));
            
            CreateMap<Section, SectionDto>();
            CreateMap<Lecture, LectureDto>();
        }
    }
} 