namespace Sep490_Eduseen_BE.Exceptions;

public class RefreshTokenBadRequest : Exception
{
    public RefreshTokenBadRequest() 
        : base("Invalid or expired refresh token.")
        {
        }

        public RefreshTokenBadRequest(string message) 
        : base(message)
        {
        }

        public RefreshTokenBadRequest(string message, Exception innerException) 
        : base(message, innerException)
        {
        }
}