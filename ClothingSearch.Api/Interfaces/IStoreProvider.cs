using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Models;

namespace ClothingSearch.Api.Interfaces
{
    public interface IStoreProvider
    {
        Task<List<ProductDto>> SearchAsync(string query, UserSetting userSettings, string category = "");
        string ProviderType { get; }
        bool SupportsCountry(int countryId);
    }
}
