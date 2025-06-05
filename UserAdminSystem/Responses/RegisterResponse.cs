namespace UserAdminSystem.Responses;

public class RegisterResponse
{
    // ? Validar esta respuesta si es necesaria
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public byte EmailConfimed { get; set; }
}