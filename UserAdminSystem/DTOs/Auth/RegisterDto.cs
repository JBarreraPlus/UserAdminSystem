using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.DTOs;

public class RegisterDto
{
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required] public string UserName { get; set; }

    [DataType(DataType.Password)]
    [Required]
    public string Password { get; set; }

    public int? RoleId { get; set; }
}