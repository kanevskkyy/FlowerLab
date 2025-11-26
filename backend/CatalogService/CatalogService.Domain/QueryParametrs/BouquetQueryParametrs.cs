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

        public string? BouquetName { get; set; }
        public string? FlowerName { get; set; }

        public string? SortBy { get; set; } 

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
