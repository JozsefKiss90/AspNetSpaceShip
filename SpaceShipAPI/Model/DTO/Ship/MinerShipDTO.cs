using SpaceShipAPI.Model.DTO.Ship.Part;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;

namespace SpaceShipAPI.Model.DTO.Ship;

public class MinerShipDTO : ShipDetailDTO
{
    public DrillDTO Drill { get; }
    public ShipStorageDTO Storage { get; }

    public MinerShipDTO(long id, string name, ShipColor color, ShipType type, Model.Mission.Mission mission, EngineDTO engine, ShieldDTO shield, DrillDTO drill, ShipStorageDTO storage)
        : base(id, name, color, type, mission, engine, shield) 
    {
        Drill = drill;
        Storage = storage;
    }
}
