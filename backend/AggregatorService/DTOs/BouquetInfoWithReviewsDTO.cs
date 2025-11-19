namespace AggregatorService.DTOs
{
    public class BouquetInfoWithReviewsDTO
    {
        public BouquetResponse Bouquet { get; set; }
        public ReviewsListGrpcResponse Reviews { get; set; }
    }
}
