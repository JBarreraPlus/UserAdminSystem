using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAdminSystem.Data;
using UserAdminSystem.DTOs.Users;
using UserAdminSystem.Models;
using UserAdminSystem.Services.Contracts;

namespace UserAdminSystem.Controllers;

[Route("api/user")]
[ApiController]
[Authorize(Policy = "AdminPolicy")]
[Authorize(Policy = "UserPolicy")]
public class UserController(
    AppDbContext dbContext,
    IEmailService emailService,
    ICodeValidationService codeValidationService) : ControllerBase
{
    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var usersDb = await dbContext.Users.ToListAsync();
        return Ok(usersDb);
    }

    [HttpGet("get-user/{userId}")]
    public async Task<IActionResult> GetUser(int userId)
    {
        var userDb = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return Ok(userDb);
    }

    [HttpPost("add-user")]
    public async Task<IActionResult> AddUser(AddUserDto userDto)
    {
        var user = new UserApp
        {
            Email = userDto.Email,
            UserName = userDto.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        // Email confirm account
        var emailCode = await codeValidationService.GenerateAndSaveVerificationCodeAsync(userDto.Email);
        await emailService.SendEmailAsync("Email VerificationCode", $"Your code is {emailCode}");

        return Ok(user);
    }

    [HttpPut("update-password/{emailUser}/{password}")]
    public async Task<IActionResult> UpdatePassword(string emailUser, string password)
    {
        var userDb = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == emailUser);
        if (userDb is null) return NotFound("User not found");

        var checkUser = dbContext.Database
            .SqlQueryRaw<string>("EXEC ValidateUserStatus @p0", emailUser)
            .AsEnumerable()
            .FirstOrDefault();

        if (checkUser != "ValidUser")
            return BadRequest(checkUser);

        userDb.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await dbContext.SaveChangesAsync();
        return Ok();
    }
}