namespace Scorpio.Api.Models
{
    public class UiConfiguration : EntityBase
    {
        public string ParentId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
    }
}
