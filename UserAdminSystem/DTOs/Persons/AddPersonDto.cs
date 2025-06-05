using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.DTOs.Persons;

public class AddPersonDto
{
    [Required]
    public required int UserId { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string LastName { get; set; }
    public DateOnly Birthday { get; set; }
    [Required]
    public required string Curp { get; set; }
    public bool IsAccountHolder { get; set; }
}