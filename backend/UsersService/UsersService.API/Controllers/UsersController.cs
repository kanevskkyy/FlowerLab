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
    
    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
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
}