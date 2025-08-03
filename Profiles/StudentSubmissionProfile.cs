using AutoMapper;
using Sep490_Eduseen_BE.Dtos.Submission;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Profiles;

public class StudentSubmissionProfile : Profile
{
    public StudentSubmissionProfile()
    {
        // Mapping từ Submission model sang StudentSubmissionResponseDTO
        CreateMap<Submission, StudentSubmissionResponseDTO>()
            .ForMember(dest => dest.Files, opt => opt.MapFrom(src => src.SubmissionFiles));

        // Mapping từ SubmissionFile model sang SubmissionFileResponseDTO
        CreateMap<SubmissionFile, SubmissionFileResponseDTO>();

        // Mapping từ StudentSubmitAssignmentDTO sang Submission (cho việc tạo mới)
        CreateMap<StudentSubmitAssignmentDTO, Submission>()
            .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Mapping từ SubmissionFileDTO sang SubmissionFile
        CreateMap<SubmissionFileDTO, SubmissionFile>();
    }
} 