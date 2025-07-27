using Microsoft.AspNetCore.Mvc;
using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Interfaces;

namespace ClothingSearch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ISearchService searchService, ILogger<SearchController> logger)
        {
            _searchService = searchService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<SearchResultDto>> Search(
            [FromQuery] string query,
            [FromQuery] string userId = "anonymous",
            [FromQuery] string category = "")
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            try
            {
                _logger.LogInformation("🔍 Search request: query={Query}, userId={UserId}, category={Category}",
                    query, userId, category);

                // Check if SearchService is properly injected
                if (_searchService == null)
                {
                    _logger.LogError("❌ SearchService is NULL - dependency injection failed");
                    return StatusCode(500, "SearchService not configured");
                }

                _logger.LogInformation("✅ SearchService found, calling SearchAsync...");
                var result = await _searchService.SearchAsync(query, userId, category);

                _logger.LogInformation("✅ Search completed successfully - {ProductCount} products found",
                    result?.Products?.Count ?? 0);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SEARCH ERROR: Type={ExceptionType}, Message={ErrorMessage}, StackTrace={StackTrace}",
                    ex.GetType().Name, ex.Message, ex.StackTrace);

                // Return detailed error in development
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    return StatusCode(500, new
                    {
                        error = "Search failed",
                        type = ex.GetType().Name,
                        message = ex.Message,
                        innerException = ex.InnerException?.Message,
                        stackTrace = ex.StackTrace?.Split('\n').Take(5).ToArray()
                    });
                }

                return StatusCode(500, "An error occurred while searching");
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<CategorySummary>>> GetCategories(
            [FromQuery] string query,
            [FromQuery] string userId = "anonymous")
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            try
            {
                _logger.LogInformation("🔍 Categories request: query={Query}, userId={UserId}", query, userId);

                if (_searchService == null)
                {
                    _logger.LogError("❌ SearchService is NULL for categories");
                    return StatusCode(500, "SearchService not configured");
                }

                var categories = await _searchService.GetCategoriesAsync(query, userId);

                _logger.LogInformation("✅ Categories found: {CategoryCount}", categories?.Count ?? 0);

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CATEGORIES ERROR: Type={ExceptionType}, Message={ErrorMessage}",
                    ex.GetType().Name, ex.Message);

                // Return detailed error in development
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    return StatusCode(500, new
                    {
                        error = "Categories failed",
                        type = ex.GetType().Name,
                        message = ex.Message,
                        stackTrace = ex.StackTrace?.Split('\n').Take(5).ToArray()
                    });
                }

                return StatusCode(500, "An error occurred while getting categories");
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            try
            {
                var status = new
                {
                    controller = "SearchController",
                    searchService = _searchService != null ? "✅ Available" : "❌ NULL",
                    serviceType = _searchService?.GetType().Name ?? "NULL",
                    timestamp = DateTime.UtcNow
                };

                _logger.LogInformation("🧪 Search test endpoint called - SearchService: {SearchServiceStatus}",
                    _searchService != null ? "Available" : "NULL");

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Test endpoint failed");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}