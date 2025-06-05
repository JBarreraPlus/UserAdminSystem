using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using UserAdminSystem.Services.Contracts;

namespace UserAdminSystem.Services.Service;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(string subject, string message)
    {
        await Task.Run(() =>
        {
            try
            {
                var email = new MimeMessage();
                email.To.Add(MailboxAddress.Parse("michael98@ethereal.email"));
                email.From.Add(MailboxAddress.Parse("michael98@ethereal.email"));
                email.Subject = subject;
                email.Body = new TextPart("plain") { Text = message };

                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("michael98@ethereal.email", "TcqHm6JT2MS4FT6kfX");
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception e)
            {
                logger.LogError($"Error sending email: {e.Message}");
            }
        });
    }
}