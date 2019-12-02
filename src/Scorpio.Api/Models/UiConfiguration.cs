using System.ComponentModel.DataAnnotations;

namespace Scorpio.Api.Models
{
    public class UiConfiguration : EntityBase
    {
        public string ParentId { get; set; }

        [Required]
        [RegularExpression("^page$|^member$", ErrorMessage = "Type must be either 'page' or 'member")] 
        public string Type { get; set; }

        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        public string Data { get; set; }

        public override string ToString() => Name ?? string.Empty;
    }
}
