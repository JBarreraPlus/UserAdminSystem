namespace UserAdminSystem.Responses.ProfileResponse;

public class AddressResponse
{
    public int Id { get; set; }
    public string AddressName { get; set; }
    public string Street { get; set; }
    public string Neighborhood { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}