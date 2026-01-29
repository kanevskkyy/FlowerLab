import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { useCart } from "../../context/CartContext";
import { useAuth } from "../../context/useAuth";
import orderService from "../../services/orderService";
import "./OrderSuccess.css";

const OrderSuccess = () => {
  const { t } = useTranslation();
  const location = useLocation();
  const navigate = useNavigate();
  const { clearCart } = useCart();
  const { user } = useAuth();
  const { orderNumber: stateOrderNumber } = location.state || {};

  const searchParams = new URLSearchParams(location.search);
  const paramOrderNumber =
    searchParams.get("orderId") || searchParams.get("orderNumber");

  const orderNumber = stateOrderNumber || paramOrderNumber;
  const pendingGuestToken = localStorage.getItem("pendingGuestToken");
  const guestToken =
    location.state?.guestToken ||
    searchParams.get("token") ||
    searchParams.get("guestToken") ||
    pendingGuestToken;

  const [orderDate, setOrderDate] = useState(null);
  const [orderStatus, setOrderStatus] = useState(null);
  const [order, setOrder] = useState(null);
  const [isVerifying, setIsVerifying] = useState(!!paramOrderNumber);
  const [hasFetched, setHasFetched] = useState(false);

  useEffect(() => {
    const pendingOrderId = localStorage.getItem("pendingOrder");

    const isMatchingPending =
      pendingOrderId &&
      orderNumber &&
      pendingOrderId.toString().toLowerCase() ===
        orderNumber.toString().toLowerCase();

    if (stateOrderNumber || isMatchingPending) {
      clearCart();
      localStorage.removeItem("order_selectedGifts");
      localStorage.removeItem("order_isCardAdded");
    }
  }, [clearCart, stateOrderNumber, orderNumber, user]);

  useEffect(() => {
    if (location.state?.mockOrder) {
      setOrderDate(location.state.mockOrder.createdAt);
      setOrderStatus(location.state.mockOrder.status);
      setHasFetched(true);
      return;
    }

    if (!orderNumber) {
      setHasFetched(true);
      return;
    }

    let pollInterval = null;
    let pollCount = 0;
    const maxPolls = 15; // Increased to 45 seconds total (15 * 3s)

    const fetchOrderData = async () => {
      try {
        const fetchedOrder = await orderService.getById(
          orderNumber,
          guestToken,
        );
        if (fetchedOrder) {
          setOrder(fetchedOrder);
          setOrderDate(fetchedOrder.createdAt);
          const rawStatus = fetchedOrder.status?.name || fetchedOrder.status;
          const status =
            typeof rawStatus === "string" ? rawStatus.toLowerCase() : "";

          setOrderStatus(rawStatus);

          const successState = [
            "pending",
            "completed",
            "processing",
            "delivering",
            "wait_accept",
            "sandbox",
          ].includes(status);

          if (
            status === "awaitingpayment" &&
            paramOrderNumber &&
            pollCount < maxPolls
          ) {
            setIsVerifying(true);
            pollCount++;
            pollInterval = setTimeout(fetchOrderData, 3000);
          } else {
            setIsVerifying(false);
            setHasFetched(true);
            if (pollInterval) clearTimeout(pollInterval);
          }
        } else {
          console.warn(`[OrderSuccess] Order not found in API response.`);
          setHasFetched(true);
          setIsVerifying(false);
        }
      } catch (error) {
        console.error("[OrderSuccess] Error during fetch:", error);
        setHasFetched(true);
        setIsVerifying(false);
      }
    };

    fetchOrderData();

    return () => {
      if (pollInterval) clearTimeout(pollInterval);
    };
  }, [orderNumber, paramOrderNumber, guestToken, navigate]);

  const statusNormalized =
    typeof orderStatus === "string" ? orderStatus.toLowerCase() : "";

  const isFailure =
    hasFetched &&
    !isVerifying &&
    (statusNormalized === "paymentfailed" ||
      statusNormalized === "cancelled" ||
      statusNormalized === "failure" ||
      statusNormalized === "error" ||
      (statusNormalized === "awaitingpayment" && paramOrderNumber));

  const isSuccess =
    hasFetched &&
    !isVerifying &&
    !isFailure &&
    ([
      "pending",
      "completed",
      "processing",
      "delivering",
      "wait_accept",
      "sandbox",
    ].includes(statusNormalized) ||
      !paramOrderNumber);

  const handleRetryPayment = () => {
    if (order) {
      navigate("/checkout", {
        state: {
          orderData: {
            id: order.id,
            total: order.totalPrice,
            createdAt: order.createdAt,
            guestToken: order.guestToken,
          },
        },
      });
    }
  };

  return (
    <div className="page-wrapper success-page">
      <Header />
      <main className="success-content">
        <div className="success-card">
          <div className="icon-container">
            {isVerifying ? (
              <div className="status-verify-loader"></div>
            ) : isFailure ? (
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

          <h1>
            {isVerifying
              ? t("order_success.verifying")
              : isFailure
                ? t("order_success.failed_title")
                : isSuccess
                  ? t("order_success.success_title")
                  : t("order_success.processing_title")}
          </h1>

          <p className="success-message">
            {isVerifying
              ? t("order_success.verifying_msg")
              : isFailure
                ? t("order_success.failed_msg")
                : isSuccess
                  ? t("order_success.success_msg")
                  : t("order_success.processing_msg")}
          </p>

          {isSuccess && (
            <p className="email-note">{t("order_success.email_note")}</p>
          )}

          {isFailure && (
            <button
              className="continue-btn retry-btn"
              onClick={handleRetryPayment}
              style={{ marginTop: "1rem", backgroundColor: "#e53935" }}>
              {t("order_success.retry_btn")}
            </button>
          )}

          {!isVerifying && (
            <button
              className="continue-btn"
              onClick={() => navigate("/")}
              style={{ marginTop: isFailure ? "1rem" : "0" }}>
              {t("order_success.continue_shopping")}
            </button>
          )}
        </div>
      </main>
      <Footer />
    </div>
  );
};

export default OrderSuccess;
