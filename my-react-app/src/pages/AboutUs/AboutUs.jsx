import React, { useState } from "react";
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

const AboutUs = () => {
  const [menuOpen, setMenuOpen] = useState(false);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);

  const images = [
    "/path/to/image1.jpg",
    "/path/to/image2.jpg",
    "/path/to/image3.jpg",
  ];

  const handlePrevImage = () => {
    setCurrentImageIndex((prev) =>
      prev === 0 ? images.length - 1 : prev - 1
    );
  };

  const handleNextImage = () => {
    setCurrentImageIndex((prev) =>
      prev === images.length - 1 ? 0 : prev + 1
    );
  };

  return (
    <div className="about-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />

      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      {/* ================= BREADCRUMBS ================= */}
      <div className="breadcrumbs">
        <span>Main page</span>
        <span className="arrow">›</span>
        <span className="gray">About us</span>
      </div>

      {/* ================= TITLE ================= */}
      <h1 className="page-title">ABOUT US</h1>

      {/* ================= BIG IMAGE BLOCK ================= */}
      <div className="big-image-block">
        <button className="nav-left" onClick={handlePrevImage}>‹</button>

        <div
          className="image-placeholder"
          style={{
            backgroundImage: images[currentImageIndex]
              ? `url(${images[currentImageIndex]})`
              : "none",
          }}
        ></div>

        <button className="nav-right" onClick={handleNextImage}>›</button>
      </div>

      {/* ================= TEXT ================= */}
      <div className="about-text">
        <p>We are FlowerLab Vlada, a floral design studio.</p>
        <p>
          Our team creates premium bouquets and signature floral arrangements with a focus on quality,
          style, and emotion…
        </p>
        <p>
          FlowerLab Vlada is quality, style, and service that inspire and leave a lasting impression.
        </p>
      </div>

      {/* ================= WHY CHOOSE US ================= */}
      <h2 className="section-title">WHY CHOOSE US?</h2>

      <div className="why-grid">
        <div className="why-item">
          <p>Freshness<br />Guaranteed</p>
          <div className="icon-placeholder">
            <img src={FlowerIcon} className="why-icon" alt="" />
          </div>
        </div>

        <div className="why-item">
          <p>Handcrafted<br />with Love</p>
          <div className="icon-placeholder">
            <img src={HandIcon} className="why-icon" alt="" />
          </div>
        </div>

        <div className="why-item">
          <p>Fast & Reliable<br />Delivery</p>
          <div className="icon-placeholder">
            <img src={TruckIcon} className="why-icon" alt="" />
          </div>
        </div>

        <div className="why-item">
          <p>Personalized<br />Designs</p>
          <div className="icon-placeholder">
            <img src={SparklesIcon} className="why-icon" alt="" />
          </div>
        </div>
      </div>

      {/* ================= LOCATIONS & CONTACTS ================= */}
      <div className="info-container">
        
        {/* LEFT COLUMN — LOCATIONS */}
        <div className="info-column">
          <h2 className="section-title">Our locations:</h2>

          <div className="info-items">

            <div className="footer-item footer-location">
              <img src={LocationIcon} alt="Location" className="footer-icon location-icon" />
              <div className="footer-text">
                <a
                  href="https://maps.app.goo.gl/myw4J2CtWA9AGVuj6"
                  target="_blank"
                  rel="noopener noreferrer"
                  className="footer-link-text"
                >
                  <p>м. Чернівці, вул. Василя Александрі, 1</p>
                </a>
              </div>
            </div>

            <div className="footer-item footer-location">
              <img src={LocationIcon} alt="Location" className="footer-icon location-icon" />
              <div className="footer-text">
                <a
                  href="https://maps.app.goo.gl/11uTt4nTxqpv2K3w5"
                  target="_blank"
                  rel="noopener noreferrer"
                  className="footer-link-text"
                >
                  <p>м. Чернівці, вул. Герцена 2а</p>
                </a>
              </div>
            </div>

          </div>
        </div>

        {/* RIGHT COLUMN — CONTACTS */}
        <div className="info-column">
          <h2 className="section-title">Our contact information:</h2>

          <div className="info-items">

            <div className="footer-item footer-phone">
              <img src={PhoneIcon} alt="Phone" className="footer-icon phone-icon" />
              <a
                href="tel:+380501591912"
                className="footer-link-single"
              >
                <div className="footer-text">
                  <p>+38 050 159 19 12</p>
                </div>
              </a>
            </div>

            <div className="footer-item footer-instagram">
              <img src={InstagramIcon} alt="Instagram" className="footer-icon instagram-icon" />
              <a
                href="https://www.instagram.com/flowerlab_vlada/"
                target="_blank"
                rel="noopener noreferrer"
                className="footer-link-single"
              >
                <div className="footer-text">
                  <p>@flowerlab_vlada</p>
                </div>
              </a>
            </div>

          </div>
        </div>

      </div>

      {/* ================= FOOTER ================= */}
      <Footer />
    </div>
  );
};

export default AboutUs;