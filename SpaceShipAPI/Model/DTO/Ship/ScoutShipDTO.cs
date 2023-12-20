using SpaceShipAPI.Model.DTO.Ship.Part;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;

namespace SpaceShipAPI.Model.DTO.Ship;

public class ScoutShipDTO : ShipDetailDTO
{
    public ScannerDTO Scanner { get; }

    public ScoutShipDTO(long id, string name, ShipColor color, ShipType type, Model.Mission.Mission mission, EngineDTO engine, ShieldDTO shield, ScannerDTO scanner)
        : base(id, name, color, type, mission, engine, shield)
    {
        Scanner = scanner;
    }
}
