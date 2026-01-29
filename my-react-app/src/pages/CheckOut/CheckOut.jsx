import React, { useState } from "react";
import { useLocation } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../../components/PopupMenu/PopupMenu";
import PaymentTimer from "../../components/PaymentTimer/PaymentTimer";
import "./CheckOut.css";

import { useTranslation, Trans } from "react-i18next";
import { useCheckOut } from "./hooks/useCheckOut";

const CheckOut = () => {
  const { t } = useTranslation();
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
        <h1 className="checkout-title">{t("checkout.title")}</h1>

        <div className="checkout-container">
          {/* Payment Info Block */}
          <div className="payment-info-block">
            <h2>LIQPAY</h2>
            <p className="payment-label">{t("checkout.payment_info")}</p>
            <p className="order-number">{t("checkout.order_details")}</p>

            {createdAt && (
              <div style={{ marginBottom: "1rem" }}>
                <PaymentTimer
                  createdAt={createdAt}
                  onExpire={() => window.location.reload()}
                />
              </div>
            )}

            <div className="total-section">
              <span>{t("checkout.total_to_pay")}</span>
              <span className="total-amount">{total}.00 UAH</span>
            </div>
          </div>

          <div
            className="privat24-info"
            style={{ marginTop: "30px", marginBottom: "30px" }}>
            <p>
              <Trans i18nKey="checkout.redirect_msg" />
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
            {loading ? t("checkout.redirecting") : t("checkout.proceed_btn")}
          </button>

          {/* Cancel Button */}
          <button className="cancel-payment-btn" onClick={handleCancelPayment}>
            {t("checkout.cancel_btn")}
          </button>
        </div>
      </main>

      <Footer />
    </div>
  );
};

export default CheckOut;
