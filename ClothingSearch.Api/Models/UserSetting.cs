using System.ComponentModel.DataAnnotations;

namespace ClothingSearch.Api.Models
{
    public class UserSetting
    {
        [Key]
        [MaxLength(50)]
        public string UserId { get; set; } = string.Empty;
        
        public int CountryId { get; set; }
        public virtual Country Country { get; set; } = null!;
        
        [MaxLength(10)]
        public string ClothingSize { get; set; } = "M";
        
        [MaxLength(10)]
        public string ShoeSize { get; set; } = "42";
        
        [MaxLength(5)]
        public string ShoeSizeSystem { get; set; } = "EU";
        
        public DateTime Updated { get; set; } = DateTime.UtcNow;
    }
}
