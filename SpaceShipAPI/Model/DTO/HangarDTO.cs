using System.Collections.Generic;
using System.Linq;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceshipAPI.Spaceship.Model.Station;

namespace SpaceShipAPI.Model.DTO;

public class HangarDTO
{
   public HashSet<ShipDTO> Ships { get; init; }
   public int Level { get; init; }
   public int Capacity { get; init; }
   public int FreeDocks { get; set; }  
   public bool FullyUpgraded { get; init; }

   public HangarDTO(HashSet<ShipDTO> ships, int level, int capacity, int freeDocks, bool fullyUpgraded)
   {
      Ships = ships;
      Level = level;
      Capacity = capacity;
      FreeDocks = freeDocks;
      FullyUpgraded = fullyUpgraded;
   }
}
public class HangarDTOFactory
{
   public static HangarDTO Create(IHangarManager hangarManager)
   {
      var ships = GetAllShipDTOs(hangarManager);
      int level = hangarManager.GetCurrentLevel();
      int capacity = hangarManager.GetCurrentCapacity();
      int freeDocks = hangarManager.GetCurrentAvailableDocks();
      bool fullyUpgraded = hangarManager.IsFullyUpgraded();

      return new HangarDTO(ships, level, capacity, freeDocks, fullyUpgraded);
   }

   private static HashSet<ShipDTO> GetAllShipDTOs(IHangarManager hangar)
   {
    
      return hangar.GetAllShips().Select(ship => new ShipDTO(ship)).ToHashSet();
   }
}
