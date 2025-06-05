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
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    [HttpPut("update-role")]
    public async Task<IActionResult> UpdateRole([FromQuery] string lastRole, [FromQuery] string newRole)
    {
        var currentRolDb = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == lastRole);
        if (currentRolDb == null) return NotFound("Role not found");

        var nameExists = await dbContext.Roles.AnyAsync(x => x.Name == newRole);
        if (nameExists) return BadRequest("Role already exists");

        currentRolDb.Name = newRole;
        await dbContext.SaveChangesAsync();
        return Ok("Role updated");
    }

    [HttpPut("delete-role")]
    public async Task<IActionResult> DeleteRole([FromQuery] string roleName)
    {
        var roleDb = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (roleDb == null) return NotFound("Role not found");

        roleDb.IsActive = false;
        await dbContext.SaveChangesAsync();
        return Ok("Role deleted");
    }

    [HttpGet("get-modules-role")]
    public async Task<IActionResult> GetModulesRole([FromQuery] string roleName)
    {
        var moduleRolesDb = await dbContext.Roles.Where(r => r.Name == roleName).SelectMany(r => r.RoleModules).Select(rm => rm.Module).ToListAsync();
        return Ok(moduleRolesDb);
    }

    [HttpPost("add-module-role")]
    public async Task<IActionResult> AddModuleRole([FromQuery] string roleName, [FromQuery] string addModule)
    {
        var roleDb = await dbContext.Roles.Include(r => r.RoleModules).FirstOrDefaultAsync(r => r.Name == roleName);
        if (roleDb == null) return NotFound("Role not found");

        var moduleDb = await dbContext.Modules.FirstOrDefaultAsync(m => m.Name == addModule);
        if (moduleDb == null) return NotFound("Module not found");

        if (roleDb.RoleModules.Any(rm => rm.ModuleId == moduleDb.Id))
            return BadRequest("Module already exists for this role");

        var roleModule = new RoleModule
        {
            RoleId = roleDb.Id,
            ModuleId = moduleDb.Id
        };

        await dbContext.RoleModules.AddAsync(roleModule);
        await dbContext.SaveChangesAsync();
        return Ok("Module added to role");
    }

    [HttpPut("delete-module-role")]
    public async Task<IActionResult> DeleteModuleRole([FromQuery] string roleName, [FromQuery] string moduleName)
    {
        var roleDb = await dbContext.Roles.Include(r => r.RoleModules).FirstOrDefaultAsync(r => r.Name == roleName);
        if (roleDb == null) return NotFound("Role not found");

        var moduleDb = await dbContext.Modules.FirstOrDefaultAsync(m => m.Name == moduleName);
        if (moduleDb == null) return NotFound("Module not found");

        var roleModule = roleDb.RoleModules.FirstOrDefault(rm => rm.ModuleId == moduleDb.Id);
        if (roleModule == null) return NotFound("Module not found for this role");

        roleModule.IsActive = false;
        dbContext.RoleModules.Update(roleModule);
        await dbContext.SaveChangesAsync();
        return Ok("Module removed from role");
    }
}