using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Scorpio.Api.Models
{
    public class Stream : EntityBase
    {
        [Required]
        [MinLength(2)]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [RegularExpression(".+://.*|localhost.*")]
        [JsonProperty("uri")]
        public string Uri { get; set; }

        public override string ToString() => $"Name: {Name} Uri: {Uri}";
    }
}
