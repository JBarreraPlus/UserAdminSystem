using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.Models;

public class Module : BaseModel
{
    [Key] public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<RoleModule> RoleModules { get; set; }
    public ICollection<ModulePermission> ModulePermissions { get; set; }
}