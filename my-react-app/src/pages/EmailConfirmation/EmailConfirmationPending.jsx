import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useTranslation } from "react-i18next";
import authService from "../../services/authService";
import toast from "react-hot-toast";
import { extractErrorMessage } from "../../utils/errorUtils";
import "./EmailConfirmation.css";

export default function EmailConfirmationPending() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const location = useLocation();
  const [countdown, setCountdown] = useState(0);
  const [loading, setLoading] = useState(false);

  // Get email from navigation state (passed from Register)
  const email = location.state?.email || "";

  useEffect(() => {
    let timer;
    if (countdown > 0) {
      timer = setInterval(() => {
        setCountdown((prev) => prev - 1);
      }, 1000);
    }
    return () => clearInterval(timer);
  }, [countdown]);

  const handleResend = async () => {
    if (!email) {
      toast.error(t("toasts.email_not_found"));
      return;
    }

    setLoading(true);
    try {
      await authService.resendConfirmationEmail(email);
      toast.success(t("toasts.confirmation_resent"));
      setCountdown(60);
    } catch (error) {
      console.error("Resend error:", error);
      toast.error(t(extractErrorMessage(error, "toasts.resend_failed")));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="confirmation-page">
      <div className="confirmation-box">
        <h2 className="confirmation-title">{t("auth.registration_success")}</h2>
        <p className="confirmation-text">
          {t("auth.verification_success_msg")}
        </p>

        <div className="resend-section">
          <p className="resend-hint">{t("auth.resend_hint")}</p>
          <button
            className="confirmation-btn resend-btn"
            onClick={handleResend}
            disabled={loading || countdown > 0}>
            {loading
              ? t("auth.sending")
              : countdown > 0
                ? t("auth.resend_in", { count: countdown })
                : t("auth.resend_btn")}
          </button>
        </div>

        <button
          className="confirmation-btn outline"
          onClick={() => navigate("/login")}>
          {t("auth.back_to_login")}
        </button>
      </div>
    </div>
  );
}
