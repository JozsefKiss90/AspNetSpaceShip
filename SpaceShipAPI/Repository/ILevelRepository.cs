using SpaceShipAPI.DTO;

namespace SpaceShipAPI.Repository;

using SpaceShipAPI.Model;
using System;
using System.Collections.Generic;

public interface ILevelRepository
{

    Level GetLevelByTypeAndLevel(UpgradeableType type, int level);
    Level GetLevelByTypeAndMax(UpgradeableType type, bool isMax);
    IEnumerable<Level> GetLevelsByType(UpgradeableType type);
    Level FindById(long id);

    void Save(Level level);

    void Delete(Level level);
}

