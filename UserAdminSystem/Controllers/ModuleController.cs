using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAdminSystem.Data;
using UserAdminSystem.Models;

namespace UserAdminSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ModuleController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("get-modules")]
    public async Task<IActionResult> GetModules()
    {
        var modulesDb = await dbContext.Modules.ToListAsync();
        return Ok(modulesDb);
    }

    [HttpPost("add-module/{moduleName}")]
    public async Task<IActionResult> AddModule([Required] string moduleName)
    {
        var checkModule = await dbContext.Modules.FirstOrDefaultAsync(x => x.Name == moduleName);
        if (checkModule != null) return BadRequest("Module already exists");

        var module = new Module
        {
            Name = moduleName
        };

        await dbContext.Modules.AddAsync(module);
        await dbContext.SaveChangesAsync();
        return Ok("Module added");
    }

    [HttpPut("update-module")]
    public async Task<IActionResult> UpdateModule(string lastModule, string newModule)
    {
        var currentModuleDb = await dbContext.Modules.FirstOrDefaultAsync(x => x.Name == lastModule);
        if (currentModuleDb == null) return BadRequest("Module not found");

        var nameExists = await dbContext.Modules.AnyAsync(x => x.Name == newModule);
        if (nameExists) return BadRequest("Module already exists");

        currentModuleDb.Name = newModule;
        await dbContext.SaveChangesAsync();

        return Ok("Module updated");
    }

    [HttpPut("delete-module/{moduleName}")]
    public async Task<IActionResult> DeleteModule([Required] string moduleName)
    {
        var moduleDb = await dbContext.Modules.FirstOrDefaultAsync(x => x.Name == moduleName);
        if (moduleDb == null) return BadRequest("Module not found");

        moduleDb.IsActive = false;
        await dbContext.SaveChangesAsync();
        return Ok("Module deleted");
    }

    [HttpPost("add-permission-module")]
    public async Task<IActionResult> AddPermissionModule([FromQuery] string moduleName, [FromQuery] string permissionName)
    {
        var moduleDb = await dbContext.Modules.FirstOrDefaultAsync(x => x.Name == moduleName);
        if (moduleDb is null) return NotFound("Module not found");

        var permissionDb = await dbContext.Permissions.FirstOrDefaultAsync(x => x.Name == permissionName);
        if (permissionDb is null) return NotFound("Permission not found");

        if (permissionDb.ParentId != null) return BadRequest("Permission cannot be a child permission");

        var existingModulePermission = await dbContext.ModulePermissions
            .FirstOrDefaultAsync(mp => mp.ModuleId == moduleDb.Id && mp.PermissionId == permissionDb.Id);
        if (existingModulePermission != null)
            return BadRequest("Permission already exists for this module");

        var modulePermission = new ModulePermission
        {
            ModuleId = moduleDb.Id,
            PermissionId = permissionDb.Id
        };

        dbContext.ModulePermissions.Add(modulePermission);
        await dbContext.SaveChangesAsync();
        return Ok("Permission added to module");
    }

    [HttpPut("delete-permission-module")]
    public async Task<IActionResult> DeletePermissionModule([FromQuery] string moduleName, [FromQuery] string permissionName)
    {
        var moduleDb = await dbContext.Modules.FirstOrDefaultAsync(x => x.Name == moduleName);
        if (moduleDb is null) return NotFound("Module not found");

        var permissionDb = await dbContext.Permissions.FirstOrDefaultAsync(x => x.Name == permissionName);
        if (permissionDb is null) return NotFound("Permission not found");

        var modulePermission = await dbContext.ModulePermissions
            .FirstOrDefaultAsync(mp => mp.ModuleId == moduleDb.Id && mp.PermissionId == permissionDb.Id);
        if (modulePermission is null) return NotFound("Permission not found for this module");

        dbContext.ModulePermissions.Remove(modulePermission);
        await dbContext.SaveChangesAsync();
        return Ok("Permission removed from module");
    }

    [HttpPut("update-permission-module")]
    public async Task<IActionResult> UpdatePermissionModule([FromQuery] string moduleName, [FromQuery] string permissionName)
    {
        var moduleDb = await dbContext.Modules.FirstOrDefaultAsync(x => x.Name == moduleName);
        if (moduleDb == null) return NotFound("Module not found");

        var permissionDb = await dbContext.Permissions.FirstOrDefaultAsync(x => x.Name == permissionName);
        if (permissionDb == null) return NotFound("Permission not found");

        if (permissionDb.ParentId != null)
        {
            return BadRequest("The permission cannot be a child.");
        }

        var modulePermission = await dbContext.ModulePermissions
            .FirstOrDefaultAsync(mp => mp.ModuleId == moduleDb.Id);
        if (modulePermission == null) return NotFound("Permission not found for this module");

        dbContext.ModulePermissions.Remove(modulePermission);
        await dbContext.SaveChangesAsync();

        var newModulePermission = new ModulePermission
        {
            ModuleId = moduleDb.Id,
            PermissionId = permissionDb.Id
        };

        dbContext.ModulePermissions.Add(newModulePermission);
        await dbContext.SaveChangesAsync();

        return Ok("Permission updated for module successfully.");
    }
}