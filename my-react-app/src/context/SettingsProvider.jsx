import { useState, useEffect } from "react";
import { SettingsContext } from "./SettingsContext";
import i18n from "../i18n";

const SettingsProvider = ({ children }) => {
  const [lang, setLang] = useState(() => {
    const saved = localStorage.getItem("appLanguage");
    if (!saved) return "ua";
    const normalized = saved.toLowerCase();
    return normalized === "ua" || normalized.startsWith("uk") ? "ua" : "en";
  });
  const [currency, setCurrency] = useState(() => {
    return localStorage.getItem("appCurrency") || "UAH";
  });

  useEffect(() => {
    i18n.changeLanguage(lang);
    localStorage.setItem("appLanguage", lang);
  }, [lang]);

  useEffect(() => {
    localStorage.setItem("appCurrency", currency);
  }, [currency]);

  const toggleLang = () => setLang((prev) => (prev === "ua" ? "en" : "ua"));
  const toggleCurrency = () =>
    setCurrency((prev) => (prev === "UAH" ? "USD" : "UAH"));

  const value = {
    lang,
    setLang,
    toggleLang,
    currency,
    setCurrency,
    toggleCurrency,
  };

  return (
    <SettingsContext.Provider value={value}>
      {children}
    </SettingsContext.Provider>
  );
};

export default SettingsProvider;
