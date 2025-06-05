using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.DTOs.Address;

public class AddAddressDto
{
    [Required]
    public required string AddressName { get; set; }
    [Required]
    public required string Street { get; set; }
    public string? Neighborhood { get; set; }
    [Required]
    public required string City { get; set; }
    [Required]
    public required string State { get; set; }
    public string? PostalCode { get; set; }
    [Required]
    public required string Country { get; set; }
}