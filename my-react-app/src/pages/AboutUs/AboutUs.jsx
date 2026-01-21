import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "./AboutUs.css";

import PopupMenu from "../popupMenu/PopupMenu";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";

import LocationIcon from "../../assets/images/location-icon.svg";
import PhoneIcon from "../../assets/images/phone-icon.svg";
import InstagramIcon from "../../assets/images/instagram-icon.svg";
import SparklesIcon from "../../assets/images/sparkles-icon.svg";
import FlowerIcon from "../../assets/images/flower-icon.svg";
import HandIcon from "../../assets/images/hand-icon.svg";
import TruckIcon from "../../assets/images/truck-icon.svg";

import Photo1 from "../../assets/images/headerPhoto.svg";
import Photo2 from "../../assets/images/bouquet1.JPG";

const AboutUs = () => {
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
          Main page
        </span>
        <span className="arrow">›</span>
        <span className="gray">About us</span>
      </div>

      <h1 className="page-title">ABOUT US</h1>

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
          We are FlowerLab Vlada, a floral design studio.
          <br />
          Our team creates premium bouquets and signature floral arrangements
          with a focus on quality, style, and emotion. Daily deliveries of fresh
          flowers, a wide selection, and caring service make us the studio
          people trust in Chernivtsi.
          <br />
          We are the first in Ukraine to introduce the format of giant bouquets
          and large signature box compositions. This has become part of our
          unique style and brand identity.
          <br />
          We continuously grow, learn, and improve our work. We expand our team,
          introduce new shapes, colors, and arrangements, explore global trends,
          and adapt them for our clients.
          <br />
          Our studio also collaborates with various brands and venues, creating
          floral solutions for events, gifts, and visual decor.
          <br />
          FlowerLab Vlada is quality, style, and service that inspire and leave
          a lasting impression.
        </p>
      </div>

      {/* WHY */}
      <h2 className="section-title">WHY CHOOSE US?</h2>

      <div className="why-grid">
        <div className="why-item">
          <p>
            Freshness
            <br />
            Guaranteed
          </p>
          <div className="icon-placeholder">
            <img src={FlowerIcon} className="why-icon" alt="Freshness" />
          </div>
        </div>

        <div className="why-item">
          <p>
            Handcrafted
            <br />
            with Love
          </p>
          <div className="icon-placeholder">
            <img src={HandIcon} className="why-icon" alt="Handcrafted" />
          </div>
        </div>

        <div className="why-item">
          <p>
            Fast & Reliable
            <br />
            Delivery
          </p>
          <div className="icon-placeholder">
            <img src={TruckIcon} className="why-icon" alt="Delivery" />
          </div>
        </div>

        <div className="why-item">
          <p>
            Personalized
            <br />
            Designs
          </p>
          <div className="icon-placeholder">
            <img src={SparklesIcon} className="why-icon" alt="Personalized" />
          </div>
        </div>
      </div>

      {/* CONTACTS & LOCATIONS */}
      <div className="info-container">
        <div className="info-column">
          <h2 className="info-title">Our locations:</h2>
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
          <h2 className="info-title">Our contact information:</h2>
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
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default AboutUs;
