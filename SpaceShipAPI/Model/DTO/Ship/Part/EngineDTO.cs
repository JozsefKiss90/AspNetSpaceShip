using SpaceShipAPI.Model.Ship.ShipParts;

namespace SpaceShipAPI.Model.DTO.Ship.Part;

    public record EngineDTO(int Level, int Speed, bool FullyUpgraded)
    {
        public EngineDTO(EngineManager engineManager) 
            : this(engineManager.GetCurrentLevel(), engineManager.Speed, engineManager.IsFullyUpgraded())
        {
        }
    }
