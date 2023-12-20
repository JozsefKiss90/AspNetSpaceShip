using SpaceshipAPI.Spaceship.Model.Station;

namespace SpaceShipAPI.Model.DTO;

public class SpaceStationDataDTO
{
    public long Id { get; }
    public string Name { get; }

    public SpaceStationDataDTO(long id, string name)
    {
        Id = id;
        Name = name;
    }

    public SpaceStationDataDTO(SpaceStation station)
        : this(station.Id, station.Name)
    {
    }
}

