using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsersService.Domain.Entities;
using System.Security.Claims;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;

[Authorize] // Всі методи вимагають дійсного Access Token!
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAddressService _addressService;
    
    public UsersController(UserManager<ApplicationUser> userManager, IAddressService addressService)
    {
        _userManager = userManager;
        _addressService = addressService;
    }   

    /// <summary>
    /// Отримати дані поточного користувача (Personal Information)
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        // Отримуємо ID користувача з JWT Claim
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            return NotFound();
        }

        // TODO: Використовувати DTO для повернення лише необхідних даних
        return Ok(new 
        {
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber
        });
    }

    /// <summary>
    /// Оновити персональні дані (Personal Information)
    /// </summary>
    [HttpPut("me")]
    // TODO: Створити DTO для запиту оновлення
    public async Task<IActionResult> UpdateCurrentUser([FromBody] object updateModel) 
    {
        // Логіка оновлення даних користувача
        return Ok(new { Message = "User data updated successfully." });
    }

    /// <summary>
    /// Ендпоїнт для видалення облікового запису
    /// </summary>
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) return NotFound();

        // TODO: Додаткова логіка (наприклад, анулювання всіх токенів)

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Account deletion failed." });
        }
        return NoContent(); // 204 No Content
    }
    
    [HttpGet("me/addresses")]
    public async Task<IActionResult> GetSavedAddresses()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
        return Ok(addresses);
    }

    /// <summary>
    /// Створити нову адресу доставки
    /// </summary>
    [HttpPost("me/addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] AddressDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var newAddress = await _addressService.CreateAddressAsync(userId, model);
        return CreatedAtAction(nameof(GetSavedAddresses), newAddress);
    }
    [HttpPut("me/addresses/{id}")]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _addressService.UpdateAddressAsync(userId, id, model);
    
        if (!result)
        {
            // 404, якщо адреса не існує або не належить цьому користувачеві
            return NotFound(new { Message = "Address not found or access denied." });
        }
    
        return NoContent(); // 204 No Content - стандартна відповідь для успішного PUT
    }

    /// <summary>
    /// Видалити адресу
    /// </summary>
    [HttpDelete("me/addresses/{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _addressService.DeleteAddressAsync(userId, id);
    
        if (!result)
        {
            return NotFound(new { Message = "Address not found or access denied." });
        }
    
        return NoContent(); // 204 No Content
    }
}