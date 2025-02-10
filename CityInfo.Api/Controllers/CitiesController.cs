using Asp.Versioning;
using AutoMapper;
using CityInfo.Api.Models;
using CityInfo.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.Api.Controllers
{
    /// <summary>
    /// Controller for returning City information
    /// </summary>
    /// <param name="cityInfoRepository">Repository for IO actions</param>
    /// <param name="mapper">IMapper for use of AutoMapper</param>
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/cities")]
    [ApiVersion(1)]
    [ApiVersion(2)]
    public class CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper) : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository =
            cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));

        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        const int maxCitiesPageSize = 20;

        /// <summary>
        /// Get a list of Cities matching the passed criteria
        /// </summary>
        /// <param name="name">Exact match City Name</param>
        /// <param name="searchQuery">Full text search which is applied to City Name and Description. Includes partial matches.</param>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10, max 20)</param>
        /// <returns>Returns a collection of cities matching the criteria without Points of Interest</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }

        /// <summary>
        /// Get a City by Id
        /// </summary>
        /// <param name="id">The Id of the City to Get</param>
        /// <param name="includePointsOfInterest">Whether or not to include Points of Interest</param>
        /// <returns>A city with or without points of interest</returns>
        /// <response code="200">Returns the requested city</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

            if (city == null)
            {
                return NotFound();
            }

            return includePointsOfInterest ? Ok(_mapper.Map<CityDto>(city)) : 
                Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
