using System.ComponentModel.DataAnnotations;

namespace UserAdminSystem.DTOs.Address;

public class UpdateAddressDto
{
    public string? AddressName { get; set; }
    public string? Street { get; set; }
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}