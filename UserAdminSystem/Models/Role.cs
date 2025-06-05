using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.Models;

public class Role : BaseModel
{
    [Key] public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<UserApp> Users { get; set; }
    public ICollection<RoleModule> RoleModules { get; set; }
}