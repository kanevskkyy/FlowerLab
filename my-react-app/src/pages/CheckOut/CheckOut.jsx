import React, { useState } from "react";
import { useLocation } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";
import PaymentTimer from "../../components/PaymentTimer/PaymentTimer";
import "./CheckOut.css";

import { useCheckOut } from "./hooks/useCheckOut";

const CheckOut = () => {
  const location = useLocation();
  const orderData = location.state?.orderData || {};
  const createdAt = orderData.createdAt;

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
    handleCancelPayment,
    loading,
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
            <p className="order-number">Order Payment Details</p>

            {createdAt && (
              <div style={{ marginBottom: "1rem" }}>
                <PaymentTimer
                  createdAt={createdAt}
                  onExpire={() => window.location.reload()}
                />
              </div>
            )}

            <div className="total-section">
              <span>Total to Pay:</span>
              <span className="total-amount">{total}.00 UAH</span>
            </div>
          </div>

          <div
            className="privat24-info"
            style={{ marginTop: "30px", marginBottom: "30px" }}>
            <p>
              You will be redirected to the secure <b>LiqPay</b> payment page.
              <br />
              Supported methods: Card, Apple Pay, Google Pay, Privat24.
            </p>
          </div>

          {/* Complete Payment Button */}
          <button
            className="complete-payment-btn"
            onClick={handleCompletePayment}
            disabled={loading}
            style={{
              opacity: loading ? 0.7 : 1,
              cursor: loading ? "not-allowed" : "pointer",
            }}>
            {loading ? "Redirecting to LiqPay..." : "Proceed to Secure Payment"}
          </button>

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
