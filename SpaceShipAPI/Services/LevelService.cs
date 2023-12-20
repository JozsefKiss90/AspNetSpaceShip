using SpaceShipAPI.DTO;
using SpaceShipAPI.Model;
using SpaceShipAPI.Repository;
using SpaceShipAPI.Services;

public class LevelService : ILevelService
{
    private readonly ILevelRepository _levelRepository;

    public LevelService(ILevelRepository levelRepository)
    {
        _levelRepository = levelRepository;
    }

    public List<UpgradeableType> GetLevelTypes()
    {
        return Enum.GetValues(typeof(UpgradeableType)).Cast<UpgradeableType>().ToList();
    }

    public Level GetLevelByTypeAndLevel(UpgradeableType type, int level)
    {
        return _levelRepository.GetLevelByTypeAndLevel(type, level) ?? throw new ArgumentException($"{type} has no level {level}.");
    }
    
    public List<LevelDTO> GetLevelsByType(UpgradeableType type)
    {
        var levels = _levelRepository.GetLevelsByType(type);
        return levels.Select(l => new LevelDTO(l)).ToList();
    }
    
    public LevelDTO UpdateLevelById(long id, NewLevelDTO newLevelDTO)
    {
        var level = FindById(id);
        level.Effect = newLevelDTO.Effect;
        level.Costs = new HashSet<LevelCost>(newLevelDTO.Cost.Select(kv => new LevelCost { Resource = kv.Key, Amount = kv.Value, LevelId = level.Id }));
        Save(level);
        return new LevelDTO(level);
    }
    
    public LevelDTO AddNewLevel(NewLevelDTO newLevelDTO)
    {
        var prevMaxLevel = _levelRepository.GetLevelByTypeAndMax(newLevelDTO.Type, true);
        var newMaxLevel = CreateNewMaxLevel(newLevelDTO, prevMaxLevel);

        if (prevMaxLevel != null)
        {
            prevMaxLevel.Max = false;
            Save(prevMaxLevel);
        }
        Save(newMaxLevel);
        return new LevelDTO(newMaxLevel);
    }
    public bool DeleteLastLevelOfType(UpgradeableType type)
    {
        var maxLevel = _levelRepository.GetLevelByTypeAndMax(type, true) ?? throw new InvalidOperationException($"No max level has been set for {type} type");
        if (maxLevel.LevelValue == 1)
        {
            throw new ArgumentException("The first level can't be deleted.");
        }

        var newMaxLevel = _levelRepository.GetLevelByTypeAndLevel(type, maxLevel.LevelValue - 1) ?? throw new InvalidOperationException($"Level sequence is incorrect for {type} type");
        newMaxLevel.Max = true;
        Delete(maxLevel);
        Save(newMaxLevel);
        return true;
    }

    private Level FindById(long id)
    {
        return _levelRepository.FindById(id) ?? throw new Exception("Level not found");
    }

    private void Save(Level level)
    {
        _levelRepository.Save(level);
    }

    private void Delete(Level level)
    {
        _levelRepository.Delete(level);
    }

    private Level CreateNewMaxLevel(NewLevelDTO newLevelDTO, Level prevMaxLevel)
    {
        return new Level
        {
            LevelValue = prevMaxLevel == null ? 1 : prevMaxLevel.LevelValue + 1,
            Type = newLevelDTO.Type,
            Effect = newLevelDTO.Effect,
            Costs = new HashSet<LevelCost>(newLevelDTO.Cost.Select(kv => new LevelCost { Resource = kv.Key, Amount = kv.Value })),
            Max = true
        };
    }
}
