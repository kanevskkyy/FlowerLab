using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.DAL.Helpers
{
    public class PagedList<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; } = new();

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList() { }

        public static PagedList<T> Create(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
        {
            return new PagedList<T>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = currentPage,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public static PagedList<T> ToPagedList<T>(List<T> source, int pageNumber, int pageSize)
        {
            int count = source.Count;
            List<T> items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PagedList<T>.Create(items, count, pageNumber, pageSize);
        }
    }
}
