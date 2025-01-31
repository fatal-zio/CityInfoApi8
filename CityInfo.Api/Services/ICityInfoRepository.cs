using CityInfo.Api.Entities;

namespace CityInfo.Api.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<City?> GetCityAsync(int cityId);
        Task<IEnumerable<PointOfInterest>> GetPointOfInterestForCityAsync(int cityId, bool includePointsOfInterest);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
    }
}
