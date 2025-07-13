using Sep490_Eduseen_BE.Dtos;

namespace Sep490_Eduseen_BE.Dtos.Auth;

public class AuthResponseDTO
{
    public bool IsAuthSuccessful { get; set; }
    public string? ErrorMessage { get; set; }

    public TokenDTO Token { get; set; }

    public ProfileDTO? User { get; set; } // Thông tin cơ bản của người dùng

}
public class TokenDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
public class ForgotPasswordDTO
{
    public string Email { get; set; } = null!;
}

public class ResetPasswordDTO
{
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

public class ChangePasswordDTO
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmNewPassword { get; set; } = null!;
}

public class GenericResponseDTO
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

