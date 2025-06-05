using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserAdminSystem.Data;
using UserAdminSystem.DTOs;
using UserAdminSystem.Models;
using UserAdminSystem.Responses;
using UserAdminSystem.Services.Contracts;
using UserAdminSystem.Utilities;

namespace UserAdminSystem.Services.Service;

public class AuthService(
    AppDbContext context,
    IOptions<JwtSection> config,
    ILogger<AuthService> logger,
    IEmailService emailService,
    ICodeValidationService codeValidationService)
    : IAuthService
{
    public async Task<GeneralResponse<LoginResponse>> SignInAsync(LoginDto user)
    {
        var userDb = await GetUserByEmailAsync(user.Email);
        if (userDb == null)
            return new GeneralResponse<LoginResponse>(false, "User not found", StatusCodes.Status404NotFound);

        if (userDb.EmailConfirmed == false)
            return new GeneralResponse<LoginResponse>(false, "Your account has not been confirmed",
                StatusCodes.Status401Unauthorized);

        // Verify password
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, userDb.PasswordHash);
        if (!isPasswordValid)
            return new GeneralResponse<LoginResponse>(false, "Invalid password", StatusCodes.Status400BadRequest);

        // Generate JWT
        var token = GenerateJwtToken(userDb);
        return new GeneralResponse<LoginResponse>(true, "Login successful", Data: new LoginResponse { Token = token });
    }

    public async Task<GeneralResponse<object>> RegisterAsync(RegisterDto registerDto)
    {
        var checkUser = await context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
        if (checkUser != null)
            return new GeneralResponse<object>(false, "Email already exists", StatusCodes.Status400BadRequest);

        var userEntity = new UserApp
        {
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };

        context.Users.Add(userEntity);
        await context.SaveChangesAsync();

        // Generate and save verification code
        var emailCode = await codeValidationService.GenerateAndSaveVerificationCodeAsync(registerDto.Email);
        await emailService.SendEmailAsync("Email Verification Code", $"Your code is {emailCode}");

        return new GeneralResponse<object>(true, "Account created");
    }

    public async Task<GeneralResponse<object>> ActivateAccountAsync(ActivationAccountDto activationAccount)
    {
        var userDb = await context.Users.FirstOrDefaultAsync(u => u.Email == activationAccount.Email);
        if (userDb == null) return new GeneralResponse<object>(false, "User not found", StatusCodes.Status404NotFound);
        if (userDb.EmailConfirmed == true)
            return new GeneralResponse<object>(false, "Your account already activated",
                StatusCodes.Status401Unauthorized);
        var checkCode =
            await codeValidationService.ValidateVerificationCodeAsync(activationAccount.Email, activationAccount.Code);
        if (checkCode == false)
            return new GeneralResponse<object>(false, "Invalid code", StatusCodes.Status400BadRequest);

        return new GeneralResponse<object>(true, "Account activated");
    }

    public async Task<GeneralResponse<object>> ResendEmailVerificationCodeAsync(string email)
    {
        var userDb = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (userDb == null) return new GeneralResponse<object>(false, "Email not found", StatusCodes.Status404NotFound);
        if (userDb.EmailConfirmed == true)
            return new GeneralResponse<object>(false, "Your account already activated",
                StatusCodes.Status401Unauthorized);

        var emailCode = await codeValidationService.GenerateAndSaveVerificationCodeAsync(email);
        if (emailCode == null)
            return new GeneralResponse<object>(false, "Email not found", StatusCodes.Status404NotFound);
        await emailService.SendEmailAsync("Email Verification Code", $"Your code is {emailCode}");
        return new GeneralResponse<object>(true, "Email verification code sent");
    }

    public async Task<GeneralResponse<object>> SendPasswordRecoveryCodeAsync(string email)
    {
        var emailCode = await codeValidationService.GenerateAndSaveVerificationCodeAsync(email);
        if (emailCode == null)
            return new GeneralResponse<object>(false, "Email not found", StatusCodes.Status404NotFound);

        await emailService.SendEmailAsync("Password Recovery Code", $"Your code is {emailCode}");

        return new GeneralResponse<object>(true, "Password recovery code sent");
    }

    public async Task<GeneralResponse<object>> SetNewPasswordAsync(PasswordRecoveryDto passwordRecoveryDto)
    {
        var checkCode =
            await codeValidationService.ValidateVerificationCodeAsync(passwordRecoveryDto.Email,
                passwordRecoveryDto.Code);
        if (checkCode == false)
            return new GeneralResponse<object>(false, "Invalid code", StatusCodes.Status400BadRequest);

        var userDb = await context.Users.FirstOrDefaultAsync(u => u.Email == passwordRecoveryDto.Email);
        if (userDb == null) return new GeneralResponse<object>(false, "Email not found", StatusCodes.Status404NotFound);

        userDb.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordRecoveryDto.NewPassword);
        context.Users.Update(userDb);
        await context.SaveChangesAsync();

        return new GeneralResponse<object>(true, "Password updated");
    }

    private async Task<UserApp?> GetUserByEmailAsync(string email) // ? Helper
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user;
    }

    private string GenerateJwtToken(UserApp userDb)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userDb.Id.ToString()),
            new Claim(ClaimTypes.Name, userDb.UserName!),
            new Claim(ClaimTypes.Email, userDb.Email!)
        };

        var token = new JwtSecurityToken(
            config.Value.Issuer,
            config.Value.Audience,
            userClaims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}