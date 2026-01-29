import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";

import translationEN from "./locales/en.json";
import translationUA from "./locales/ua.json";

const resources = {
  ENG: {
    translation: translationEN,
  },
  UA: {
    translation: translationUA,
  },
};

const savedLanguage = localStorage.getItem("appLanguage") || "UA";

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    lng: savedLanguage,
    fallbackLng: "UA",
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
