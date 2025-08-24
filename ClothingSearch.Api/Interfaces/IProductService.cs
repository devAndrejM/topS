using ClothingSearch.Api.DTOs;

namespace ClothingSearch.Api.Interfaces
{
    public interface IProductService
    {
        Task<SearchResponseDto> SearchProductsAsync(SearchRequestDto request);
        Task<List<ProductDto>> GetProductsByStoreAsync(string storeName, string query, int countryId);
        List<string> GetSupportedStoresForCountry(int countryId);
    }
}
