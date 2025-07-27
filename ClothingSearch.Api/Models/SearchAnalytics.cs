using System.ComponentModel.DataAnnotations;

namespace ClothingSearch.Api.Models
{
    public class SearchAnalytics
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Query { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;
        
        public int ResultCount { get; set; }
        
        [MaxLength(50)]
        public string UserId { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
