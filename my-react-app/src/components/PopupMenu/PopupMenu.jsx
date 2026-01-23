import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSettings } from "../../context/useSettings";
import "./PopupMenu.css";

export default function PopupMenu({ isOpen, onClose }) {
  const navigate = useNavigate();
  const { lang, toggleLang, currency, toggleCurrency } = useSettings();

  // Стан для відкриття/закриття підменю каталогу
  const [isCatalogOpen, setIsCatalogOpen] = useState(false);

  const handleNav = (path) => {
    navigate(path);
    onClose();
  };

  return (
    <div
      className={`popup-menu-overlay ${isOpen ? "open" : ""}`}
      onClick={onClose}>
      <div className="popup-menu" onClick={(e) => e.stopPropagation()}>
        {/* Close Button */}
        <button className="close-btn" onClick={onClose}>
          <span style={{ fontSize: "24px", fontWeight: "300" }}>✕</span>
        </button>

        {/* Menu List */}
        <ul className="menu-list">
          <li className="menu-item" onClick={() => handleNav("/")}>
            Home
          </li>

          {/* CATALOG ACCORDION */}
          <li className="menu-item-group">
            <div
              className="menu-item-header"
              onClick={() => setIsCatalogOpen(!isCatalogOpen)}>
              <span className="menu-item">Catalog</span>
              <span className={`menu-arrow ${isCatalogOpen ? "open" : ""}`}>
                ›
              </span>
            </div>

            {/* Submenu with animation class */}
            <div className={`submenu-wrapper ${isCatalogOpen ? "open" : ""}`}>
              <ul className="submenu-list">
                <li
                  className="submenu-item"
                  onClick={() => handleNav("/catalog")}>
                  Bouquets
                </li>
                {/* Поки що ведемо на каталог, пізніше можна додати ?category=gifts */}
                <li
                  className="submenu-item"
                  onClick={() => handleNav("/gifts")}>
                  Gifts
                </li>
              </ul>
            </div>
          </li>

          <li className="menu-item" onClick={() => handleNav("/about")}>
            About Us
          </li>
        </ul>

        {/* Settings */}
        <div className="popup-settings">
          <div className="setting-row">
            <span className="setting-label">Language:</span>
            <button className="popup-text-btn" onClick={toggleLang}>
              <span className={lang === "UA" ? "active-text" : ""}>UA</span>/
              <span className={lang === "ENG" ? "active-text" : ""}>ENG</span>
            </button>
          </div>

          <div className="setting-row">
            <span className="setting-label">Currency:</span>
            <button className="popup-text-btn" onClick={toggleCurrency}>
              <span className={currency === "UAH" ? "active-text" : ""}>
                UAH
              </span>
              /
              <span className={currency === "USD" ? "active-text" : ""}>
                USD
              </span>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
