import React, { useState } from "react";
import "./AboutUs.css";
import PopupMenu from "../popupMenu/PopupMenu";

import UserProfileIcon from "../../assets/images/UserProfileIcon.svg";
import ShoppingBagIcon from "../../assets/images/ShoppingBagIcon.svg";

const AboutUs = () => {
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <div className="about-page">

      {/* ================= HEADER ================= */}
      <header className="header">
        <div className="header-left">
          <button className="menu-btn" onClick={() => setMenuOpen(true)}>☰</button>
          <span className="lang">UA/ENG</span>
        </div>

        <div className="logo">[LOGO]</div>

        <div className="header-right">
          <span className="currency">UAH/USD</span>
          <button className="icon-btn">
            <img src={ShoppingBagIcon} className="icon" />
          </button>
          <button className="icon-btn">
            <img src={UserProfileIcon} className="icon" />
          </button>
        </div>
      </header>

      {/* Popup */}
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      {/* ================= BREADCRUMBS ================= */}
      <div className="breadcrumbs">
        <span>Main page</span> <span className="arrow">›</span> <span className="gray">About us</span>
      </div>

      {/* ================= TITLE ================= */}
      <h1 className="page-title">ABOUT US</h1>

      {/* ================= BIG IMAGE BLOCK ================= */}
      <div className="big-image-block">
        <button className="nav-left">‹</button>
        <div className="image-placeholder"></div>
        <button className="nav-right">›</button>
      </div>

      {/* ================= TEXT ================= */}
      <div className="about-text">
        <p>
          Welcome to our flower shop — a place where every bouquet tells a story.
          We believe that flowers are more than just a gift; they are a way to express emotions,
          share happiness, and make every moment memorable.
        </p>

        <p>
          Our team carefully selects fresh flowers every day to ensure the highest quality and beauty in every arrangement.
          Whether it's a romantic gesture, a celebration, or just a small token of appreciation,
          we create bouquets that speak from the heart.
        </p>

        <p>
          We take pride in our attention to detail, creative designs, and friendly service.
          Each bouquet is handcrafted with love, tailored to fit your style and occasion.
        </p>
      </div>

      {/* ================= WHY CHOOSE US ================= */}
      <h2 className="section-title">WHY CHOOSE US?</h2>

      <div className="why-grid">
        <div className="why-item">
          <div className="icon-placeholder"></div>
          <p>Freshness Guaranteed</p>
        </div>

        <div className="why-item">
          <div className="icon-placeholder"></div>
          <p>Handcrafted with Love</p>
        </div>

        <div className="why-item">
          <div className="icon-placeholder"></div>
          <p>Fast & Reliable Delivery</p>
        </div>

        <div className="why-item">
          <div className="icon-placeholder"></div>
          <p>Personalized Designs</p>
        </div>
      </div>

      {/* ================= LOCATIONS ================= */}
      <h2 className="section-title">Our locations:</h2>

      <div className="locations">
        <div className="location">
          <span className="loc-icon">📍</span>
          <span>м. Чернівці, вул. Василя Александрі 1</span>
        </div>

        <div className="location">
          <span className="loc-icon">📍</span>
          <span>м. Чернівці, вул. Гершана 2а</span>
        </div>
      </div>

      {/* ================= FOOTER ================= */}
      <footer className="footer">
        <div className="footer-col">
          <p>м. Чернівці, вул. Гершана 2а</p>
          <p>вул. Василя Александрі 1</p>
        </div>

        <div className="footer-col">
          <p>+38 050 159 12 12</p>
        </div>

        <div className="footer-col">
          <p>@florist_vlada</p>
        </div>
      </footer>
    </div>
  );
};

export default AboutUs;
