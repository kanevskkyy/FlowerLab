import { useNavigate } from "react-router-dom";
import { useCart } from "../../context/CartContext";
import { useSettings } from "../../context/useSettings";
import { useAuth } from "../../context/useAuth";
import { useTranslation } from "react-i18next";
import CartPopup from "../CartPopup/CartPopup";

import ShoppingBagIcon from "../../assets/icons/ShoppingBagIcon.svg";
import UserProfileIcon from "../../assets/icons/UserProfileIcon.svg";
import FlowerLabVladaLogo from "../../assets/icons/FlowerLabVladaLogo.svg";

import "./Header.css";

const Header = ({ onMenuOpen }) => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { cartOpen, setCartOpen, cartItems } = useCart();
  const { user } = useAuth();
  const { lang, setLang } = useSettings();

  return (
    <>
      <header className="header">
        <div className="header-left">
          <button
            className="menu-btn"
            onClick={onMenuOpen}
            aria-label="Open menu">
            ☰
          </button>

          <div className="text-btn desktop-only">
            <span
              className={lang === "UA" ? "active-text" : ""}
              onClick={() => setLang("UA")}>
              UA
            </span>
            <span> / </span>
            <span
              className={lang === "ENG" ? "active-text" : ""}
              onClick={() => setLang("ENG")}>
              ENG
            </span>
          </div>
        </div>

        <div className="logo-wrapper">
          <img
            src={FlowerLabVladaLogo}
            alt="Flower Lab Vlada"
            className="logo"
            onClick={() => navigate("/")}
          />
        </div>

        <div className="header-right">
          <button className="icon-btn" onClick={() => setCartOpen(true)}>
            <img src={ShoppingBagIcon} alt="Cart" className="icon" />
            {cartItems.length > 0 && (
              <span className="cart-badge">{cartItems.length}</span>
            )}
          </button>

          <button
            className="icon-btn profile-btn"
            onClick={() => navigate("/cabinet")}>
            <img
              src={user?.photoUrl || UserProfileIcon}
              alt="Profile"
              className={`icon ${user?.photoUrl ? "profile-avatar" : ""}`}
            />
            <span className="profile-label">
              {user ? t("auth.profile") : t("auth.login")}
            </span>
          </button>
        </div>
      </header>

      <CartPopup
        isOpen={cartOpen}
        onClose={() => setCartOpen(false)}
        items={cartItems}
      />
    </>
  );
};

export default Header;
