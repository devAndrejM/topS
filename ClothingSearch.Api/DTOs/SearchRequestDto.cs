namespace ClothingSearch.Api.DTOs
{
    public class SearchRequestDto
    {
        public string Query { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int CountryId { get; set; } = 1;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<string>? Brands { get; set; }
        public List<string>? Sizes { get; set; }
        public bool InStockOnly { get; set; } = false;
    }
}
