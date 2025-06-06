using UserAdminSystem.Models;

namespace UserAdminSystem.Responses.ProfileResponse;

public class PersonsResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public List<AddressResponse>? Addresses { get; set; }
    public UserResponse User { get; set; }
    public DateOnly Birthday { get; set; }
    public string? Curp { get; set; }
    public bool IsAccountHolder { get; set; }
}