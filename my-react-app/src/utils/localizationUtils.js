/**
 * Extracts a localized value from a field or dictionary.
 * @returns {string|null} The localized string, or null if not found (allowing caller to fallback).
 */
export const getLocalizedValue = (field, currentLang = "ua") => {
  if (!field) return null;

  // Normalize language code to lowercase
  const lang =
    currentLang.toLowerCase() === "ua" ||
    currentLang.toLowerCase().startsWith("uk")
      ? "ua"
      : "en";

  if (typeof field === "object" && field !== null) {
    const val = field[lang] || field.ua || field.en;
    if (val) return val;

    const normalizedField = {};
    Object.keys(field).forEach((k) => {
      normalizedField[k.toLowerCase()] = field[k];
    });
    return (
      normalizedField[lang] || normalizedField.ua || normalizedField.en || null
    );
  }

  return field;
};
