using System.ComponentModel.DataAnnotations;

namespace Scorpio.Api.Paging
{
    public class PageParam
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ItemsPerPage { get; set; }

        public PageParam(int pageNumber, int itemsPerPage)
        {
            PageNumber = pageNumber;
            ItemsPerPage = itemsPerPage;
        }

        // Required for serialization
        public PageParam()
        {
        }
    }
}
