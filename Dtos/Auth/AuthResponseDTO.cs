namespace Sep490_Eduseen_BE.Dtos.Auth;

public class AuthResponseDTO
{
    public bool IsAuthSuccessful { get; set; }
    public string? ErrorMessage { get; set; }

    public TokenDTO Token { get; set; }

}
public class TokenDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}