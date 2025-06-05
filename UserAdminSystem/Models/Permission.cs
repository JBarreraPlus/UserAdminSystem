using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.Models;

public class Permission : BaseModel
{
    [Key] public int Id { get; set; }

    public string Name { get; set; }

    public int? ParentId { get; set; } // ? Relación de jerarquía dentro de la misma tabla

    public ICollection<ModulePermission> ModulePermissions { get; set; }
}