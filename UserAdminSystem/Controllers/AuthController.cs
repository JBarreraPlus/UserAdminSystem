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
        // If there is a token in the header, we need to validate if is an admin user or any role that allows register an user and assign it a role.
        if (user.RoleId.HasValue && Request.Headers.ContainsKey("Authorization"))
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userRole = GetUserRole(token);

            if (userRole != "Admin" && userRole != "SuperUser")
            {
                return Unauthorized(new { Message = "You do not have permission to assign a role." });
            }
        }
        var result = await authService.RegisterAsync(user);
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

    private string GetUserRole(string token)
    {
        //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var userRole = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

        return userRole ?? string.Empty;
    }
}