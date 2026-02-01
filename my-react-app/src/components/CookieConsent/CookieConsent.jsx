import { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import "./CookieConsent.css";

const CookieConsent = () => {
  const { t } = useTranslation();
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    const consent = localStorage.getItem("cookieConsent");
    if (!consent) {
      setIsVisible(true);
    }
  }, []);

  const handleAccept = () => {
    localStorage.setItem("cookieConsent", "accepted");
    setIsVisible(false);
  };

  const handleDecline = () => {
    localStorage.setItem("cookieConsent", "declined");
    setIsVisible(false);
  };

  if (!isVisible) return null;

  return (
    <div className="cookie-consent-overlay">
      <div className="cookie-consent-banner">
        <div className="cookie-content">
          <p>
            {t("cookies.banner_text")}
            <a href="/privacy-policy" className="cookie-policy-link">
              {t("cookies.policy_link")}
            </a>
            .
          </p>
        </div>
        <div className="cookie-actions">
          <button className="cookie-btn decline" onClick={handleDecline}>
            {t("cookies.decline_btn")}
          </button>
          <button className="cookie-btn accept" onClick={handleAccept}>
            {t("cookies.accept_btn")}
          </button>
        </div>
      </div>
    </div>
  );
};

export default CookieConsent;
