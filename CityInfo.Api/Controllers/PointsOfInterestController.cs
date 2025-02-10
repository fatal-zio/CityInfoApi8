using Asp.Versioning;
using AutoMapper;
using CityInfo.Api.Models;
using CityInfo.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Api.Controllers
{
    /// <summary>
    /// Provides CRUD functionality for Points of Interest
    /// </summary>
    /// <param name="logger">An implementation of ILogger</param>
    /// <param name="mailService">An implementation of IMailService</param>
    /// <param name="cityInfoRepository">City Info Repository</param>
    /// <param name="mapper">Instance of IMapper for AutoMapper</param>
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    [Authorize(Policy = "MustBeFromNewYork")]
    [ApiController]
    [ApiVersion(2)]
    public class PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService,
        ICityInfoRepository cityInfoRepository, IMapper mapper) : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger =
            logger ?? throw new ArgumentNullException(nameof(logger));

        private readonly IMailService _mailService =
            mailService ?? throw new ArgumentNullException(nameof(mailService));

        private readonly ICityInfoRepository _cityInfoRepository =
            cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));

        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        /// <summary>
        /// Returns a list of Points of Interest for a City
        /// </summary>
        /// <param name="cityId">Integer Id for a given city</param>
        /// <returns>A collection of Points of Interest for a City</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            if (!string.IsNullOrEmpty(cityName) && !await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
            {
                return Forbid();
            }

            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex);
                return StatusCode(500, "A problem occured while handling your message");
            }
        }

        /// <summary>
        /// Get a single Point of Interest
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="pointOfInterestId"></param>
        /// <returns>A Point of Interest</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            return (pointOfInterestEntity == null) ? NotFound() :
                Ok(_mapper.Map<PointOfInterestDto>(pointOfInterestEntity));
        }

        /// <summary>
        /// Create a new Point of Interest for a City
        /// </summary>
        /// <param name="cityId">Id of a City</param>
        /// <param name="pointOfInterest">A new Point of Interest</param>
        /// <returns>A newly created Point of Interest, complete with new Id</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", 
                new
                {
                    cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                },
                createdPointOfInterestToReturn);
        }

        /// <summary>
        /// Updates a Point of Interest by replacing the existing one
        /// </summary>
        /// <param name="cityId">Id of a City</param>
        /// <param name="pointOfInterestId">Id for a Point of Interest</param>
        /// <param name="pointOfInterest">A new version of the given Point of Interest</param>
        /// <returns>No Content</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Update a Point of Interest using a JSON Patch Document
        /// </summary>
        /// <param name="cityId">Id of a City</param>
        /// <param name="pointOfInterestId">Id for a Point of Interest</param>
        /// <param name="patchDocument">JSON Patch Document to apply to a given Point of Interest</param>
        /// <returns>No Content</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a Point of Interest for a City
        /// </summary>
        /// <param name="cityId">Id of the City</param>
        /// <param name="pointOfInterestId">Id of the Point of interest</param>
        /// <returns>No Content</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send("Point of interest deleted.",
                $"Point of interest: {pointOfInterestEntity.Name} with id: {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
