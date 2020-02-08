using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Scorpio.Api.Models
{
    public class Position : EntityBase
    {
        /// <summary>
        /// Describes type of point to put
        /// </summary>
        [Required]
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// GPS position property: latitude
        /// </summary>
        [Required]
        [JsonProperty("lat")]
        public string Latitude { get; set; }

        /// <summary>
        /// GPS position property: longitude
        /// </summary>
        [Required]
        [JsonProperty("lng")]
        public string Longitude { get; set; }
    }
}
