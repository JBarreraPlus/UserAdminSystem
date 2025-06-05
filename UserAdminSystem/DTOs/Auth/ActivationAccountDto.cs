using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.DTOs;

public class ActivationAccountDto
{
    [Required] public string Code { get; set; }

    [Required] public string Email { get; set; }
}