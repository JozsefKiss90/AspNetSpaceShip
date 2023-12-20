using SpaceshipAPI;
using SpaceShipAPI.Model.DTO;
using SpaceshipAPI.Model.Ship;
using SpaceshipAPI.Spaceship.Model.Station;

public interface ISpaceStationRepository : IDisposable
{
    Task<IEnumerable<SpaceStationDTO>> GetAllAsync();
    Task<SpaceStation> GetByIdAsync(long id);
    Task<SpaceStation> GetByUserAsync(UserEntity user);
    Task<SpaceStation>  CreateAsync(SpaceStation spaceStation);
    Task UpdateAsync(SpaceStation spaceStation);
    Task DeleteAsync(long id);
}
