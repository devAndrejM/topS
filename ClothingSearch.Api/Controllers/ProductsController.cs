using Microsoft.AspNetCore.Mvc;
using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Interfaces;

namespace ClothingSearch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;
        
        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        
        [HttpPost("search")]
        public async Task<ActionResult<SearchResponseDto>> Search([FromBody] SearchRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return BadRequest("Search query is required");
            
            try
            {
                var result = await _productService.SearchProductsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching");
                return StatusCode(500, "An error occurred while searching");
            }
        }
        
        [HttpGet("search")]
        public async Task<ActionResult<SearchResponseDto>> SearchGet(
            [FromQuery] string query,
            [FromQuery] string category = "",
            [FromQuery] int countryId = 1,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool inStockOnly = false)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query is required");
            
            var request = new SearchRequestDto
            {
                Query = query,
                Category = category,
                CountryId = countryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                InStockOnly = inStockOnly
            };
            
            try
            {
                var result = await _productService.SearchProductsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching");
                return StatusCode(500, "An error occurred while searching");
            }
        }
        
        [HttpGet("stores")]
        public ActionResult<List<string>> GetSupportedStores([FromQuery] int countryId = 1)
        {
            try
            {
                var stores = _productService.GetSupportedStoresForCountry(countryId);
                return Ok(stores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stores");
                return StatusCode(500, "An error occurred");
            }
        }
        
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
        }
    }
}
