using SpaceShipAPI.Model.DTO.Ship.Part;
using SpaceShipAPI.Model.Ship;
namespace SpaceShipAPI.Model.DTO.Ship;

public abstract class ShipDetailDTO : ShipDTO 
{
    public EngineDTO Engine { get; }
    public ShieldDTO Shield { get; }

    public ShipDetailDTO(long id, string name, ShipColor color, ShipType type, Model.Mission.Mission? mission, EngineDTO engine, ShieldDTO shield)
        : base(id, name, color, type, mission?.Id ?? 0L) 
    {
        Engine = engine;
        Shield = shield;
    }
}
