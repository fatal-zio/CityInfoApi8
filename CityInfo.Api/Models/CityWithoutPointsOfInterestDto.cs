namespace CityInfo.Api.Models
{
    /// <summary>
    /// A City without Points of Interest
    /// </summary>
    public class CityWithoutPointsOfInterestDto
    {
        /// <summary>
        /// The Id of the City
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The Name of the City
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// The Description of the City
        /// </summary>
        public string? Description { get; set; }
    }
}
