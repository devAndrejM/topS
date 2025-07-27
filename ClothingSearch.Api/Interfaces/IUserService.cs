using ClothingSearch.Api.DTOs;

namespace ClothingSearch.Api.Interfaces
{
    public interface IUserService
    {
        Task<UserSettingDto?> GetUserSettingsAsync(string userId);
        Task<UserSettingDto> CreateOrUpdateUserSettingsAsync(string userId, UpdateUserSettingDto dto);
    }
}
