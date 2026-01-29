import { useTranslation } from "react-i18next";
import bouquet4 from "../../../assets/images/about-image.webp";

function AboutSection() {
  const { t } = useTranslation();
  return (
    <section className="about-section">
      <h2 className="section-title">{t("home.sections.about_title")}</h2>
      <div className="about-content">
        <div className="about-text">
          <p>{t("home.sections.about_desc")}</p>
        </div>
        <div className="about-image">
          <img
            src={bouquet4}
            alt="About banner"
            className="about-img"
            loading="lazy"
          />
        </div>
      </div>
    </section>
  );
}

export default AboutSection;
