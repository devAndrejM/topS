namespace ClothingSearch.Api.DTOs
{
    public class UserSettingDto
    {
        public string UserId { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string ClothingSize { get; set; } = "M";
        public string ShoeSize { get; set; } = "42";
        public string ShoeSizeSystem { get; set; } = "EU";
    }
    
    public class UpdateUserSettingDto
    {
        public int CountryId { get; set; }
        public string ClothingSize { get; set; } = "M";
        public string ShoeSize { get; set; } = "42";
        public string ShoeSizeSystem { get; set; } = "EU";
    }
}
