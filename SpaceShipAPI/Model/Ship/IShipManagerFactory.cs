using SpaceshipAPI.Model.Ship;

namespace SpaceShipAPI.Model.Ship;

public interface IShipManagerFactory 
{
    ISpaceShipManager GetSpaceShipManager(SpaceShip ship);
}
