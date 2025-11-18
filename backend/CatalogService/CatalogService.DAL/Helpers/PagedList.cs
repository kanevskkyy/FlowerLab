using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.Helpers
{
    public class PagedList<T>
    {
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public List<T> Items { get; private set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(IEnumerable<T> items, int count, int page, int pageSize)
        {
            Items = items.ToList();
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = page;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}
