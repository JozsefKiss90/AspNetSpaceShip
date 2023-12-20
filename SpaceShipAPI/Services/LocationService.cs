using System.Security.Claims;
using SpaceShipAPI.Model.Location;
using SpaceShipAPI.Repository;
using SpaceShipAPI.Model.DTO;

namespace SpaceShipAPI.Services
{
    public class LocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository ?? throw new ArgumentNullException(nameof(locationRepository));
        }

        public async Task<Location> GetByIdAsync(long id)
        {
            return await _locationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _locationRepository.GetAllAsync();
        }

        public async Task<Location> CreateAsync(Location location)
        {
            return await _locationRepository.CreateAsync(location);
        }

        public async Task<Location> UpdateAsync(Location location)
        {
            return await _locationRepository.UpdateAsync(location);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _locationRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<LocationDTO>> GetAllLocationsForCurrentUserAsync(
            ClaimsPrincipal user,
            List<ResourceType> resourceTypes,
            int reserveGreaterThan
            )
        {
            string userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var locations = await _locationRepository
                .FindAllByUserAndResourceTypeInAndResourceReserveGreaterThanAsync(
                    userId,
                    resourceTypes,
                    reserveGreaterThan);

            return locations.Select(location => new LocationDTO(location));
        }
    }
}