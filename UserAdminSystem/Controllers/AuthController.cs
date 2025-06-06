using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAdminSystem.DTOs;
using UserAdminSystem.Services.Contracts;

namespace UserAdminSystem.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    //TODO: Implementar Roles para la autorización de los endpoints

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto user)
    {
        var result = await authService.SignInAsync(user);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto user)
    {
        var tokenHeader = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var result = await authService.RegisterAsync(user, tokenHeader);
        return StatusCode(result.StatusCode, result);

    }

    [HttpPost("activate-account")]
    [AllowAnonymous]
    public async Task<IActionResult> ActivateAccount(ActivationAccountDto activation)
    {
        var result = await authService.ActivateAccountAsync(activation);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("resend-email-verification-code")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendEmailVerificationCode([Required] string email)
    {
        var result = await authService.ResendEmailVerificationCodeAsync(email);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("send-password-recovery-code")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SendPasswordRecoveryCode([Required] string email)
    {
        var result = await authService.SendPasswordRecoveryCodeAsync(email);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("set-new-password")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SetNewPassword(PasswordRecoveryDto passwordRecoveryDto)
    {
        var result = await authService.SetNewPasswordAsync(passwordRecoveryDto);
        return StatusCode(result.StatusCode, result);
    }


}