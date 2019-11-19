using System.ComponentModel.DataAnnotations;

namespace Scorpio.Api.Models
{
    public class Sensor : EntityBase
    {
        [Required]
        public string SensorKey { get; set; }

        [Required]
        public string Name { get; set; }


        public string Unit { get; set; }

        public override string ToString() => Name ?? string.Empty;
    }
}
