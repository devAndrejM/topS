using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Interfaces;
using ClothingSearch.Api.Models;

namespace ClothingSearch.Api.Providers
{
    public class AmazonProvider : IStoreProvider
    {
        private readonly ILogger<AmazonProvider> _logger;
        
        public string ProviderType => "affiliate";
        
        public AmazonProvider(ILogger<AmazonProvider> logger)
        {
            _logger = logger;
        }
        
        public bool SupportsCountry(int countryId)
        {
            return countryId == 3;
        }
        
        public async Task<List<ProductDto>> SearchAsync(string query, UserSetting userSettings, string category = "")
        {
            await Task.Delay(500);
            
            var imageUrl1 = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=300";
            var imageUrl2 = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=300";
            
            return new List<ProductDto>
            {
                new ProductDto
                {
                    Name = $"Nike Air Max 270 - {query}",
                    Brand = "Nike",
                    Price = 89.99m,
                    Currency = "USD",
                    ImageUrl = imageUrl1,
                    ProductUrl = "https://amazon.com/nike-air-max",
                    AffiliateUrl = "https://amazon.com/nike-air-max?tag=youraffid",
                    StoreName = "Amazon",
                    Category = "Shoes",
                    Sizes = new List<string> { "8", "9", "10", "11", "12" },
                    InStock = true,
                    Description = "Classic Nike Air Max 270 sneakers"
                },
                new ProductDto
                {
                    Name = $"Nike Dri-FIT T-Shirt - {query}",
                    Brand = "Nike",
                    Price = 24.99m,
                    Currency = "USD",
                    ImageUrl = imageUrl2,
                    ProductUrl = "https://amazon.com/nike-tshirt",
                    AffiliateUrl = "https://amazon.com/nike-tshirt?tag=youraffid",
                    StoreName = "Amazon",
                    Category = "Clothing",
                    Sizes = new List<string> { "S", "M", "L", "XL" },
                    InStock = true,
                    Description = "Moisture-wicking Nike Dri-FIT technology"
                }
            };
        }
    }
}
