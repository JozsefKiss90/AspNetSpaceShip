using SpaceShipAPI.Model.Location;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceShipAPI.Repository
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> FindAllByUserAndResourceTypeInAndResourceReserveGreaterThanAsync(
            string userId,
            IEnumerable<ResourceType> resourceTypes,
            int reserveGreaterThan);

        Task<Location> GetByIdAsync(long id);

        Task<IEnumerable<Location>> GetAllAsync();

        Task<Location> CreateAsync(Location location);

        Task<Location> UpdateAsync(Location location);

        Task<bool> DeleteAsync(long id);
    }

}