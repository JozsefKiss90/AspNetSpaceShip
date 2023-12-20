using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceShipAPI.Database;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Services;

namespace SpaceShipAPI.Controllers;

[Route("api/v1/ship/")] 
[ApiController]
[Authorize]
public class SpaceShipController : ControllerBase
{
    private readonly IShipService  _shipService;
    private readonly ILogger<SpaceShipController> _logger;
    private readonly AppDbContext _userContext; 

    public SpaceShipController(IShipService shipService, AppDbContext userContext, ILogger<SpaceShipController> logger)
    {
        _shipService = shipService ?? throw new ArgumentNullException(nameof(shipService));
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {

        var spaceShips = await _shipService.GetAllShips(User);
        return Ok(spaceShips);
    }
    
    [HttpGet("station/{stationId}", Name = "GetShipByStationId")]
    public async Task<IActionResult> GetShipsByStationId(long stationId)
    {
        var spaceShips = await _shipService.GetShipsByStationAsync(stationId, User);
        return Ok(spaceShips);
    }

    [HttpGet("{id}", Name = "GetShip")] 
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var spaceShip = await _shipService.GetShipDetailsByIdAsync(id);
        if (spaceShip == null)
        {
            return NotFound();
        }

        return Ok(spaceShip);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] NewShipDTO newShipDTO)
    {
        try
        {
            if (newShipDTO == null)
            {
                return BadRequest("No ship given!"); 
            }
            
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && c.Value != "TokenForTheApiWithAuth");
            var userId = userIdClaim?.Value;
            var userExists = await _userContext.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            { 
                return StatusCode(404, "Invalid User ID");
            }

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }
            _logger.LogInformation($"User ID: {userId}");
                       
            ShipDTO spaceShip = await _shipService.CreateShip(newShipDTO, User);
            if (spaceShip == null)
            { 
                return StatusCode(405, "Unable to create ship");
            }
            return Ok(spaceShip);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a ship");
            return StatusCode(500, "Internal Server Error");
        }
    }
    
    [HttpGet("color")]
    public async Task<IActionResult> GetShipColors()
    {
        ShipColor[] colors = _shipService.getColors();
        return Ok(colors);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] SpaceShip spaceShip)
    {
        if (spaceShip == null || id != spaceShip.Id)
        {
            return BadRequest();
        }

        var existingSpaceShip = await _shipService.GetByIdAsync(id);
        if (existingSpaceShip == null)
        {
            return NotFound();
        }

        await _shipService.UpdateAsync(spaceShip);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var existingSpaceShip = await _shipService.GetByIdAsync(id);
        if (existingSpaceShip == null)
        {
            return NotFound();
        }

        await _shipService.DeleteAsync(id);

        return NoContent();
    }
    
    [HttpGet( "cost/{shipType}")]
    public async Task<IActionResult> getMinerShipCost(String shipType) {
        var type = (ShipType)Enum.Parse(typeof(ShipType), shipType.ToUpper());

        var costResult = await _shipService.GetShipCost(type);
        return Ok(costResult);
    }
}
