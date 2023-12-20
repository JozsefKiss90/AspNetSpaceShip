using System.Security;
using SpaceShipAPI.Model.Mission;
using SpaceShipAPI.Repository;
using System.Security.Claims;
using SpaceShipAPI.Model.DTO;
using SpaceShipAPI.Model.DTO.Mission;
using SpaceShipAPI.Model.DTO.MissionDTO;
using SpaceShipAPI.Model.DTO.MissionDTOs;
using SpaceShipAPI.Model.Ship;

namespace SpaceShipAPI.Services
{
    public class MissionService
    {
        private readonly IMissionRepository _missionRepository;
        private readonly MissionFactory _missionFactory;
        private readonly ISpaceShipRepository _spaceShipRepository;
        private readonly ILocationRepository _locationRepository;

        public MissionService(
            IMissionRepository missionRepository, 
            MissionFactory missionFactory, 
            ISpaceShipRepository spaceShipRepository,
            ILocationRepository locationRepository)
        {
            _missionRepository = missionRepository;
            _missionFactory = missionFactory;
            _spaceShipRepository = spaceShipRepository;
            _locationRepository = locationRepository;
        }

        public async Task<IEnumerable<MissionDTO>> GetAllActiveMissionsForCurrentUserAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var missions = await _missionRepository.GetMissionsByUserIdAndCurrentStatusNotAsync(userId, MissionStatus.ARCHIVED);
            return missions.Select(m => UpdateAndConvert(m));
        }
        public async Task<MissionDetailDTO> GetMissionByIdAsync(long id, ClaimsPrincipal user)
        {
            var mission = await GetMissionByIdAndCheckAccessAsync(id, user);
            var missionManager = _missionFactory.GetMissionManager(mission);
            if ( missionManager.UpdateStatus())
            {
                await _missionRepository.UpdateAsync(mission);
            }
            return missionManager.GetDetailedDTO();
        }
        public async Task<MissionDetailDTO> StartNewMiningMission(NewMiningMissionDTO newMissionDTO, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var spaceShip = await _spaceShipRepository.GetByIdAsync(newMissionDTO.shipId);
            
            if (spaceShip == null)
            {
                throw new Exception($"No ship found with id {newMissionDTO.shipId}");
            }

            if (spaceShip.UserId != userId)
            {
                throw new SecurityException("You don't have authority to send this ship.");
            }

            if (!(spaceShip is MinerShip))
            {
                throw new ArgumentException("Ship is not a miner ship.");
            }

            var location = await _locationRepository.GetByIdAsync(newMissionDTO.locationId);

            if (location == null)
            {
                throw new Exception($"No location found with id {newMissionDTO.locationId}");
            }

            var mission = _missionFactory.StartNewMiningMission((MinerShip)spaceShip, location, newMissionDTO.activityTime);
            mission = (MiningMission) await _missionRepository.CreateAsync(mission);
            var missionManager = _missionFactory.GetMissionManager(mission);
            missionManager.UpdateStatus();
            await _missionRepository.CreateAsync(mission);
            return missionManager.GetDetailedDTO();
        }
        
        public MissionDetailDTO StartNewScoutingMission(NewScoutingMissionDTO newMissionDTO, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
            // Fetch the SpaceShip by ID
            var spaceShip = _spaceShipRepository.GetByIdAsync(newMissionDTO.ShipId).Result;

            if (spaceShip == null)
            {
                throw new Exception($"No ship found with id {newMissionDTO.ShipId}");
            }

            if (spaceShip.UserId != userId)
            {
                throw new SecurityException("You don't have authority to send this ship.");
            }

            if (!(spaceShip is ScoutShip))
            {
                throw new ArgumentException("Ship is not a scout ship.");
            }

            var mission = _missionFactory.StartNewScoutingMission(
                (ScoutShip)spaceShip,
                newMissionDTO.Distance,
                newMissionDTO.ActivityTime,
                newMissionDTO.TargetResource,
                newMissionDTO.PrioritizingDistance);

            mission = (ScoutingMission)_missionRepository.CreateAsync(mission).Result;

            var missionManager = _missionFactory.GetMissionManager(mission);
            missionManager.UpdateStatus();
            
            _missionRepository.CreateAsync(mission).Wait();

            return missionManager.GetDetailedDTO();
        }


        private MissionDTO UpdateAndConvert(Mission mission)
        {
            var missionManager = _missionFactory.GetMissionManager(mission);
            if (missionManager.UpdateStatus())
            {
                _missionRepository.UpdateAsync(mission);
            }
            return new MissionDTO(mission);
        }
        private Mission Update(Mission mission)
        {
            var missionManager = _missionFactory.GetMissionManager(mission);
            _missionRepository.UpdateAsync(mission);
            return mission;
        }
        
        public async Task<MissionDetailDTO> ArchiveMission(long id, ClaimsPrincipal user)
        {
            var mission = await GetMissionByIdAndCheckAccessAsync(id, user);
            var missionManager = _missionFactory.GetMissionManager(mission);
            if (missionManager.ArchiveMission())
            {
                _missionRepository.UpdateAsync(mission).Wait();
                return missionManager.GetDetailedDTO();
            }
            return null;
        }

        
        public async Task<MissionDetailDTO> AbortMission(long id, ClaimsPrincipal user)
        {
            var mission = await GetMissionByIdAndCheckAccessAsync(id, user);
            var missionManager = _missionFactory.GetMissionManager(mission);
            if (missionManager.UpdateStatus())
            {
                _missionRepository.UpdateAsync(mission).Wait();
            }
            if (missionManager.AbortMission())
            {
                _missionRepository.UpdateAsync(mission).Wait();
                return missionManager.GetDetailedDTO();
            }
            return null;
        }
        
        private async Task<Mission> GetMissionByIdAndCheckAccessAsync(long id, ClaimsPrincipal user)
        {
            var mission = await _missionRepository.GetByIdAsync(id);
            if (mission == null)
            {
                throw new KeyNotFoundException($"No mission found with id {id}");
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user.IsInRole("Admin") || mission.UserId == userId)
            {
                return mission;
            }
            else
            {
                throw new UnauthorizedAccessException("You don't have authority to access this mission");
            }
        }
    }
}
