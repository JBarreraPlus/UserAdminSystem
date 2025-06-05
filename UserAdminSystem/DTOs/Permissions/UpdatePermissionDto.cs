using Microsoft.Build.Framework;

namespace UserAdminSystem.DTOs.Permissions;

public class UpdatePermissionDto
{
    [Required] public int PermissionId { get; set; }

    public string? Name { get; set; }

    public int? ParentId { get; set; }
}