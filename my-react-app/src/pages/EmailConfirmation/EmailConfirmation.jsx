import React, { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import toast from "react-hot-toast";
import axiosClient from "../../api/axiosClient";
import { extractErrorMessage } from "../../utils/errorUtils";
import "./EmailConfirmation.css";

export default function EmailConfirmation() {
  const { t } = useTranslation();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState("loading"); // loading, success, error
  const [message, setMessage] = useState("");

  const userId = searchParams.get("userId");
  const token = searchParams.get("token");

  const [resendLoading, setResendLoading] = useState(false);
  const [countdown, setCountdown] = useState(0);
  const [resendEmail, setResendEmail] = useState("");

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
    if (!resendEmail) {
      toast.error(t("auth.email_required"));
      return;
    }

    setResendLoading(true);
    try {
      await axiosClient.post("/api/users/auth/resend-confirm-email", {
        email: resendEmail,
      });
      toast.success(t("toasts.confirmation_resent"));
      setCountdown(60);
    } catch (error) {
      console.error("Resend error:", error);
      toast.error(t(extractErrorMessage(error, "toasts.resend_failed")));
    } finally {
      setResendLoading(false);
    }
  };

  useEffect(() => {
    if (!userId || !token) {
      setStatus("error");
      setMessage(t("toasts.invalid_token"));
      return;
    }

    const confirmEmail = async () => {
      try {
        await axiosClient.get("/api/users/auth/confirm-email", {
          params: { userId, token },
        });

        setStatus("success");
        setMessage(t("auth.verification_success_msg"));
      } catch (error) {
        console.error("Confirmation error:", error);
        setStatus("error");
        setMessage(
          t(extractErrorMessage(error, "auth.verification_error_default")),
        );
      }
    };

    confirmEmail();
  }, [userId, token, t]);

  return (
    <div className="confirmation-page">
      <div className="confirmation-box">
        {status === "loading" && (
          <h2 className="confirmation-title">{t("auth.verifying")}</h2>
        )}

        {status === "success" && (
          <>
            <h2 className="confirmation-title">
              {t("auth.verification_success")}
            </h2>
            <p className="confirmation-text">{message}</p>
            <button
              className="confirmation-btn"
              onClick={() => navigate("/login")}>
              {t("auth.go_to_login")}
            </button>
          </>
        )}

        {status === "error" && (
          <>
            <h2 className="confirmation-title error">
              {t("auth.verification_error")}
            </h2>
            <p className="confirmation-text">{message}</p>

            <div className="resend-section">
              <p className="resend-hint">{t("auth.resend_hint")}</p>
              <input
                type="email"
                className="resend-input"
                placeholder="youremail@gmail.com"
                value={resendEmail}
                onChange={(e) => setResendEmail(e.target.value)}
              />
              <button
                className="confirmation-btn resend-btn"
                onClick={handleResend}
                disabled={resendLoading || countdown > 0}>
                {resendLoading
                  ? t("auth.sending")
                  : countdown > 0
                    ? t("auth.resend_in", { count: countdown })
                    : t("auth.resend_btn")}
              </button>
            </div>

            <button
              className="confirmation-btn outline"
              onClick={() => navigate("/login")}>
              {t("auth.go_to_login")}
            </button>
          </>
        )}
      </div>
    </div>
  );
}
