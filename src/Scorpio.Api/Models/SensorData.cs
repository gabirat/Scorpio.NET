using System;
using System.ComponentModel.DataAnnotations;

namespace Scorpio.Api.Models
{
    public class SensorData : EntityBase
    {
        [Required]
        public string SensorKey { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        public string Value { get; set; }

        public override string ToString() => SensorKey ?? string.Empty;
    }
}
