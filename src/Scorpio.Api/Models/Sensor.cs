using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Scorpio.Api.Models
{
    public class Sensor : EntityBase
    {
        [Required]
        [JsonProperty("sensorKey")]
        public string SensorKey { get; set; }

        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        public override string ToString() => Name ?? string.Empty;
    }
}
