using Blog.Models;
using Blog.Services.Interfaces;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Blog.Services
{
    public class EmailService : /*IEmailSender,*/ IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");

            MimeMessage newEmail = new();
            //Add all email address to the "TO" for the email.
            newEmail.Sender = MailboxAddress.Parse(emailSender);

            //var email = "theseanbowers@gmail.com";

            newEmail.To.Add(MailboxAddress.Parse(email));

            //Add the subject to the email.
            newEmail.Subject = subject;

            //Add the body to the email.
            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();

            //Send the email.
            using SmtpClient smtpClient = new();
            try
            {
                var host = _mailSettings.Host ?? Environment.GetEnvironmentVariable("Host");
                var port = _mailSettings.Port != 0 ? _mailSettings.Port : int.Parse(Environment.GetEnvironmentVariable("Port")!);
                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, _mailSettings.Password ?? Environment.GetEnvironmentVariable("Password"));

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);
            }
            catch
            {
                throw;
            }
        }
        public async Task SendAdminEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");

            MimeMessage newEmail = new();
            //Add all email address to the "TO" for the email.
            newEmail.Sender = MailboxAddress.Parse(emailSender);

            newEmail.To.Add(MailboxAddress.Parse(email));

            //Add the subject to the email.
            newEmail.Subject = subject;

            //Add the body to the email.
            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();

            //Send the email.
            using SmtpClient smtpClient = new();
            try
            {
                var host = _mailSettings.Host ?? Environment.GetEnvironmentVariable("Host");
                var port = _mailSettings.Port != 0 ? _mailSettings.Port : int.Parse(Environment.GetEnvironmentVariable("Port")!);
                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, _mailSettings.Password ?? Environment.GetEnvironmentVariable("Password"));

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);
            }
            catch
            {
                throw;
            }
        }
    }
}
