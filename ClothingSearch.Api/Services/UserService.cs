using Microsoft.EntityFrameworkCore;
using ClothingSearch.Api.Data;
using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Interfaces;
using ClothingSearch.Api.Models;

namespace ClothingSearch.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ClothingSearchContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ClothingSearchContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserSettingDto?> GetUserSettingsAsync(string userId)
        {
            var userSetting = await _context.UserSettings
                .Include(u => u.Country)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (userSetting == null)
                return null;

            return new UserSettingDto
            {
                UserId = userSetting.UserId,
                CountryId = userSetting.CountryId,
                CountryName = userSetting.Country.Name,
                ClothingSize = userSetting.ClothingSize,
                ShoeSize = userSetting.ShoeSize,
                ShoeSizeSystem = userSetting.ShoeSizeSystem
            };
        }

        public async Task<UserSettingDto> CreateOrUpdateUserSettingsAsync(string userId, UpdateUserSettingDto dto)
        {
            var existingSettings = await _context.UserSettings
                .Include(u => u.Country)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (existingSettings != null)
            {
                existingSettings.CountryId = dto.CountryId;
                existingSettings.ClothingSize = dto.ClothingSize;
                existingSettings.ShoeSize = dto.ShoeSize;
                existingSettings.ShoeSizeSystem = dto.ShoeSizeSystem;
                existingSettings.Updated = DateTime.UtcNow;
                
                _context.UserSettings.Update(existingSettings);
            }
            else
            {
                existingSettings = new UserSetting
                {
                    UserId = userId,
                    CountryId = dto.CountryId,
                    ClothingSize = dto.ClothingSize,
                    ShoeSize = dto.ShoeSize,
                    ShoeSizeSystem = dto.ShoeSizeSystem,
                    Updated = DateTime.UtcNow
                };
                
                _context.UserSettings.Add(existingSettings);
            }

            await _context.SaveChangesAsync();

            var country = await _context.Countries.FindAsync(dto.CountryId);
            
            return new UserSettingDto
            {
                UserId = userId,
                CountryId = dto.CountryId,
                CountryName = country?.Name ?? "",
                ClothingSize = dto.ClothingSize,
                ShoeSize = dto.ShoeSize,
                ShoeSizeSystem = dto.ShoeSizeSystem
            };
        }
    }
}
