using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.DTOs.Persons
{
    public class UpdatePersonDto
    {
        public int? UserId { get; set; } //? Check if a person can be updated a different user account
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? Curp { get; set; }
        public bool? IsAccountHolder { get; set; }
    }
}