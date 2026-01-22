import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { useCart } from "../../context/CartContext";
import PaymentTimer from "../../components/PaymentTimer/PaymentTimer";
import orderService from "../../services/orderService";
import "./OrderSuccess.css";

// You might want to import a checkmark icon or use an SVG directly
// import checkCircle from "../../assets/icons/check-circle.svg";

const OrderSuccess = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { clearCart } = useCart();
  const { orderNumber: stateOrderNumber } = location.state || {};

  // Support URL query params for payment callbacks (e.g. ?orderId=...)
  const searchParams = new URLSearchParams(location.search);
  const paramOrderNumber =
    searchParams.get("orderId") || searchParams.get("orderNumber");

  const orderNumber = stateOrderNumber || paramOrderNumber;

  const [orderDate, setOrderDate] = useState(null);
  const [orderStatus, setOrderStatus] = useState(null);

  useEffect(() => {
    // Only clear cart if we came from our own checkout (state present)
    if (stateOrderNumber) {
      clearCart();
      localStorage.removeItem("order_selectedGifts");
      localStorage.removeItem("order_isCardAdded");
    }
  }, [clearCart, stateOrderNumber]);

  useEffect(() => {
    if (location.state?.mockOrder) {
      setOrderDate(location.state.mockOrder.createdAt);
      setOrderStatus(location.state.mockOrder.status);
      return;
    }

    if (!orderNumber) return;

    const fetchOrderData = async () => {
      try {
        const order = await orderService.getById(orderNumber);
        if (order) {
          setOrderDate(order.createdAt);
          setOrderStatus(order.status?.name || order.status);
        }
      } catch (error) {
        console.error("Failed to fetch order details for timer", error);
      }
    };

    fetchOrderData();
  }, [orderNumber, location.state]);

  // Retry Payment Handler
  const handleRetryPayment = async () => {
    try {
      // Try catalogService if orderService doesn't have it (based on useCheckOut)
      const { paymentUrl } = await import("../../services/catalogService").then(
        (m) => m.default.payOrder(orderNumber),
      );
      if (paymentUrl) window.location.href = paymentUrl;
    } catch (error) {
      console.error("Failed to retry payment", error);
    }
  };

  const isFailure =
    orderStatus === "PaymentFailed" || orderStatus === "Cancelled";

  return (
    <div className="page-wrapper success-page">
      <Header />
      <main className="success-content">
        <div className="success-card">
          <div className="icon-container">
            {isFailure ? (
              <svg
                width="80"
                height="80"
                viewBox="0 0 24 24"
                fill="none"
                xmlns="http://www.w3.org/2000/svg">
                <circle
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="#e53935"
                  strokeWidth="1.5"
                />
                <path
                  d="M15 9L9 15"
                  stroke="#e53935"
                  strokeWidth="1.5"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
                <path
                  d="M9 9L15 15"
                  stroke="#e53935"
                  strokeWidth="1.5"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
            ) : (
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
            )}
          </div>

          <h1>{isFailure ? "Payment Failed" : "Thank You!"}</h1>

          <p className="success-message">
            {isFailure
              ? "We could not process your payment. Please try again."
              : "Your order has been placed successfully."}
          </p>

          {orderNumber && (
            <p className="order-number-text">
              Order number: <strong>#{orderNumber}</strong>
            </p>
          )}

          {/* Payment Timer if AwaitingPayment */}
          {orderStatus === "AwaitingPayment" && orderDate && (
            <>
              <PaymentTimer
                createdAt={orderDate}
                onExpire={() => window.location.reload()}
              />
              <button
                className="continue-btn retry-btn"
                onClick={handleRetryPayment}
                style={{
                  marginTop: "1rem",
                  marginBottom: "1rem",
                  backgroundColor: "#4CAF50",
                }}>
                Pay Now
              </button>
            </>
          )}

          {isFailure && (
            <button
              className="continue-btn retry-btn"
              onClick={handleRetryPayment}
              style={{ marginTop: "1rem", backgroundColor: "#e53935" }}>
              Retry Payment
            </button>
          )}

          <button
            className="continue-btn"
            onClick={() => navigate("/")}
            style={{ marginTop: isFailure ? "1rem" : "0" }}>
            Continue Shopping
          </button>
        </div>
      </main>
      <Footer />
    </div>
  );
};

export default OrderSuccess;
