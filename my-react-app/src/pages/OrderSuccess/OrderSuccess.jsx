import React, { useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { useCart } from "../../context/CartContext";
import "./OrderSuccess.css";

// You might want to import a checkmark icon or use an SVG directly
// import checkCircle from "../../assets/icons/check-circle.svg";

const OrderSuccess = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { clearCart } = useCart();
  const { orderNumber } = location.state || {}; // Fallback if navigated directly

  useEffect(() => {
    clearCart();
    // Also clear the persisted gifts/card state from OrderPlacement
    localStorage.removeItem("order_selectedGifts");
    localStorage.removeItem("order_isCardAdded");
  }, [clearCart]);

  return (
    <div className="page-wrapper success-page">
      <Header />
      <main className="success-content">
        <div className="success-card">
          <div className="icon-container">
            <svg
              width="80"
              height="80"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg">
              <path
                d="M12 22C17.5 22 22 17.5 22 12C22 6.5 17.5 2 12 2C6.5 2 2 6.5 2 12C2 17.5 6.5 22 12 22Z"
                stroke="#d46aac"
                strokeWidth="1.5"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M7.75 12L10.58 14.83L16.25 9.17004"
                stroke="#d46aac"
                strokeWidth="1.5"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          </div>
          <h1>Thank You!</h1>
          <p className="success-message">
            Your order has been placed successfully.
          </p>
          {orderNumber && (
            <p className="order-number-text">
              Order number: <strong>#{orderNumber}</strong>
            </p>
          )}
          <p className="email-note">
            We have sent a confirmation email to you.
          </p>
          <button className="continue-btn" onClick={() => navigate("/")}>
            Continue Shopping
          </button>
        </div>
      </main>
      <Footer />
    </div>
  );
};

export default OrderSuccess;
