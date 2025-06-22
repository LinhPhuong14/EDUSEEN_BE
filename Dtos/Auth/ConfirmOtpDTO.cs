using System.ComponentModel.DataAnnotations;

namespace Sep490_Eduseen_BE.Dtos.Auth;
public class ConfirmOtpDTO

{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP code is required.")]
    public string Otp { get; set; } = string.Empty;

}
