using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ReviewService.Domain.Helpers
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
        public static async Task<PagedList<T>> ToPagedListAsync(IFindFluent<T, T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            int count = (int)await source.CountDocumentsAsync(cancellationToken: cancellationToken);
            List<T> items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return PagedList<T>.Create(items, count, pageNumber, pageSize);
        }
    }
}
