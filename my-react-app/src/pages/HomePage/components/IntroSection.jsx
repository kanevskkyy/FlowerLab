import React from "react";
import { useTranslation } from "react-i18next";

function IntroSection() {
  const { t } = useTranslation();
  return (
    <section className="intro-section">
      <div className="intro-content">
        <h2 className="intro-title">{t("intro.title")}</h2>
        <p className="intro-desc">{t("intro.desc")}</p>
      </div>
    </section>
  );
}

export default IntroSection;
