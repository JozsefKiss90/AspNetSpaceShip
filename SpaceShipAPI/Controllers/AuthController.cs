using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpaceshipAPI;
using SpaceshipAPI.Contracts;
using SpaceShipAPI.Services;
using SpaceShipAPI.Services.Authentication;

namespace SpaceShipAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authenticationService;
    private readonly UserManager<UserEntity> _userManager;
    private readonly ISpaceStationService _spaceStationService;
    private readonly ILogger<AuthController> _logger;
    public AuthController(
        IAuthService authenticationService, 
        UserManager<UserEntity> userManager, 
        ISpaceStationService spaceStationService,
        ILogger<AuthController> logger
        )
    {
        _authenticationService = authenticationService;
        _userManager = userManager;
        _spaceStationService = spaceStationService;
        _logger = logger;
    }
    
    [HttpGet("TestAuthentication")]
    [Authorize] 
    public IActionResult TestAuthentication()
    {
        return Ok("Authentication successful");
    }
   /* [HttpPost("Register")]
    public async Task<ActionResult<RegistrationResponse>> Register1(RegistrationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authenticationService.RegisterAsync(request.Email, request.Username, request.Password, "User");

        if (!result.Success)
        {
            AddErrors(result);
            return BadRequest(ModelState);
        }

        return CreatedAtAction(nameof(Register), new RegistrationResponse(result.Email, result.UserName));
    }*/

    [HttpPost("Register")]
    public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var registrationResult = await _authenticationService.RegisterAsync(request.Email, request.Username, request.Password, "User");

        if (!registrationResult.Success)
        {
            AddErrors(registrationResult);
            return BadRequest(ModelState);
        }

        // Retrieve the user after successful registration
        var newUser = await _userManager.FindByNameAsync(request.Username);
        
        if (newUser == null)
        {
            // Handle the unexpected case where the user is not found after registration
            return StatusCode(500, "An error occurred after registration.");
        }

        // Create ClaimsPrincipal for the newly registered user
        var userClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(UserClaims(newUser), "local"));
     
        // Create a new space station for the user
        try
        {
            var spaceStationName = "Station_" + newUser.UserName; // Or any naming convention you prefer
            await _spaceStationService.CreateAsync(spaceStationName, userClaimsPrincipal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating space station for user {UserId}", newUser.Id);

            // Optional: Delete the user if space station creation fails
            await _userManager.DeleteAsync(newUser);

            // Return a specific error message
            return StatusCode(500, "Registration succeeded, but failed to create a space station.");
        }

        return CreatedAtAction(nameof(Register), new RegistrationResponse(newUser.Email, newUser.UserName));
    }
    
    private IEnumerable<Claim> UserClaims(UserEntity user)
    {
        return new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            
        };
    }


    [HttpPost("Login")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authenticationService.LoginAsync(request.Email, request.Password);

        if (!result.Success)
        {
            AddErrors(result);
            return BadRequest(ModelState);
        }
        
        return Ok(new AuthResponse(result.Email, result.UserName, result.Token));
    }

    private void AddErrors(AuthResult result)
    {
        foreach (var error in result.ErrorMessages)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }
    }
}