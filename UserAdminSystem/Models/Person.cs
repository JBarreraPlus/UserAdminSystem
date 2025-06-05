using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.Models;

public class Person : BaseModel
{
    [Key] public int Id { get; set; }

    public int AddressId { get; set; }

    public Address Address { get; set; }

    public int UserId { get; set; }

    public UserApp User { get; set; }


    public string Name { get; set; }

    //[Required]
    //[StringLength(100, MinimumLength = 2)]
    public string? LastName { get; set; }

    //[Required]
    //[DataType(DataType.Date)]
    public DateOnly Birthday { get; set; }

    //[Required]
    //[RegularExpression(@"^[A-Z]{4}[0-9]{6}[A-Z0-9]{8}$", ErrorMessage = "Invalid format")]
    public string? Curp { get; set; }
    public bool IsAccountHolder { get; set; }
}