using System.ComponentModel.DataAnnotations;

namespace ClothingSearch.Api.Models
{
    public class SearchCache
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string SearchQuery { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;
        
        public int StoreId { get; set; }
        public virtual Store Store { get; set; } = null!;
        
        public string Results { get; set; } = "[]";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpiresAt { get; set; }
        
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}
