namespace AggregatorService.DTOs
{
    public class BouquetListDto
    {
        public List<BouquetSimpleDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }

    public class BouquetSimpleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public string MainPhotoUrl { get; set; } = null!;
        public List<BouquetSizeDto> Sizes { get; set; } = new();
    }

    public class BouquetSizeDto
    {
        public Guid SizeId { get; set; }
        public string SizeName { get; set; } = null!;
        public double Price { get; set; }
        public int MaxAssemblableCount { get; set; }
        public bool IsAvailable { get; set; }
        public List<FlowerInBouquetDto> Flowers { get; set; } = new();
    }

    public class FlowerInBouquetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
