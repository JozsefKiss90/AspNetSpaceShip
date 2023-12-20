using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SpaceShipAPI.Model.Location;
using SpaceShipAPI.Services;
using System.Threading.Tasks;
using SpaceShipAPI.Model.DTO;

namespace SpaceShipAPI.Controllers
{
    [ApiController]
    [Route("api/v1/location")]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
        }

        [HttpGet]
        public async Task<ActionResult<List<LocationDTO>>> GetAllLocationsForCurrentUser(
            [FromQuery] bool includeDepleted = false,
            [FromQuery] List<ResourceType> resources = null,
            [FromQuery] string sort = null)
        {
            int reserveGreaterThan = includeDepleted ? -1 : 0;
            
            resources ??= new List<ResourceType>(Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>());

            string orderBy;
            bool asc;

            switch (sort)
            {
                case "discoveredASC":
                    orderBy = "discovered";
                    asc = true;
                    break;
                case "nameASC":
                    orderBy = "name";
                    asc = true;
                    break;
                case "nameDESC":
                    orderBy = "name";
                    asc = false;
                    break;
                case "reserveASC":
                    orderBy = "resourceReserve";
                    asc = true;
                    break;
                case "reserveDESC":
                    orderBy = "resourceReserve";
                    asc = false;
                    break;
                case "distanceASC":
                    orderBy = "distanceFromStation";
                    asc = true;
                    break;
                case "distanceDESC":
                    orderBy = "distanceFromStation";
                    asc = false;
                    break;
                default:
                    orderBy = "discovered";
                    asc = false;
                    break;
            }

            var locations = await _locationService.GetAllLocationsForCurrentUserAsync(User, resources, reserveGreaterThan);

            return Ok(locations);
        }
    }
}
