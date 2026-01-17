import React, { useState } from "react";
import { useLocation } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";
import "./CheckOut.css";

import { useCheckOut } from "./hooks/useCheckOut";

const CheckOut = () => {
  const location = useLocation();
  const orderData = location.state?.orderData || {};
  
  const [menuOpen, setMenuOpen] = useState(false);

  const {
    paymentMethod,
    setPaymentMethod,
    cardNumber,
    expiry,
    cvv,
    sendReceipt,
    setSendReceipt,
    orderNumber,
    total,
    handleCardNumberChange,
    handleExpiryChange,
    handleCvvChange,
    handleCompletePayment,
    handleCancelPayment
  } = useCheckOut({ orderData });

  return (
    <div className="page-wrapper checkout-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="checkout">
        <h1 className="checkout-title">CHECKOUT</h1>

        <div className="checkout-container">
          
          {/* Payment Info Block */}
          <div className="payment-info-block">
            <h2>LIQPAY</h2>
            <p className="payment-label">Payment Information</p>
            <p className="order-number">Order Payment №{orderNumber}</p>
            
            <div className="total-section">
              <span>Total to Pay:</span>
              <span className="total-amount">{total}.00 UAH</span>
            </div>
          </div>

          {/* Quick Payment Buttons */}
          <div className="quick-payment-buttons">
            <button className="quick-pay-btn privat24-btn">
              <span className="icon-24">24</span>
              Pay
            </button>
            <button className="quick-pay-btn google-pay-btn">
              Pay with <span className="google-icon">G</span> Pay
            </button>
          </div>

          <div className="divider">
            <span>or</span>
          </div>

          {/* Payment Method Selection */}
          <div className="payment-methods">
            <button
              className={`payment-method-btn ${paymentMethod === "card" ? "active" : ""}`}
              onClick={() => setPaymentMethod("card")}
            >
              <div className="method-icon">💳</div>
              <span>Card</span>
            </button>
            <button
              className={`payment-method-btn ${paymentMethod === "privat24" ? "active" : ""}`}
              onClick={() => setPaymentMethod("privat24")}
            >
              <div className="method-icon privat-icon">24</div>
              <span>Privat24</span>
            </button>
          </div>

          {/* Card Payment Form */}
          {paymentMethod === "card" && (
            <div className="card-form">
              <div className="form-group">
                <label>Credit card number</label>
                <input
                  type="text"
                  placeholder="0000 0000 0000 0000"
                  value={cardNumber}
                  onChange={handleCardNumberChange}
                  maxLength="19"
                />
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label>Expires</label>
                  <input
                    type="text"
                    placeholder="MM/YY"
                    value={expiry}
                    onChange={handleExpiryChange}
                    maxLength="5"
                  />
                </div>

                <div className="form-group">
                  <label>CVV2</label>
                  <input
                    type="password"
                    placeholder="•••"
                    value={cvv}
                    onChange={handleCvvChange}
                    maxLength="3"
                  />
                </div>
              </div>

              <label className="checkbox-label">
                <input
                  type="checkbox"
                  checked={sendReceipt}
                  onChange={(e) => setSendReceipt(e.target.checked)}
                />
                <span>Send receipt to e-mail</span>
              </label>

              <p className="terms-text">
                By clicking "Pay," you acknowledge that you've reviewed the service details
                and accept the terms of our <span className="link">public agreement</span>
              </p>
            </div>
          )}

          {/* Privat24 Info */}
          {paymentMethod === "privat24" && (
            <div className="privat24-info">
              <p>You will be redirected to Privat24 to complete the payment</p>
            </div>
          )}

          {/* Complete Payment Button */}
          {paymentMethod && (
            <button className="complete-payment-btn" onClick={handleCompletePayment}>
              Complete payment
            </button>
          )}

          {/* Cancel Button */}
          <button className="cancel-payment-btn" onClick={handleCancelPayment}>
            Cancel payment
          </button>
        </div>
      </main>

      <Footer />
    </div>
  );
};

export default CheckOut;