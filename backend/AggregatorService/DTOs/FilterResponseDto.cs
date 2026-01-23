namespace AggregatorService.DTOs
{
    public class FilterResponseDto
    {
        public List<FilterSizeDto> SizeResponseList { get; set; } = new();
        public List<FilterEventDto> EventResponseList { get; set; } = new();
        public List<FilterRecipientDto> ReceivmentResponseList { get; set; } = new();
        public List<FilterFlowerDto> FlowerResponseList { get; set; } = new();
        public FilterPriceRangeDto? PriceRange { get; set; }
    }

    public class FilterSizeDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class FilterEventDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class FilterRecipientDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class FilterFlowerDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Color { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
    }

    public class FilterPriceRangeDto
    {
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
    }
}
