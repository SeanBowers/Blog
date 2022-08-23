namespace Blog.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string firstName, string lastName, string userName, string subject, string htmlMessage);
    }
}
