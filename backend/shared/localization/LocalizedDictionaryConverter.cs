using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace shared.localization
{
    public class LocalizedDictionaryConverter : JsonConverter<Dictionary<string, string>>
    {
        private readonly ILanguageProvider _languageProvider = new LanguageProvider();

        public LocalizedDictionaryConverter()
        {
        }

        public override Dictionary<string, string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Use clean options to avoid recursion if this converter is registered globally
            var cleanOptions = new JsonSerializerOptions(options);
            cleanOptions.Converters.Remove(this); // Attempt to remove self if present, or just use new options if simpler
            
            // safer approach: just use default options, BUT we need to copy settings if needed. 
            // For simple dictionary, generic Deserialize should work.
            // Using a new instance of options usually breaks converter chain.
            return JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, new JsonSerializerOptions { PropertyNamingPolicy = null });
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
        {
            if (!string.IsNullOrEmpty(_languageProvider.CurrentLanguage) && value.TryGetValue(_languageProvider.CurrentLanguage, out var localizedValue))
            {
                writer.WriteStringValue(localizedValue);
            }
            else if (!string.IsNullOrEmpty(_languageProvider.CurrentLanguage) && value.Count > 0)
            {
                // Fallback to first available language if requested one is missing
                writer.WriteStringValue(value.Values.FirstOrDefault() ?? "");
            }
            else
            {
                // No language set or Admin request - write full dictionary MANUALLY to avoid recursion
                // Do NOT use JsonSerializer.Serialize(writer, value) here as it might trigger this converter again
                writer.WriteStartObject();
                foreach (var kvp in value)
                {
                    writer.WritePropertyName(kvp.Key);
                    writer.WriteStringValue(kvp.Value);
                }
                writer.WriteEndObject();
            }
        }
    }
}
