using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAdminSystem.Data;
using UserAdminSystem.DTOs.Permissions;
using UserAdminSystem.Models;

namespace UserAdminSystem.Controllers;

[Route("api/permission")]
[ApiController]
[Authorize(Policy = "AdminPolicy")]
public class PermissionController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("get-permissions")]
    public async Task<IActionResult> GetPermissions()
    {
        var permissionsDb = await dbContext.Permissions.ToListAsync();
        return Ok(permissionsDb);
    }

    [HttpGet("get-parents-permission")]
    public async Task<IActionResult> GetParentsPermission()
    {
        var parentsDb = await dbContext.Permissions.Where(p => p.ParentId == null).ToListAsync();
        foreach (var parent in parentsDb)
        {
        }

        return Ok(parentsDb);
    }

    [HttpPost("add-permission")]
    public async Task<IActionResult> AddPermission(AddPermissionDto permission)
    {
        var nameExists = await dbContext.Permissions
            .AnyAsync(p => p.Name == permission.Name);

        if (nameExists)
            return Conflict("Permission already exists");

        // Check if the permission has children
        if (permission.ParentId != 0)
        {
            var parentPermission = await dbContext.Permissions
            .FirstOrDefaultAsync(p => p.Id == permission.ParentId);

            if (parentPermission == null || parentPermission.ParentId == null)
            {
                return BadRequest("Parent permission cannot be a parent itself.");
            }
        }

        var permissionModel = new Permission
        {
            Name = permission.Name,
            ParentId = permission.ParentId != 0 ? permission.ParentId : null
        };

        await dbContext.Permissions.AddAsync(permissionModel);
        await dbContext.SaveChangesAsync();
        return Ok("Permission added");
    }

    [HttpPut("update-permission")]
    public async Task<IActionResult> UpdatePermission(UpdatePermissionDto permission)
    {
        var permissionDb = await dbContext.Permissions
            .Where(p => p.Id == permission.PermissionId)
            .FirstOrDefaultAsync();

        if (permissionDb == null)
            return NotFound("Permission not found");

        if (!string.IsNullOrWhiteSpace(permission.Name))
        {
            var nameExists = await dbContext.Permissions
                .AnyAsync(p => p.Name == permission.Name && p.Id != permission.PermissionId);

            if (nameExists)
                return Conflict("Permission already exists");

            permissionDb.Name = permission.Name;
        }

        // Check if the permission has children
        var hasChildren = await dbContext.Permissions
            .AnyAsync(p => p.ParentId == permission.PermissionId);

        if (hasChildren && permission.ParentId != 0)
        {
            return BadRequest("A parent permission with children cannot be reassigned as a child of another parent permission.");
        }

        // Check if the parent permission exists and is not a parent itself
        if (permission.ParentId != 0)
        {
            var parentPermission = await dbContext.Permissions
                .FirstOrDefaultAsync(p => p.Id == permission.ParentId);

            if (parentPermission == null || parentPermission.ParentId == null)
            {
                return BadRequest("Parent permission cannot be a parent itself.");
            }
        }

        permissionDb.ParentId = permission.ParentId != 0 ? permission.ParentId : null;

        await dbContext.SaveChangesAsync();
        return Ok("Permission updated");
    }


    [HttpPut("delete-permission/{permissionId}/{deleteHierarchy}")]
    public async Task<IActionResult> DeletePermission(int permissionId, bool deleteHierarchy)
    {
        var permissionDb = await dbContext.Permissions.FindAsync(permissionId);

        if (permissionDb == null)
            return NotFound("Permission not found");

        if (deleteHierarchy)
        {
            // Delete hierarchy in cascade
            await DeactivatePermissionsRecursively(permissionDb.Id);
        }
        else
        {
            // Only deactivate parent permission and convert childs into new parents
            permissionDb.IsActive = false;

            var childPermissions = await dbContext.Permissions
                .Where(p => p.ParentId == permissionDb.Id)
                .ToListAsync();

            foreach (var child in childPermissions) child.ParentId = null; //Convert permission to root
        }

        await dbContext.SaveChangesAsync();
        return Ok("Permission deleted successfully.");
    }

    private async Task DeactivatePermissionsRecursively(int permissionId)
    {
        var permission = await dbContext.Permissions.FindAsync(permissionId);
        if (permission == null) return;

        permission.IsActive = false;

        var childPermissions = await dbContext.Permissions
            .Where(p => p.ParentId == permission.Id)
            .ToListAsync();

        foreach (var child in childPermissions) await DeactivatePermissionsRecursively(child.Id);
    }
}