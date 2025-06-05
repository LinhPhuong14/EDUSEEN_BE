namespace Sep490_Eduseen_BE.Repositories;

public interface IUserContext
{
    Task<int> GetCurrentUserIdAsync();
    int GetCurrentUserId();
}