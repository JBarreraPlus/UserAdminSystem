namespace UserAdminSystem.Models;

public class RoleModule : BaseModel
{
    public int RoleId { get; set; }

    public Role Role { get; set; }

    public int ModuleId { get; set; }

    public Module Module { get; set; }
}