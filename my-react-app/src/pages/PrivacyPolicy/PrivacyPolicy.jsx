import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../../components/PopupMenu/PopupMenu";
import "./PrivacyPolicy.css";

const PrivacyPolicy = () => {
  const { t } = useTranslation();
  const [menuOpen, setMenuOpen] = useState(false);

  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  const sections = t("privacy_policy.sections", { returnObjects: true });

  return (
    <div className="privacy-policy-page-root">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="privacy-policy-main-content">
        <div className="privacy-policy-content">
          <h1 className="privacy-title">{t("privacy_policy.title")}</h1>
          <p className="privacy-date">{t("privacy_policy.last_updated")}</p>

          <p className="privacy-intro">{t("privacy_policy.intro")}</p>

          <div className="privacy-sections">
            {Array.isArray(sections) &&
              sections.map((section, index) => (
                <div key={index} className="privacy-policy-section">
                  <h2 className="privacy-policy-section-title">
                    {section.title}
                  </h2>
                  <p className="privacy-policy-section-content">
                    {section.content}
                  </p>
                </div>
              ))}
          </div>

          <div className="privacy-footer-note">
            <p>
              {t(
                "privacy_policy.footer_note",
                "Якщо у вас виникли запитання, зв'яжіться з нами через розділ Контакти.",
              )}
            </p>
          </div>
        </div>
      </main>

      <Footer />
    </div>
  );
};

export default PrivacyPolicy;
