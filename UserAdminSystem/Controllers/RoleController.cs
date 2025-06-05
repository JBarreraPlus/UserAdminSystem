using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAdminSystem.Data;
using UserAdminSystem.Models;

namespace UserAdminSystem.Controllers;

[Route("api/role")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RoleController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("get-roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var rolesDb = await dbContext.Roles.ToListAsync();
        return Ok(rolesDb);
    }

    [HttpPost("add-role/{rolName}")]
    public async Task<IActionResult> AddRole(string rolName)
    {
        var checkRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == rolName);
        if (checkRole != null) return BadRequest("Role already exists");

        var role = new Role
        {
            Name = rolName
        };
        await dbContext.Roles.AddAsync(role);
        await dbContext.SaveChangesAsync();
        return Ok("Role added");
    }

    [HttpPut("update-role/{rolName}")]
    public async Task<IActionResult> UpdateRole([Required] string rolName)
    {
        var rolDb = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == rolName);
        if (rolDb == null) return NotFound("Role not found");

        var nameExists = await dbContext.Roles.AnyAsync(x => x.Name == rolName);
        if (nameExists) return BadRequest("Role already exists");

        rolDb.Name = rolName;
        await dbContext.SaveChangesAsync();
        return Ok("Role updated");
    }

    [HttpPut("delete-role/{rolName}")]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var roleDb = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (roleDb == null) return NotFound("Role not found");

        roleDb.IsActive = false;
        await dbContext.SaveChangesAsync();
        return Ok("Role deleted");
    }
    
    //TODO: Test endpoints
    //TODO: Get Modules
    //TODO: Add Module
    //TODO: Delete Module
}