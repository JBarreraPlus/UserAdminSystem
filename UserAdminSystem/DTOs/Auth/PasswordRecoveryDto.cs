using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.DTOs;

public class PasswordRecoveryDto
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] public string Code { get; set; }

    [Required] public string NewPassword { get; set; }
}