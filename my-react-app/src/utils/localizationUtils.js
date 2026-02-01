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

/**
 * Normalizes a status name into a translation key.
 */
export const getStatusKey = (name) => {
  if (!name) return "";
  return name.replace(/\s/g, "").toLowerCase();
};

/**
 * Localizes a status object or name using translations or fallback to i18next keys.
 */
export const getLocalizedStatus = (statusObj, currentLang, t) => {
  if (!statusObj) return "";
  const name = typeof statusObj === "string" ? statusObj : statusObj.name;
  const translations = statusObj.translations;

  const localized = getLocalizedValue(translations, currentLang);
  if (localized) return localized;

  return t(`order_status.${getStatusKey(name)}`, {
    defaultValue: name,
  });
};
