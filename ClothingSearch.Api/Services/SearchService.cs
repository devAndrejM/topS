using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ClothingSearch.Api.Data;
using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Interfaces;
using ClothingSearch.Api.Models;

namespace ClothingSearch.Api.Services
{
    public class SearchService : ISearchService
    {
        private readonly ClothingSearchContext _context;
        private readonly IEnumerable<IStoreProvider> _storeProviders;
        private readonly ILogger<SearchService> _logger;

        public SearchService(
            ClothingSearchContext context,
            IEnumerable<IStoreProvider> storeProviders,
            ILogger<SearchService> logger)
        {
            _context = context;
            _storeProviders = storeProviders;
            _logger = logger;
        }

        public async Task<SearchResultDto> SearchAsync(string query, string userId, string category = "")
        {
            _logger.LogInformation("🔍 SearchService.SearchAsync called: query={Query}, userId={UserId}, category={Category}",
                query, userId, category);

            var userSettings = await GetUserSettingsAsync(userId);
            if (userSettings == null)
            {
                _logger.LogInformation("👤 Creating default user settings for userId={UserId}", userId);
                userSettings = await CreateDefaultUserSettingsAsync(userId);
            }

            _logger.LogInformation("👤 User settings: CountryId={CountryId}, Country={CountryName}",
                userSettings.CountryId, userSettings.Country?.Name ?? "Unknown");

            var cachedResults = await GetCachedResultsAsync(query, category, userSettings.CountryId);
            if (cachedResults != null)
            {
                _logger.LogInformation("✅ Returning cached results for query: {Query}", query);
                return cachedResults;
            }

            _logger.LogInformation("🔄 No cache found, performing fresh search...");

            var allProducts = new List<ProductDto>();
            var availableStores = await _context.Stores
                .Where(s => s.IsActive && s.CountryId == userSettings.CountryId)
                .ToListAsync();

            _logger.LogInformation("🏪 Found {StoreCount} active stores for country {CountryId}",
                availableStores.Count, userSettings.CountryId);

            var searchTasks = new List<Task<List<ProductDto>>>();

            foreach (var store in availableStores)
            {
                var provider = _storeProviders.FirstOrDefault(p =>
                    p.ProviderType == store.ProviderType &&
                    p.SupportsCountry(userSettings.CountryId));

                if (provider != null)
                {
                    _logger.LogInformation("🔍 Adding search task for provider: {ProviderType}", store.ProviderType);
                    searchTasks.Add(provider.SearchAsync(query, userSettings, category));
                }
                else
                {
                    _logger.LogWarning("⚠️ No provider found for store: {StoreName}, ProviderType: {ProviderType}",
                        store.Name, store.ProviderType);
                }
            }

            _logger.LogInformation("🚀 Starting {TaskCount} parallel search tasks...", searchTasks.Count);

            try
            {
                var results = await Task.WhenAll(searchTasks);
                allProducts = results.SelectMany(r => r).ToList();

                _logger.LogInformation("✅ Search completed: {ProductCount} total products found", allProducts.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during parallel store search for query: {Query}", query);
                // Continue with empty results rather than crashing
            }

            var searchResult = new SearchResultDto
            {
                Query = query,
                Products = allProducts,
                TotalResults = allProducts.Count,
                LastUpdated = DateTime.UtcNow,
                IsFromCache = false,
                SelectedCategory = category
            };

            if (string.IsNullOrEmpty(category))
            {
                searchResult.Categories = GenerateCategories(allProducts);
                _logger.LogInformation("📊 Generated {CategoryCount} categories", searchResult.Categories.Count);
            }

            await CacheResultsAsync(query, category, userSettings.CountryId, searchResult);
            await LogSearchAnalyticsAsync(query, category, userId, allProducts.Count);

            _logger.LogInformation("✅ Search completed successfully for query: {Query}", query);
            return searchResult;
        }

        public async Task<List<CategorySummary>> GetCategoriesAsync(string query, string userId)
        {
            _logger.LogInformation("🔍 GetCategoriesAsync called: query={Query}, userId={UserId}", query, userId);

            var result = await SearchAsync(query, userId);

            _logger.LogInformation("✅ Categories retrieved: {CategoryCount}", result.Categories.Count);
            return result.Categories;
        }

        private async Task<UserSetting?> GetUserSettingsAsync(string userId)
        {
            try
            {
                return await _context.UserSettings
                    .Include(u => u.Country)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting user settings for userId={UserId}", userId);
                return null;
            }
        }

        private async Task<UserSetting> CreateDefaultUserSettingsAsync(string userId)
        {
            try
            {
                var defaultCountry = await _context.Countries.FirstOrDefaultAsync(c => c.Name == "Croatia");
                if (defaultCountry == null)
                {
                    // If Croatia doesn't exist, get the first available country
                    defaultCountry = await _context.Countries.FirstAsync();
                    _logger.LogWarning("⚠️ Croatia not found, using {CountryName} as default", defaultCountry.Name);
                }

                var userSetting = new UserSetting
                {
                    UserId = userId,
                    CountryId = defaultCountry.Id,
                    ClothingSize = "M",
                    ShoeSize = "42",
                    ShoeSizeSystem = "EU"
                };

                _context.UserSettings.Add(userSetting);
                await _context.SaveChangesAsync();

                userSetting.Country = defaultCountry;

                _logger.LogInformation("✅ Created default user settings for userId={UserId}, country={CountryName}",
                    userId, defaultCountry.Name);

                return userSetting;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creating default user settings for userId={UserId}", userId);
                throw;
            }
        }

        private async Task<SearchResultDto?> GetCachedResultsAsync(string query, string category, int countryId)
        {
            try
            {
                // FIXED: Replace IsExpired computed property with direct DateTime comparison
                var cacheExpiry = DateTime.UtcNow.AddHours(-6); // 6 hours cache

                var cached = await _context.SearchCaches
                    .Include(c => c.Store)
                    .Where(c => c.SearchQuery == query &&
                               c.Category == category &&
                               c.Store.CountryId == countryId &&
                               c.ExpiresAt > DateTime.UtcNow) // FIXED: Use ExpiresAt field instead of computed IsExpired
                    .FirstOrDefaultAsync();

                if (cached != null)
                {
                    try
                    {
                        var products = JsonSerializer.Deserialize<List<ProductDto>>(cached.Results) ?? new List<ProductDto>();

                        _logger.LogInformation("✅ Cache hit: {ProductCount} products from cache", products.Count);

                        return new SearchResultDto
                        {
                            Query = query,
                            Products = products,
                            TotalResults = products.Count,
                            LastUpdated = cached.CreatedAt,
                            IsFromCache = true,
                            SelectedCategory = category,
                            Categories = string.IsNullOrEmpty(category) ? GenerateCategories(products) : new List<CategorySummary>()
                        };
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "❌ Failed to deserialize cached results for query: {Query}", query);

                        // Remove corrupted cache entry
                        _context.SearchCaches.Remove(cached);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error checking cache for query: {Query}", query);
                // Continue without cache
            }

            return null;
        }

        private async Task CacheResultsAsync(string query, string category, int countryId, SearchResultDto result)
        {
            try
            {
                var store = await _context.Stores.FirstOrDefaultAsync(s => s.CountryId == countryId);
                if (store == null)
                {
                    _logger.LogWarning("⚠️ No store found for countryId={CountryId}, skipping cache", countryId);
                    return;
                }

                var cache = new SearchCache
                {
                    SearchQuery = query,
                    Category = category,
                    StoreId = store.Id,
                    Results = JsonSerializer.Serialize(result.Products),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(6)
                };

                _context.SearchCaches.Add(cache);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Results cached for query: {Query}", query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to cache results for query: {Query}", query);
                // Continue without caching
            }
        }

        private List<CategorySummary> GenerateCategories(List<ProductDto> products)
        {
            try
            {
                return products
                    .Where(p => !string.IsNullOrEmpty(p.Category))
                    .GroupBy(p => p.Category)
                    .Select(g => new CategorySummary
                    {
                        Name = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(c => c.Count)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error generating categories");
                return new List<CategorySummary>();
            }
        }

        private async Task LogSearchAnalyticsAsync(string query, string category, string userId, int resultCount)
        {
            try
            {
                var analytics = new SearchAnalytics
                {
                    Query = query,
                    Category = category,
                    UserId = userId,
                    ResultCount = resultCount,
                    Timestamp = DateTime.UtcNow
                };

                _context.SearchAnalytics.Add(analytics);
                await _context.SaveChangesAsync();

                _logger.LogInformation("📊 Analytics logged for query: {Query}", query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to log search analytics for query: {Query}", query);
                // Continue without analytics
            }
        }
    }
}