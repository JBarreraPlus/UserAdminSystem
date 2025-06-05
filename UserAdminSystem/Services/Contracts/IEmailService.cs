namespace UserAdminSystem.Services.Contracts;

public interface IEmailService
{
    Task SendEmailAsync(string subject, string message);
}