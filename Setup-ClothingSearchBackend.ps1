# Setup-ClothingSearchBackend.ps1
# Minimal script to create ClothingSearch.Api backend files

param(
    [string]$ProjectPath = ".\ClothingSearch.Api",
    [switch]$Force
)

if (Test-Path $ProjectPath) {
    if ($Force) {
        Remove-Item $ProjectPath -Recurse -Force
    } else {
        Write-Host "Directory exists. Use -Force to overwrite."
        exit 1
    }
}

New-Item -ItemType Directory -Path $ProjectPath -Force | Out-Null
Set-Location $ProjectPath

# Create directories
$directories = @("Controllers", "DTOs", "Models", "Interfaces", "Services", "Providers", "Properties")
foreach ($dir in $directories) {
    New-Item -ItemType Directory -Path $dir -Force | Out-Null
}

# .csproj
@'
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
  </ItemGroup>
</Project>
'@ | Set-Content "ClothingSearch.Api.csproj" -Encoding UTF8

# ProductDto
@'
namespace ClothingSearch.Api.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ProductUrl { get; set; } = string.Empty;
        public string AffiliateUrl { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Sizes { get; set; } = new List<string>();
        public bool InStock { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
'@ | Set-Content "DTOs/ProductDto.cs" -Encoding UTF8

# SearchRequestDto
@'
namespace ClothingSearch.Api.DTOs
{
    public class SearchRequestDto
    {
        public string Query { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int CountryId { get; set; } = 1;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<string>? Brands { get; set; }
        public List<string>? Sizes { get; set; }
        public bool InStockOnly { get; set; } = false;
    }
}
'@ | Set-Content "DTOs/SearchRequestDto.cs" -Encoding UTF8

# SearchResponseDto
@'
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
'@ | Set-Content "DTOs/SearchResponseDto.cs" -Encoding UTF8

# UserSetting
@'
namespace ClothingSearch.Api.Models
{
    public class UserSetting
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public string PreferredCurrency { get; set; } = "HRK";
        public List<string> PreferredBrands { get; set; } = new List<string>();
        public List<string> PreferredSizes { get; set; } = new List<string>();
        public decimal? DefaultMinPrice { get; set; }
        public decimal? DefaultMaxPrice { get; set; }
        public bool ShowOnlyInStock { get; set; } = false;
    }
}
'@ | Set-Content "Models/UserSetting.cs" -Encoding UTF8

# IStoreProvider
@'
using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Models;

namespace ClothingSearch.Api.Interfaces
{
    public interface IStoreProvider
    {
        string ProviderType { get; }
        bool SupportsCountry(int countryId);
        Task<List<ProductDto>> SearchAsync(string query, UserSetting userSettings, string category = "");
    }
}
'@ | Set-Content "Interfaces/IStoreProvider.cs" -Encoding UTF8

# IProductService
@'
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
'@ | Set-Content "Interfaces/IProductService.cs" -Encoding UTF8

# ProductService
@'
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
'@ | Set-Content "Services/ProductService.cs" -Encoding UTF8

# HervisProvider
@'
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
'@ | Set-Content "Providers/HervisProvider.cs" -Encoding UTF8

# AmazonProvider
@'
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
'@ | Set-Content "Providers/AmazonProvider.cs" -Encoding UTF8

# ProductsController
@'
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
'@ | Set-Content "Controllers/ProductsController.cs" -Encoding UTF8

# Program.cs
@'
using ClothingSearch.Api.Interfaces;
using ClothingSearch.Api.Services;
using ClothingSearch.Api.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<HervisProvider>();
builder.Services.AddTransient<IStoreProvider, HervisProvider>();
builder.Services.AddTransient<IStoreProvider, AmazonProvider>();
builder.Services.AddTransient<IProductService, ProductService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8100", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => new { Message = "ClothingSearch API is running!" });

app.Run();
'@ | Set-Content "Program.cs" -Encoding UTF8

# appsettings.json
@'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
'@ | Set-Content "appsettings.json" -Encoding UTF8

# launchSettings.json
@'
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
'@ | Set-Content "Properties/launchSettings.json" -Encoding UTF8

Write-Host "Setup complete. Run: dotnet run"