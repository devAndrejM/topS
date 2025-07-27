using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClothingSearch.Api.Data;
using ClothingSearch.Api.DTOs;
using ClothingSearch.Api.Interfaces;
using ClothingSearch.Api.Models;

namespace ClothingSearch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ClothingSearchContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService, 
            ClothingSearchContext context,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
        }

        [HttpGet("{userId}/settings")]
        public async Task<ActionResult<UserSettingDto>> GetUserSettings(string userId)
        {
            try
            {
                var settings = await _userService.GetUserSettingsAsync(userId);
                if (settings == null)
                {
                    return NotFound("User settings not found");
                }
                
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user settings for user: {UserId}", userId);
                return StatusCode(500, "An error occurred while getting user settings");
            }
        }

        [HttpPost("{userId}/settings")]
        public async Task<ActionResult<UserSettingDto>> CreateOrUpdateUserSettings(
            string userId, 
            [FromBody] UpdateUserSettingDto dto)
        {
            try
            {
                var settings = await _userService.CreateOrUpdateUserSettingsAsync(userId, dto);
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user settings for user: {UserId}", userId);
                return StatusCode(500, "An error occurred while updating user settings");
            }
        }

        [HttpGet("countries")]
        public async Task<ActionResult<List<Country>>> GetCountries()
        {
            try
            {
                var countries = await _context.Countries
                    .OrderBy(c => c.Name)
                    .ToListAsync();
                
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting countries");
                return StatusCode(500, "An error occurred while getting countries");
            }
        }
    }
}
