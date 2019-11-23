using System.Collections.Generic;

namespace Scorpio.Api.Paging
{
    public class PagedList<T>
    {
        public IEnumerable<T> Values { get; set; }
        public long TotalItems { get; set; }
        public long ItemsPerPage { get; set; }
        public int Page { get; set; }

        public static PagedList<T> Build(IList<T> items, long total, long perPage, int page)
        {
            return new PagedList<T>
            {
                TotalItems = total,
                Values = items,
                ItemsPerPage = perPage,
                Page = page
            };
        }
    }
}
