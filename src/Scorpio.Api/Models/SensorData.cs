using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Scorpio.Api.Models
{
    public class SensorData : EntityBase
    {
        [Required]
        [JsonProperty("sensorKey")]
        public string SensorKey { get; set; }

        [Required]
        [JsonProperty("timeStamp")]
        public DateTime TimeStamp { get; set; }

        [Required]
        [JsonProperty("value")]
        public string Value { get; set; }

        [MaxLength(500)]
        [JsonProperty("comment")]
        public string Comment { get; set; }

        public override string ToString() => SensorKey ?? string.Empty;
    }
}
