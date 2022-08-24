using Microsoft.AspNetCore.Identity.UI.Services;

namespace Blog.Services.Interfaces
{
    public interface IEmailService : IEmailSender
    {
        public Task SendAdminEmailAsync(string email, string subject, string htmlMessage);
    }
}
