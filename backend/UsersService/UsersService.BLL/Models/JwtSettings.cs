namespace UsersService.BLL.Models
{
    public class JwtSettings
    {
        public string Secret { get; set; } // Секретний ключ для підпису (має бути довгим)
        public string Issuer { get; set; } // Видавець токена (наприклад, FlowerLab.UsersService)
        public string Audience { get; set; } // Для кого призначений токен (наприклад, FlowerLab.WebAPI)
        public int AccessTokenExpirationMinutes { get; set; } // Термін дії Access Token (наприклад, 15 хв)
        public int RefreshTokenExpirationDays { get; set; } // Термін дії Refresh Token (наприклад, 7 днів)
    }
}