using UserAdminSystem.DTOs;
using UserAdminSystem.Responses;

namespace UserAdminSystem.Services.Contracts;

public interface IAuthService
{
    Task<GeneralResponse<LoginResponse>> SignInAsync(LoginDto user);
    Task<GeneralResponse<object>> RegisterAsync(RegisterDto user);
    Task<GeneralResponse<object>> ActivateAccountAsync(ActivationAccountDto activationAccount);
    Task<GeneralResponse<object>> ResendEmailVerificationCodeAsync(string email);
    Task<GeneralResponse<object>> SendPasswordRecoveryCodeAsync(string email);
    Task<GeneralResponse<object>> SetNewPasswordAsync(PasswordRecoveryDto passwordRecoveryDto);
}