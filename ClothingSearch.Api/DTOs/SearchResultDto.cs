namespace ClothingSearch.Api.DTOs
{
    public class SearchResultDto
    {
        public string Query { get; set; } = string.Empty;
        public List<CategorySummary> Categories { get; set; } = new List<CategorySummary>();
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public bool IsFromCache { get; set; }
        public DateTime LastUpdated { get; set; }
        public int TotalResults { get; set; }
        public string SelectedCategory { get; set; } = string.Empty;
    }
    
    public class CategorySummary
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
