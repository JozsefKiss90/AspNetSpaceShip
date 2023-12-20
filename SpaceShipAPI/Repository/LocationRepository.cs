using Microsoft.EntityFrameworkCore;
using SpaceShipAPI.Database;
using SpaceShipAPI.Model.Location;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceShipAPI.Repository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;

        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> FindAllByUserAndResourceTypeInAndResourceReserveGreaterThanAsync(
            string userId,
            IEnumerable<ResourceType> resourceTypes,
            int reserveGreaterThan)
        {
            return await _context.Locations
                .Where(loc => loc.User.Id == userId &&
                              resourceTypes.Contains(loc.ResourceType) &&
                              loc.ResourceReserve > reserveGreaterThan)
                .ToListAsync();
        }

        public async Task<Location> GetByIdAsync(long id)
        {
            return await _context.Locations.FindAsync(id);
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location> CreateAsync(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<Location> UpdateAsync(Location location)
        {
            _context.Entry(location).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
                return false;

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
