namespace UserAdminSystem.Models;

public class ModulePermission : BaseModel
{
    public int ModuleId { get; set; }

    public Module Module { get; set; }

    public int PermissionId { get; set; }

    public Permission Permission { get; set; }
}