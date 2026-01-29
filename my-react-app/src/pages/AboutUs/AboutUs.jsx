import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import "./AboutUs.css";

import PopupMenu from "../../components/PopupMenu/PopupMenu";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";

import LocationIcon from "../../assets/icons/location-icon.svg";
import PhoneIcon from "../../assets/icons/phone-icon.svg";
import InstagramIcon from "../../assets/icons/instagram-icon.svg";
import TelegramIcon from "../../assets/icons/telegram-icon.svg";
import SparklesIcon from "../../assets/icons/sparkles-icon.svg";
import FlowerIcon from "../../assets/icons/flower-icon.svg";
import HandIcon from "../../assets/icons/hand-icon.svg";
import TruckIcon from "../../assets/icons/truck-icon.svg";

import Photo1 from "../../assets/images/headerPhoto.svg";
import Photo2 from "../../assets/images/bouquet1.webp";

const AboutUs = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [menuOpen, setMenuOpen] = useState(false);

  const images = [Photo1, Photo2, Photo1, Photo2];

  const [current, setCurrent] = useState(0);
  const [fade, setFade] = useState(true);

  // Автоматичний слайд з плавним переходом
  useEffect(() => {
    const interval = setInterval(() => {
      setFade(false);

      setTimeout(() => {
        setCurrent((prev) => (prev + 1) % images.length);
        setFade(true);
      }, 800); // плавна зміна
    }, 5000); // показ кожного зображення 5 секунд

    return () => clearInterval(interval);
  }, [images.length]);

  return (
    <div className="about-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      {/* Breadcrumbs */}
      <div className="breadcrumbs">
        <span onClick={() => navigate("/")} style={{ cursor: "pointer" }}>
          {t("about_page.main")}
        </span>
        <span className="arrow">›</span>
        <span className="gray">{t("nav.about")}</span>
      </div>

      <h1 className="page-title">{t("about_page.title")}</h1>

      {/* ================= SLIDER ================= */}
      <div className="big-image-block">
        <img
          src={images[current]}
          className={`slider-image ${fade ? "fade-in" : "fade-out"}`}
          alt="Slide"
        />
      </div>

      {/* TEXT */}
      <div className="about-text">
        <p className="break-line" style={{ lineHeight: "2.0" }}>
          {t("about_page.history")}
        </p>
      </div>

      {/* WHY */}
      <h2 className="section-title">{t("about_page.why_title")}</h2>

      <div className="why-grid">
        <div className="why-item">
          <p>{t("about_page.freshness")}</p>
          <div className="icon-placeholder">
            <img src={FlowerIcon} className="why-icon" alt="Freshness" />
          </div>
        </div>

        <div className="why-item">
          <p>{t("about_page.handcrafted")}</p>
          <div className="icon-placeholder">
            <img src={HandIcon} className="why-icon" alt="Handcrafted" />
          </div>
        </div>

        <div className="why-item">
          <p>{t("about_page.delivery")}</p>
          <div className="icon-placeholder">
            <img src={TruckIcon} className="why-icon" alt="Delivery" />
          </div>
        </div>

        <div className="why-item">
          <p>{t("about_page.personalized")}</p>
          <div className="icon-placeholder">
            <img src={SparklesIcon} className="why-icon" alt="Personalized" />
          </div>
        </div>
      </div>

      {/* CONTACTS & LOCATIONS */}
      <div className="info-container">
        <div className="info-column">
          <h2 className="info-title">{t("about_page.locations")}</h2>
          <div className="info-items">
            <div className="info-item footer-location">
              <img
                src={LocationIcon}
                className="footer-icon location-icon"
                alt="Location"
              />
              <a
                href="https://maps.app.goo.gl/myw4J2CtWA9AGVuj6"
                target="_blank"
                rel="noopener noreferrer"
                className="footer-link-text">
                <p>м. Чернівці, вул. Василя Александрі, 1</p>
              </a>
            </div>

            <div className="info-item footer-location">
              <img
                src={LocationIcon}
                className="footer-icon location-icon"
                alt="Location"
              />
              <a
                href="https://maps.app.goo.gl/11uTt4nTxqpv2K3w5"
                target="_blank"
                rel="noopener noreferrer"
                className="footer-link-text">
                <p>м. Чернівці, вул. Герцена 2а</p>
              </a>
            </div>
          </div>
        </div>

        <div className="info-column">
          <h2 className="info-title">{t("about_page.contacts")}</h2>
          <div className="info-items">
            <div className="info-item footer-phone">
              <img
                src={PhoneIcon}
                className="footer-icon phone-icon"
                alt="Phone"
              />
              <a href="tel:+380501591912" className="footer-link-single">
                <p>+38 050 159 19 12</p>
              </a>
            </div>

            <div className="info-item footer-instagram">
              <img
                src={InstagramIcon}
                className="footer-icon instagram-icon"
                alt="Instagram"
              />
              <a
                href="https://www.instagram.com/flowerlab_vlada/"
                target="_blank"
                rel="noopener noreferrer"
                className="footer-link-single">
                <p>@flowerlab_vlada</p>
              </a>
            </div>

            <div className="info-item footer-telegram">
              <img
                src={TelegramIcon}
                className="footer-icon telegram-icon"
                alt="Telegram"
              />
              <a
                href="https://t.me/flower_lab_vlada"
                target="_blank"
                rel="noopener noreferrer"
                className="footer-link-single">
                <p>@flower_lab_vlada</p>
              </a>
            </div>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default AboutUs;
