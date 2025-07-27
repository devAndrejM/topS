using System.ComponentModel.DataAnnotations;

namespace ClothingSearch.Api.Models
{
    public class Country
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = string.Empty;
        
        public DateTime Created { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
        public virtual ICollection<UserSetting> UserSettings { get; set; } = new List<UserSetting>();
    }
}
