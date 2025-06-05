using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.Models;

public class UserApp : BaseModel
{
    [Key] public int Id { get; set; }

    [EmailAddress] public string? Email { get; set; }

    public string? UserName { get; set; }
    public string? ProfileImage { get; set; }

    public int? RoleId { get; set; }
    public Role? Role { get; set; }

    public string? PasswordHash { get; set; }

    public bool? EmailConfirmed { get; set; } = false;
    public string? PhoneNumber { get; set; }
    public string? VerificationCode { get; set; }
    public DateTime? Expiration { get; set; }
    public ICollection<Person>? Persons { get; set; }
}