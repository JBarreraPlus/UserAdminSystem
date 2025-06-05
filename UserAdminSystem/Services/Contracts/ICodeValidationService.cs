namespace UserAdminSystem.Services.Contracts;

public interface ICodeValidationService
{
    Task<bool> ValidateVerificationCodeAsync(string email, string code);
    Task<string?> GenerateAndSaveVerificationCodeAsync(string email);
    string GenerateVerificationCode();
}