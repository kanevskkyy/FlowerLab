import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./PopupMenu.css";

const PopupMenu = ({ isOpen, onClose }) => {
  const [catalogOpen, setCatalogOpen] = useState(false);
  const navigate = useNavigate();

  const goTo = (path) => {
    navigate(path);
    onClose();
  };

  return (
    <div
      className={`popup-menu-overlay ${isOpen ? "open" : ""}`}
      onClick={onClose}
    >
      <div className="popup-menu" onClick={(e) => e.stopPropagation()}>
        <button className="close-btn" onClick={onClose}>☰</button>

        <ul className="menu-list">

          <li className="menu-item" onClick={() => goTo("/")}>HOME PAGE</li>

          <li className="menu-item" onClick={() => goTo("/about")}>ABOUT US</li>

          <li
            className="menu-item menu-row"
            onClick={() => setCatalogOpen(!catalogOpen)}
          >
            <span>CATALOG</span>
            <span className="arrow">{catalogOpen ? "∨" : ">"}</span>
          </li>

          {catalogOpen && (
            <ul className="submenu">
              <li onClick={() => goTo("/catalog")}>Bouquets</li>
              <li onClick={() => goTo("/catalog")}>Baskets</li>
              <li onClick={() => goTo("/catalog")}>Boxes</li>
              <li onClick={() => goTo("/catalog")}>Gifts</li>
              <li onClick={() => goTo("/catalog")}>Balloons</li>
            </ul>
          )}
        </ul>
      </div>
    </div>
  );
};

export default PopupMenu;
