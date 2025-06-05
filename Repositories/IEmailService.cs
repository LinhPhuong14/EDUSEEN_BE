namespace Sep490_Eduseen_BE.Repositories
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

}
