using Microsoft.EntityFrameworkCore;
using SpaceShipAPI;
using SpaceShipAPI.Database;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship;

public class SpaceShipRepository : ISpaceShipRepository
{
    private readonly AppDbContext _context;

    public SpaceShipRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SpaceShip>> GetAllAsync()
    {
        var spaceShips = await _context.Set<SpaceShip>().ToListAsync();
        //return spaceShips.Select(s => new ShipDTO(s));
        return spaceShips;
    }
    
    public async Task<SpaceShip> GetByIdAsync(long id)
    {
        var spaceShip = await _context.Set<SpaceShip>()
            //.Include(s => s.CurrentMission) // Include related data
            .FirstOrDefaultAsync(s => s.Id == id);

        return spaceShip; // != null ? new ShipDTO(spaceShip) : null;
    }
    
    public async Task<IEnumerable<SpaceShip>> GetAllByIdAsync(long id)
    {
        var spaceShipsByUser =  _context.Set<SpaceShip>()
            .Where(s => s.Id == id).ToList();

        return spaceShipsByUser;
    }

    public async Task<IEnumerable<SpaceShip>> GetByStationIdAsync(long stationId)
    {
        var minerShips = await _context.MinerShips
            .Where(ship => ship.SpaceStationId == stationId)
            .ToListAsync();

        var scoutShips = await _context.ScoutShips
            .Where(ship => ship.SpaceStationId == stationId)
            .ToListAsync();

        var allShips = minerShips.Cast<SpaceShip>().Concat(scoutShips.Cast<SpaceShip>());
        
        var shipDTOs = allShips.Select(ship => new ShipDTO(ship));

        return allShips;
    }
    
    public async Task<SpaceShip> CreateAsync(SpaceShip spaceShip)
    {
        _context.Add(spaceShip);
        await _context.SaveChangesAsync();
        return spaceShip;
    }


    public async Task<SpaceShip> UpdateAsync(SpaceShip spaceShip)
    {
        _context.Entry(spaceShip).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return await GetByIdAsync(spaceShip.Id);
    }

    public async Task DeleteAsync(long id)
    {
        var spaceShip = await _context.Set<SpaceShip>()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (spaceShip != null)
        {
            _context.Remove(spaceShip);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<Dictionary<ResourceType, int>> GetShipCost(ShipType shipType) {
        if (shipType != null) { 
            return shipType.GetCost();
        } else {
            return null;
        }
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}