using System.ComponentModel.DataAnnotations;

namespace Scorpio.Api.Models
{
    public class Stream : EntityBase
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(".+://.*|localhost.*")]
        public string Uri { get; set; }

        public override string ToString() => $"Name: {Name} Uri: {Uri}";
    }
}
