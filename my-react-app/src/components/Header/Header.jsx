import { useNavigate } from "react-router-dom";
import { useCart } from "../../context/CartContext";
import { useSettings } from "../../context/useSettings"; // Імпорт нашого нового хука
import { useAuth } from "../../context/useAuth";
import CartPopup from "../../pages/CartPopup/CartPopup";

// Assets (перевір, чи шляхи до картинок правильні у твоєму проєкті)
import ShoppingBagIcon from "../../assets/icons/ShoppingBagIcon.svg";
import UserProfileIcon from "../../assets/icons/UserProfileIcon.svg";
import FlowerLabVladaLogo from "../../assets/icons/FlowerLabVladaLogo.svg";

import "./Header.css";

const Header = ({ onMenuOpen }) => {
  const navigate = useNavigate();
  const { cartOpen, setCartOpen, cartItems } = useCart();

  // Отримуємо дані з глобального контексту
  const { lang, toggleLang, currency, toggleCurrency } = useSettings();
  const { user } = useAuth();

  return (
    <>
      <header className="header">
        {/* === LEFT SIDE (Menu + Lang) === */}
        <div className="header-left">
          {/* Burger Menu */}
          <button
            className="menu-btn"
            onClick={onMenuOpen}
            aria-label="Open menu">
            ☰
          </button>

          {/* Language Switcher (Ховається на мобільному через CSS .desktop-only) */}
          <button className="text-btn desktop-only" onClick={toggleLang}>
            <span className={lang === "UA" ? "active-text" : ""}>UA</span>/
            <span className={lang === "ENG" ? "active-text" : ""}>ENG</span>
          </button>
        </div>

        {/* === CENTER (Logo) - Абсолютно по центру === */}
        <div className="logo-wrapper">
          <img
            src={FlowerLabVladaLogo}
            alt="Flower Lab Vlada"
            className="logo"
            onClick={() => navigate("/")}
          />
        </div>

        {/* === RIGHT SIDE (Currency + Cart + Profile) === */}
        <div className="header-right">
          {/* Currency Switcher (Ховається на мобільному) */}
          <button className="text-btn desktop-only" onClick={toggleCurrency}>
            <span className={currency === "UAH" ? "active-text" : ""}>UAH</span>
            /
            <span className={currency === "USD" ? "active-text" : ""}>USD</span>
          </button>

          {/* Cart */}
          <button className="icon-btn" onClick={() => setCartOpen(true)}>
            <img src={ShoppingBagIcon} alt="Cart" className="icon" />
            {/* Бейдж з кількістю товарів */}
            {cartItems.length > 0 && (
              <span className="cart-badge">{cartItems.length}</span>
            )}
          </button>

          {/* Profile */}
          <button
            className="icon-btn profile-btn"
            onClick={() => navigate("/cabinet")}>
            <img 
              src={user?.photoUrl || UserProfileIcon} 
              alt="Profile" 
              className={`icon ${user?.photoUrl ? 'profile-avatar' : ''}`} 
            />
            <span className="profile-label">
              {user ? "Profile" : "Sign up/in"}
            </span>
          </button>
        </div>
      </header>

      {/* Cart Popup */}
      <CartPopup
        isOpen={cartOpen}
        onClose={() => setCartOpen(false)}
        items={cartItems}
      />
    </>
  );
};

export default Header;
