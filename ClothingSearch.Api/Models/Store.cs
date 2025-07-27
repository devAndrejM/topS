using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ClothingSearch.Api.Models
{
    public class Store
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public int CountryId { get; set; }
        public virtual Country Country { get; set; } = null!;
        
        [Required]
        [MaxLength(50)]
        public string ProviderType { get; set; } = string.Empty;
        
        public string Config { get; set; } = "{}";
        
        public bool IsActive { get; set; } = true;
        
        public DateTime Created { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<SearchCache> SearchCaches { get; set; } = new List<SearchCache>();
        
        public T GetConfig<T>() where T : new()
        {
            try
            {
                return JsonSerializer.Deserialize<T>(Config) ?? new T();
            }
            catch
            {
                return new T();
            }
        }
        
        public void SetConfig<T>(T config)
        {
            Config = JsonSerializer.Serialize(config);
        }
    }
}
