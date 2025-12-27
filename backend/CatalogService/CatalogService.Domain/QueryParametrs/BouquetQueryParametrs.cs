using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Domain.QueryParametrs
{
    public class BouquetQueryParameters
    {
        public List<Guid> SizeIds { get; set; } = new();
        public List<Guid> EventIds { get; set; } = new();
        public List<Guid> RecipientIds { get; set; } = new();
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<int> Quantities { get; set; } = new();
        public List<Guid> FlowerIds { get; set; } = new();
        public string? Name { get; set; }
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string ToCacheKey()
        {
            var sb = new StringBuilder("bouquets:");

            if (!string.IsNullOrEmpty(Name))
                sb.Append($"name:{Name}:");

            if (MinPrice.HasValue)
                sb.Append($"minprice:{MinPrice}:");

            if (MaxPrice.HasValue)
                sb.Append($"maxprice:{MaxPrice}:");

            if (SizeIds?.Any() == true)
                sb.Append($"sizes:{string.Join(",", SizeIds.OrderBy(x => x))}:");

            if (EventIds?.Any() == true)
                sb.Append($"events:{string.Join(",", EventIds.OrderBy(x => x))}:");

            if (RecipientIds?.Any() == true)
                sb.Append($"recipients:{string.Join(",", RecipientIds.OrderBy(x => x))}:");

            if (Quantities?.Any() == true)
                sb.Append($"quantities:{string.Join(",", Quantities.OrderBy(x => x))}:");

            if (FlowerIds?.Any() == true)
                sb.Append($"flowers:{string.Join(",", FlowerIds.OrderBy(x => x))}:");

            sb.Append($"sort:{SortBy ?? "default"}:");
            sb.Append($"page:{Page}:");
            sb.Append($"size:{PageSize}");

            return sb.ToString();
        }
    }
}