import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import "./PaymentTimer.css";

const PaymentTimer = ({ createdAt, onExpire }) => {
  const PAYMENT_TIMEOUT_MS = 10 * 60 * 1000; // 10 minutes

  const { t } = useTranslation();

  const calculateTimeLeft = () => {
    const createdTime = new Date(createdAt).getTime();
    const expiryTime = createdTime + PAYMENT_TIMEOUT_MS;
    const now = new Date().getTime();
    const difference = expiryTime - now;

    if (difference <= 0) {
      return 0;
    }
    return difference;
  };

  const [timeLeft, setTimeLeft] = useState(calculateTimeLeft());

  useEffect(() => {
    // If initially expired, call onExpire immediately
    if (timeLeft <= 0) {
      if (onExpire) onExpire();
      return;
    }

    const timer = setInterval(() => {
      const remaining = calculateTimeLeft();
      setTimeLeft(remaining);

      if (remaining <= 0) {
        clearInterval(timer);
        if (onExpire) onExpire();
      }
    }, 1000);

    return () => clearInterval(timer);
  }, [createdAt, onExpire]); // Depend on createdAt

  const formatTime = (ms) => {
    if (ms <= 0) return "00:00";
    const totalSeconds = Math.floor(ms / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;
    return `${minutes.toString().padStart(2, "0")}:${seconds
      .toString()
      .padStart(2, "0")}`;
  };

  return (
    <div className="payment-timer">
      <p className="payment-timer-label">{t("checkout.timer_label")}</p>
      <div
        className={`payment-timer-value ${timeLeft < 60000 ? "urgent" : ""}`}>
        {formatTime(timeLeft)}
      </div>
      <p className="payment-timer-hint">{t("checkout.timer_hint")}</p>
    </div>
  );
};

export default PaymentTimer;
