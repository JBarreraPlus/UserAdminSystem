using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAdminSystem.Data;
using UserAdminSystem.Models;

namespace UserAdminSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    [HttpPut("update-module/{moduleName}")]
    public async Task<IActionResult> UpdateModule([Required] string moduleName)
    {
        var moduleDb = await dbContext.Modules.FirstOrDefaultAsync(x => x.Name == moduleName);
        if (moduleDb == null) return BadRequest("Module not found");

        var nameExists = await dbContext.Modules.AnyAsync(x => x.Name == moduleName);
        if (nameExists) return BadRequest("Module already exists");
        moduleDb.Name = moduleName;
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

    //TODO: Test endpoints
    //TODO: AddPermission
    //TODO: DeletePermission
    //TODO: UpdatePermission
}