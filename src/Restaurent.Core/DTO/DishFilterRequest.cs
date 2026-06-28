namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Represents filter options for retrieving dishes
    /// </summary>
    public class DishFilterRequest
    {
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public decimal? MinRating { get; set; }
    }
}
