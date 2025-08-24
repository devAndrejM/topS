using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Interfaces;
using ClothingSearch.Api.Models;

namespace ClothingSearch.Api.Providers
{
    public class HervisProvider : IStoreProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HervisProvider> _logger;
        
        public string ProviderType => "scraping";
        
        public HervisProvider(HttpClient httpClient, ILogger<HervisProvider> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public bool SupportsCountry(int countryId)
        {
            return countryId == 1;
        }
        
        public async Task<List<ProductDto>> SearchAsync(string query, UserSetting userSettings, string category = "")
        {
            await Task.Delay(1000);
            
            var imageUrl1 = "https://images.unsplash.com/photo-1549298916-b41d501d3772?w=300";
            var imageUrl2 = "https://images.unsplash.com/photo-1506629905607-d405d7a94c9a?w=300";
            
            return new List<ProductDto>
            {
                new ProductDto
                {
                    Name = $"Adidas Superstar - {query}",
                    Brand = "Adidas",
                    Price = 599.00m,
                    Currency = "HRK",
                    ImageUrl = imageUrl1,
                    ProductUrl = "https://hervis.hr/adidas-superstar",
                    AffiliateUrl = "https://hervis.hr/adidas-superstar?ref=clothingsearch",
                    StoreName = "Hervis",
                    Category = "Shoes",
                    Sizes = new List<string> { "39", "40", "41", "42", "43", "44" },
                    InStock = true,
                    Description = "Classic Adidas Superstar sneakers"
                },
                new ProductDto
                {
                    Name = $"Puma Training Shorts - {query}",
                    Brand = "Puma",
                    Price = 199.00m,
                    Currency = "HRK",
                    ImageUrl = imageUrl2,
                    ProductUrl = "https://hervis.hr/puma-shorts",
                    AffiliateUrl = "https://hervis.hr/puma-shorts?ref=clothingsearch",
                    StoreName = "Hervis",
                    Category = "Clothing",
                    Sizes = new List<string> { "S", "M", "L", "XL" },
                    InStock = true,
                    Description = "Comfortable training shorts from Puma"
                }
            };
        }
    }
}
