import { useNavigate } from "react-router-dom";
import { useCart } from "../../context/CartContext";

import { useAuth } from "../../context/useAuth";
import CartPopup from "../CartPopup/CartPopup";

import ShoppingBagIcon from "../../assets/icons/ShoppingBagIcon.svg";
import UserProfileIcon from "../../assets/icons/UserProfileIcon.svg";
import FlowerLabVladaLogo from "../../assets/icons/FlowerLabVladaLogo.svg";

import "./Header.css";

const Header = ({ onMenuOpen }) => {
  const navigate = useNavigate();
  const { cartOpen, setCartOpen, cartItems } = useCart();

  const { user } = useAuth();

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
              {user ? "Profile" : "Sign up/in"}
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
