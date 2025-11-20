using Microsoft.AspNetCore.Mvc;
using UsersService.BLL;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;

namespace UsersService.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Реєстрація нового клієнта (Sign Up Page)
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegistrationDto model)
        {
            var result = await _authService.RegisterAsync(model);

            if (result == null)
            {
                return BadRequest(new { Message = "Реєстрація не вдалася. Користувач вже існує або пароль некоректний." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Логін користувача (Sign In Page)
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);

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
            var result = await _authService.RefreshTokenAsync(model.RefreshToken);

            if (result == null)
            {
                return Unauthorized(new { Message = "Неправильний або прострочений refresh token." });
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto model)
        {
            await _authService.LogoutAsync(model.RefreshToken);
            return NoContent();
        }
    }
}