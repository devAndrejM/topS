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
