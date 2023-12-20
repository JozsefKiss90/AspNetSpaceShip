using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShipAPI.DTO;
using SpaceShipAPI.Model;

namespace SpaceShipAPI.Controllers
{
    [Authorize(Roles = "ROLE_ADMIN")]
    [Route("api/v1/level")]
    [ApiController]
    public class LevelController : ControllerBase
    {
        private readonly LevelService _levelService;

        public LevelController(LevelService levelService)
        {
            _levelService = levelService;
        }

        [HttpGet("types")]
        public ActionResult<IEnumerable<UpgradeableType>> GetLevelTypes()
        {
            var levelTypes = _levelService.GetLevelTypes();
            return Ok(levelTypes);
        }

        [HttpGet]
        public ActionResult<IEnumerable<LevelDTO>> GetLevelsByType([FromQuery] UpgradeableType type)
        {
            var levels = _levelService.GetLevelsByType(type);
            return Ok(levels);
        }

        [HttpPatch("{id}")]
        public ActionResult<LevelDTO> UpdateLevelById(long id, [FromBody] NewLevelDTO newLevelDTO)
        {
            var updatedLevel = _levelService.UpdateLevelById(id, newLevelDTO);
            if (updatedLevel == null)
            {
                return NotFound();
            }

            return Ok(updatedLevel);
        }

        [HttpPost]
        public ActionResult<LevelDTO> AddNewLevel([FromBody] NewLevelDTO newLevelDTO)
        {
            var addedLevel = _levelService.AddNewLevel(newLevelDTO);
            return CreatedAtAction(nameof(GetLevelsByType), new { type = addedLevel.Type }, addedLevel);
        }

        [HttpDelete]
        public ActionResult<bool> DeleteLastLevelOfType([FromQuery] UpgradeableType type)
        {
            var isDeleted = _levelService.DeleteLastLevelOfType(type);
            if (!isDeleted)
            {
                return NotFound();
            }

            return Ok(true);
        }
    }
}