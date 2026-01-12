import { useState } from "react";
import { SettingsContext } from "./SettingsContext";

const SettingsProvider = ({ children }) => {
  const [lang, setLang] = useState("UA"); // "UA" | "ENG"
  const [currency, setCurrency] = useState("UAH"); // "UAH" | "USD"

  const toggleLang = () => setLang((prev) => (prev === "UA" ? "ENG" : "UA"));
  const toggleCurrency = () =>
    setCurrency((prev) => (prev === "UAH" ? "USD" : "UAH"));

  const value = {
    lang,
    toggleLang,
    currency,
    toggleCurrency,
  };

  return (
    <SettingsContext.Provider value={value}>
      {children}
    </SettingsContext.Provider>
  );
};

export default SettingsProvider;
