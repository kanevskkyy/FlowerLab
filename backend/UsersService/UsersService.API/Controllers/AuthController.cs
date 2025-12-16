using Microsoft.AspNetCore.Mvc;
using UsersService.BLL.Models.Auth;
using UsersService.BLL.Models.Email;
using UsersService.BLL.Services.Interfaces;

namespace UsersService.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegistrationDto model)
        {
            await authService.RegisterAsync(model);

            return Ok(new
            {
                Message = "Користувача створено ✅. Перевірте email для підтвердження 📧"
            });

        }

        [HttpGet("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            try
            {
                await authService.ConfirmEmailAsync(userId, token);
                return Ok(new { Message = "Email підтверджено 🎉. Тепер можете увійти." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await authService.LoginAsync(model);

            if (result == null)
            {
                return Unauthorized(new { Message = "Неправильний email або пароль." });
            }

            return Ok(result);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto model)
        {
            var result = await authService.RefreshTokenAsync(model.RefreshToken);

            if (result == null)
            {
                return Unauthorized(new { Message = "Неправильний або прострочений refresh token." });
            }

            return Ok(result);

        }

        [HttpPost("resend-confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailDto model)
        {
            try
            {
                await authService.ResendConfirmationEmailAsync(model.Email);
                return Ok(new { Message = "Новий лист з підтвердженням надіслано 📧" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            try
            {
                await authService.SendResetPasswordLinkAsync(model.Email);
                return Ok(new { Message = "Лист для скидання пароля надіслано 📧" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            try
            {
                await authService.ResetPasswordAsync(model);
                return Ok(new { Message = "Пароль успішно змінено! 🎉" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto model)
        {
            await authService.LogoutAsync(model.RefreshToken);
            return NoContent();
        }
    }
}