using Microsoft.AspNetCore.Http;
using shared.localization;
using System.Threading.Tasks;

namespace shared.localization
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILanguageProvider languageProvider)
        {
            // Skip localization for admin endpoints to return full dictionary
            if (context.Request.Path.StartsWithSegments("/api/admin") || context.Request.Headers.ContainsKey("X-Skip-Localization"))
            {
                await _next(context);
                return;
            }

            var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                // Take first part, e.g. "en-US,en;q=0.9" -> "en"
                // Split by ',' to get first language option.
                // Split by ';' to strip quality factor (e.g. "en;q=0.9" -> "en").
                // Split by '-' to get language code (e.g. "en-US" -> "en").
                var lang = acceptLanguage.Split(',')[0].Split(';')[0].Split('-')[0].ToLower();
                
                // Only set if it's one of supported languages (fallback to ua if needed)
                if (lang == "uk" || lang == "ua" || lang == "ukr") lang = "ua";
                else if (lang == "en" || lang == "eng") lang = "en";
                else lang = null; // Ignore unsupported

                if (lang != null)
                {
                    languageProvider.CurrentLanguage = lang;
                }
            }

            // Ensure downstream caches (browsers, proxies) vary by language
            context.Response.Headers.Append("Vary", "Accept-Language");

            await _next(context);
        }
    }
}
