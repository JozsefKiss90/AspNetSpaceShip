namespace SpaceShipAPI.Services.Authentication;
public record AuthResult(
    bool Success,
    string Email,
    string UserName,
    string Token,
    string Role
    )
{
    //Error code - error message
    public readonly Dictionary<string, string> ErrorMessages = new();
}