using Microsoft.AspNetCore.Identity;
using SpaceshipAPI;

namespace SpaceShipAPI.Services.Authentication;

public interface ITokenService
{
    string CreateToken(UserEntity user, string role);
}