
export const getLocalizedValue = (field, currentLang = "ua") => {
  if (!field) return null;

  const lang =
    currentLang.toLowerCase() === "ua" ||
    currentLang.toLowerCase().startsWith("uk")
      ? "ua"
      : "en";

  if (typeof field === "object" && field !== null) {
    const normalizedField = {};
    Object.keys(field).forEach((k) => {
      normalizedField[k.toLowerCase()] = field[k];
    });

    if (lang === "ua") {
      return (
        normalizedField.ua ||
        normalizedField.uk ||
        normalizedField.ukr ||
        normalizedField.en ||
        null
      );
    }
    return (
      normalizedField.en ||
      normalizedField.eng ||
      normalizedField.ua ||
      normalizedField.uk ||
      null
    );
  }

  return field;
};

export const getStatusKey = (name) => {
  if (!name) return "";
  return name.replace(/\s/g, "").toLowerCase();
};

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
