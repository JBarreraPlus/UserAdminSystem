using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.Models;

public class Address : BaseModel
{
    [Key] public int Id { get; set; }

    public string? AddressName { get; set; }

    public string? Street { get; set; }
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public int PersonId { get; set; }
    public Person Person { get; set; }
}