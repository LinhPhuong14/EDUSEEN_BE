using System.ComponentModel.DataAnnotations;

namespace Sep490_Eduseen_BE.Dtos
{
    public class ProfileDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }

    public class UpdateProfileDTO
    {
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? LastName { get; set; }

        [Url(ErrorMessage = "Invalid URL format for avatar.")]
        [StringLength(500, ErrorMessage = "Avatar URL cannot exceed 500 characters.")]
        public string? AvatarUrl { get; set; }
    }
    public class UpdateUserDTO
    {
        //[StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        //public string? Username { get; set; }

        //[EmailAddress(ErrorMessage = "Invalid email format.")]
        //[StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        //public string? Email { get; set; }

        //[StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        //public string? FirstName { get; set; }

        //[StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        //public string? LastName { get; set; }

        //[Url(ErrorMessage = "Invalid URL format for avatar.")]
        //[StringLength(500, ErrorMessage = "Avatar URL cannot exceed 500 characters.")]
        //public string? AvatarUrl { get; set; }

        public int? RoleId { get; set; }
    }
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }
    }
    public class UserListDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsActive { get; set; }
        public string RoleName { get; set; } = null!;
         public int RoleId { get; set; }
    }

    public class UserDetailDto : UserListDto
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? AvatarUrl { get; set; }
    }

}