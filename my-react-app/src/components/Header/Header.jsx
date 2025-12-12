import React from "react";
import { useNavigate } from "react-router-dom";
import { useCart } from "../../context/CartContext";
import CartPopup from "../../pages/CartPopup/CartPopup";


import ShoppingBagIcon from "../../assets/images/ShoppingBagIcon.svg";
import UserProfileIcon from "../../assets/images/UserProfileIcon.svg";
import FlowerLabVladaLogo from "../../assets/images/FlowerLabVladaLogo.svg";

import "./Header.css";

const Header = ({ onMenuOpen }) => {
  const navigate = useNavigate();
  const { cartOpen, setCartOpen, cartItems } = useCart();

  return (
    <>
      <header className="header">

        {/* LEFT SIDE */}
        <div className="header-left">

          {/* ☰ MENU BUTTON */}
          <button className="menu-btn" onClick={onMenuOpen}>
            ☰
          </button>

          <span className="lang">UA/ENG</span>
        </div>

        {/* LOGO */}
        <div className="logo-wrapper">
          <img
            src={FlowerLabVladaLogo}
            alt="Logo"
            className="logo"
            onClick={() => navigate("/")}
          />
        </div>

        {/* RIGHT SIDE */}
        <div className="header-right">
          <span className="currency">UAH/USD</span>

          {/* CART */}
          <button className="icon-btn" onClick={() => setCartOpen(true)}>
            <img src={ShoppingBagIcon} alt="Cart" className="icon" />
          </button>

          {/* POPUP CART */}
          <CartPopup
            isOpen={cartOpen}
            onClose={() => setCartOpen(false)}
            items={cartItems}
          />

          {/* PROFILE */}
          <button className="icon-btn profile-btn" onClick={() => navigate("/cabinet")}>
            <img src={UserProfileIcon} alt="Profile" className="icon" />
            <span className="profile-label">sign up/in</span>
          </button>
        </div>
      </header>
    </>
  );
};

export default Header;
