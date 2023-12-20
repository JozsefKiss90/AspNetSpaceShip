using SpaceShipAPI.Model.Mission;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IMissionRepository
{
    Task<IEnumerable<Mission>> GetAllAsync();
    Task<Mission> GetByIdAsync(long id);
    Task<Mission> CreateAsync(Mission mission);
    Task UpdateAsync(Mission mission);
    Task DeleteAsync(long id);

    Task<IEnumerable<Mission>> GetMissionsByUserIdAndCurrentStatusNotAsync(String userId, MissionStatus missionStatus);
    Task<IEnumerable<Mission>> GetMissionsByUserIdAndCurrentStatusAsync(String userId, MissionStatus missionStatus);
}