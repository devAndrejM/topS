namespace ClothingSearch.Api.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ProductUrl { get; set; } = string.Empty;
        public string AffiliateUrl { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Sizes { get; set; } = new List<string>();
        public bool InStock { get; set; } = true;
        public string Description { get; set; } = string.Empty;
    }
}
