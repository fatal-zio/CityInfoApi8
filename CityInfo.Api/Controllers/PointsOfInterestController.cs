using AutoMapper;
using CityInfo.Api.Models;
using CityInfo.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Api.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
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

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            return (pointOfInterest == null) ? NotFound() :
                Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            //var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound();
            //}

            ////for demo purposes
            //var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            //var finalPointOfInterest = new PointOfInterestDto()
            //{
            //    Id = ++maxPointOfInterestId,
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description,
            //};

            //city.PointsOfInterest.Add(finalPointOfInterest);

            //return CreatedAtRoute("GetPointOfInterest", new
            //{
            //    cityId,
            //    pointOfInterestId = finalPointOfInterest.Id
            //},
            //finalPointOfInterest);
            return Ok();
        }

        [HttpPut("{pointOfInterestId}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            //var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var existingPointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);

            //if (existingPointOfInterest == null)
            //{
            //    return NotFound();
            //}

            //existingPointOfInterest.Name = pointOfInterest.Name;
            //existingPointOfInterest.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            //var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var existingPointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);

            //if (existingPointOfInterest == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            //{
            //    Name = existingPointOfInterest.Name,
            //    Description = existingPointOfInterest.Description,
            //};

            //patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (!TryValidateModel(pointOfInterestToPatch))
            //{
            //    return BadRequest(ModelState);
            //}

            //existingPointOfInterest.Name = pointOfInterestToPatch.Name;
            //existingPointOfInterest.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            //var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var existingPointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);

            //if (existingPointOfInterest == null)
            //{
            //    return NotFound();
            //}

            //city.PointsOfInterest.Remove(existingPointOfInterest);

            //_mailService.Send("Point of interest deleted.",
            //    $"Point of interest: {existingPointOfInterest.Name} with id: {existingPointOfInterest.Id} was deleted.");

            return NoContent();
        }
    }
}
