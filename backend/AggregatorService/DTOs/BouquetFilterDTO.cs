namespace AggregatorService.DTOs
{
    public class BouquetFilterDTO
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public string? SortBy { get; set; }
        public string? Name { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public List<string>? FlowerIds { get; set; }
        public List<string>? EventIds { get; set; }
        public List<string>? RecipientIds { get; set; }
        public List<string>? SizeIds { get; set; }
        public List<int>? Quantities { get; set; }
    }
}
