import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/useAuth";
import { useSettings } from "../../context/useSettings";
import { useTranslation } from "react-i18next";
import "./PopupMenu.css";

export default function PopupMenu({ isOpen, onClose }) {
  const { t } = useTranslation();
  const { lang, setLang } = useSettings();
  const navigate = useNavigate();

  const { user } = useAuth();

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
            {t("nav.home")}
          </li>

          {/* CATALOG ACCORDION */}
          <li className="menu-item-group">
            <div
              className="menu-item-header"
              onClick={() => setIsCatalogOpen(!isCatalogOpen)}>
              <span className="menu-item">{t("nav.catalog")}</span>
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
                  {t("nav.bouquets")}
                </li>
                {/* Поки що ведемо на каталог, пізніше можна додати ?category=gifts */}
                <li
                  className="submenu-item"
                  onClick={() => handleNav("/gifts")}>
                  {t("nav.gifts")}
                </li>
              </ul>
            </div>
          </li>

          <li className="menu-item" onClick={() => handleNav("/about")}>
            {t("nav.about")}
          </li>
          {!user && (
            <li className="menu-item" onClick={() => handleNav("/track-order")}>
              {t("nav.track")}
            </li>
          )}
        </ul>

        <div className="mobile-lang-switch">
          <span
            className={lang === "ua" ? "active-lang" : ""}
            onClick={() => setLang("ua")}>
            UA
          </span>
          <span className="lang-divider"> / </span>
          <span
            className={lang === "en" ? "active-lang" : ""}
            onClick={() => setLang("en")}>
            ENG
          </span>
        </div>
      </div>
    </div>
  );
}
