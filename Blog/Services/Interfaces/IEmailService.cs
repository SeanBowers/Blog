namespace Blog.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendAdminEmailAsync(string email, string subject, string htmlMessage);
    }
}
