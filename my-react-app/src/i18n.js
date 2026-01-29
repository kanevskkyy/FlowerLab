import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";

import translationEN from "./locales/en.json";
import translationUA from "./locales/ua.json";

const resources = {
  en: {
    translation: translationEN,
  },
  ua: {
    translation: translationUA,
  },
  uk: {
    translation: translationUA,
  },
};

const getNormalizedLanguage = (lang) => {
  if (!lang) return "ua";
  const normalized = lang.toLowerCase();
  return normalized === "ua" || normalized.startsWith("uk") ? "ua" : "en";
};

const savedLanguage = localStorage.getItem("appLanguage");
const initialLanguage = getNormalizedLanguage(savedLanguage);

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    lng: initialLanguage,
    fallbackLng: ["ua", "uk", "en"],
    interpolation: {
      escapeValue: false,
    },
    detection: {
      order: ["localStorage", "navigator"],
      lookupLocalStorage: "appLanguage",
      caches: ["localStorage"],
    },
  });

export default i18n;
