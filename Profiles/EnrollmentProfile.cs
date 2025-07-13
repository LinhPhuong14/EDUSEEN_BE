using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Enrollment;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Profiles
{
    public class EnrollmentProfile : Profile
    {
        public EnrollmentProfile()
        {
            CreateMap<Enrollment, EnrollmentDto>();
        }
    }
} 