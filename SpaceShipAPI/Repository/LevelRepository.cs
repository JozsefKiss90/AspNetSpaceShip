using SpaceShipAPI.Model;
using System.Collections.Generic;
using System.Linq;
using SpaceShipAPI.Database;
using SpaceShipAPI.Repository;

public class LevelRepository : ILevelRepository
{
    private readonly AppDbContext _context;

    public LevelRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public Level GetLevelByTypeAndLevel(UpgradeableType type, int level)
    {
        return _context.Levels.FirstOrDefault(l => l.Type == type && l.LevelValue == level);
    }

    public IEnumerable<Level> GetLevelsByType(UpgradeableType type)
    {
        return _context.Levels.Where(l => l.Type == type).ToList();
    }

    public Level GetLevelByTypeAndMax(UpgradeableType type, bool isMax)
    {
        return _context.Levels.FirstOrDefault(l => l.Type == type && l.Max == isMax);
    }

    public Level FindById(long id)
    {
        return _context.Levels.Find(id);
    }

    public void Save(Level level)
    {
        if (level.Id == 0)
        {
            _context.Levels.Add(level);
        }
        else
        {
            _context.Levels.Update(level);
        }
        _context.SaveChanges();
    }

    public void Delete(Level level)
    {
        _context.Levels.Remove(level);
        _context.SaveChanges();
    }
}