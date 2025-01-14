using CityInfo.Api.Models;

namespace CityInfo.Api
{
    public class CitiesDataStore
    {
        public CitiesDataStore()
        {
            Cities =
            [
                new()
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "The one with the big park."
                },
                new()
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never really finished."
                },
                new()
                {
                    Id=3,
                    Name = "Paris",
                    Description = "The one with the really big tower."
                }
            ];
        }

        public List<CityDto> Cities { get; set; }
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
    }
}
