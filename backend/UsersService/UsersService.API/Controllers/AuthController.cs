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

            SetTokensInCookies(result.AccessToken, result.RefreshToken);

            return Ok(result);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] RefreshTokenRequestDto? model = null)
        {
            // Allow refresh even if body is empty, if we use cookies
            var token = model?.RefreshToken ?? Request.Cookies["refreshToken"];
            
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = "Refresh token missing." });
            }

            var result = await authService.RefreshTokenAsync(token);

            if (result == null)
            {
                return Unauthorized(new { Message = "Неправильний або прострочений refresh token." });
            }

            SetTokensInCookies(result.AccessToken, result.RefreshToken);

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto? model = null)
        {
            try
            {
                var token = model?.RefreshToken ?? Request.Cookies["refreshToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    await authService.LogoutAsync(token);
                }
            }
            catch
            {
                // Ignore revocation errors on logout
            }
            finally
            {
                ClearTokensCookies();
            }
            
            return Ok(new { Message = "Logged out successfully" });
        }

        private void SetTokensInCookies(string accessToken, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Must be true for SameSite=None
                SameSite = SameSiteMode.None, // Required for cross-site (http -> https)
                Path = "/", // Access from any path
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("accessToken", accessToken, cookieOptions);
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private void ClearTokensCookies()
        {
            var options = new CookieOptions 
            { 
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            Response.Cookies.Append("accessToken", "", options);
            Response.Cookies.Append("refreshToken", "", options);
            
            Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Append("Pragma", "no-cache");
            Response.Headers.Append("Expires", "0");
        }
    }
}