using System.Collections.Generic;

namespace shared.localization
{
    public interface ILanguageProvider
    {
        string? CurrentLanguage { get; set; }
    }

    public class LanguageProvider : ILanguageProvider
    {
        private static readonly System.Threading.AsyncLocal<string?> _currentLanguage = new();

        public string? CurrentLanguage
        {
            get => _currentLanguage.Value;
            set => _currentLanguage.Value = value;
        }
    }
}
