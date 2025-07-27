using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Models;

namespace ClothingSearch.Api.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResultDto> SearchAsync(string query, string userId, string category = "");
        Task<List<CategorySummary>> GetCategoriesAsync(string query, string userId);
    }
}
