namespace ClothingSearch.Api.DTOs
{
    public class SearchResponseDto
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int TotalResults { get; set; }
        public string Query { get; set; } = string.Empty;
        public List<string> StoresSearched { get; set; } = new List<string>();
        public TimeSpan SearchDuration { get; set; }
    }
}
