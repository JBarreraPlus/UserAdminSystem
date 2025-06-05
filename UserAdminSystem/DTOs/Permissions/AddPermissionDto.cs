using Microsoft.Build.Framework;

namespace UserAdminSystem.DTOs.Permissions;

public class AddPermissionDto
{
    [Required] public string Name { get; set; }

    public int? ParentId { get; set; }
}