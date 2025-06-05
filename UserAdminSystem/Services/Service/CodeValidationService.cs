using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using UserAdminSystem.Data;
using UserAdminSystem.Services.Contracts;

namespace UserAdminSystem.Services.Service;

public class CodeValidationService(AppDbContext dbContext) : ICodeValidationService
{
    public async Task<bool> ValidateVerificationCodeAsync(string email, string code)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || user.VerificationCode != code || user.Expiration < DateTime.UtcNow)
            return false;

        // Marking code like verified
        user.VerificationCode = null;
        user.Expiration = null;
        user.EmailConfirmed = true;
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<string?> GenerateAndSaveVerificationCodeAsync(string email)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;

        var emailCode = GenerateVerificationCode();
        user.VerificationCode = emailCode;
        user.Expiration = DateTime.UtcNow.AddMinutes(30);
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();

        return emailCode;
    }

    public string GenerateVerificationCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var code = BitConverter.ToUInt32(bytes, 0) % 1000000;
        return code.ToString("D6");
    }
}