using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SpaceShipAPI.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("claims")]
    public IActionResult GetClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(claims);
    }
    
    [HttpGet("roles")]
    public IActionResult GetUserRoles()
    {
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
        return Ok(roles);
    }

}
