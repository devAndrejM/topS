using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Interfaces;
using ClothingSearch.Api.Models;
using System.Diagnostics;

namespace ClothingSearch.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IEnumerable<IStoreProvider> _storeProviders;
        private readonly ILogger<ProductService> _logger;
        
        public ProductService(IEnumerable<IStoreProvider> storeProviders, ILogger<ProductService> logger)
        {
            _storeProviders = storeProviders;
            _logger = logger;
        }
        
        public async Task<SearchResponseDto> SearchProductsAsync(SearchRequestDto request)
        {
            var stopwatch = Stopwatch.StartNew();
            var allProducts = new List<ProductDto>();
            var storesSearched = new List<string>();
            
            var userSettings = new UserSetting
            {
                CountryId = request.CountryId,
                ShowOnlyInStock = request.InStockOnly
            };
            
            var supportedProviders = _storeProviders.Where(p => p.SupportsCountry(request.CountryId));
            
            var searchTasks = supportedProviders.Select(async provider =>
            {
                try
                {
                    var results = await provider.SearchAsync(request.Query, userSettings, request.Category);
                    return new { ProviderName = provider.GetType().Name.Replace("Provider", ""), Results = results };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error searching provider");
                    return new { ProviderName = provider.GetType().Name.Replace("Provider", ""), Results = new List<ProductDto>() };
                }
            });
            
            var searchResults = await Task.WhenAll(searchTasks);
            
            foreach (var result in searchResults)
            {
                storesSearched.Add(result.ProviderName);
                allProducts.AddRange(result.Results);
            }
            
            stopwatch.Stop();
            
            return new SearchResponseDto
            {
                Products = allProducts,
                TotalResults = allProducts.Count,
                Query = request.Query,
                StoresSearched = storesSearched,
                SearchDuration = stopwatch.Elapsed
            };
        }
        
        public async Task<List<ProductDto>> GetProductsByStoreAsync(string storeName, string query, int countryId)
        {
            var provider = _storeProviders.FirstOrDefault(p => 
                p.GetType().Name.Replace("Provider", "").Equals(storeName, StringComparison.OrdinalIgnoreCase) && 
                p.SupportsCountry(countryId));
                
            if (provider == null) return new List<ProductDto>();
            
            var userSettings = new UserSetting { CountryId = countryId };
            return await provider.SearchAsync(query, userSettings);
        }
        
        public List<string> GetSupportedStoresForCountry(int countryId)
        {
            return _storeProviders
                .Where(p => p.SupportsCountry(countryId))
                .Select(p => p.GetType().Name.Replace("Provider", ""))
                .ToList();
        }
    }
}
