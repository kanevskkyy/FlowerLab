import { useState, useEffect } from "react";
import { SettingsContext } from "./SettingsContext";
import i18n from "../i18n";

const SettingsProvider = ({ children }) => {
  const [lang, setLang] = useState(() => {
    // If no language is saved, default to "UA" for first-time visitors
    return localStorage.getItem("appLanguage") || "UA";
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

  const toggleLang = () => setLang((prev) => (prev === "UA" ? "ENG" : "UA"));
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
