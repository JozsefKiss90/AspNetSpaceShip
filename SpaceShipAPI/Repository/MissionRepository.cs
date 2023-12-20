using SpaceShipAPI.Database;
using SpaceShipAPI.Model.Mission;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MissionRepository : IMissionRepository
{
    private readonly AppDbContext _context;

    public MissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Mission>> GetAllAsync()
    {
        return await _context.Missions.ToListAsync();
    }

    public async Task<Mission> GetByIdAsync(long id)
    {
        return await _context.Missions.FindAsync(id);
    }

    public async Task<Mission> CreateAsync(Mission mission)
    {
        _context.Missions.Add(mission);
        await _context.SaveChangesAsync();
        return mission;
    }

    public async Task UpdateAsync(Mission mission)
    {
        _context.Entry(mission).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var mission = await _context.Missions.FindAsync(id);
        if (mission != null)
        {
            _context.Missions.Remove(mission);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<IEnumerable<Mission>> GetMissionsByUserIdAndCurrentStatusNotAsync(String userId, MissionStatus missionStatus)
    {
        return await _context.Missions
            .Where(m => m.UserId == userId && m.CurrentStatus != missionStatus)
            .ToListAsync();
    }

    public async Task<IEnumerable<Mission>> GetMissionsByUserIdAndCurrentStatusAsync(String userId, MissionStatus missionStatus)
    {
        return await _context.Missions
            .Where(m => m.UserId == userId && m.CurrentStatus == missionStatus)
            .ToListAsync();
    }
}