import React, { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import axiosClient from "../../api/axiosClient";
import "./EmailConfirmation.css";

export default function EmailConfirmation() {
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
      alert("Please enter your email to resend confirmation.");
      return;
    }

    setResendLoading(true);
    try {
      await axiosClient.post("/api/users/auth/resend-confirm-email", {
        email: resendEmail,
      });
      alert("A new confirmation email has been sent! 📧");
      setCountdown(60);
    } catch (error) {
      console.error("Resend error:", error);
      alert(error.response?.data?.Message || "Failed to resend email.");
    } finally {
      setResendLoading(false);
    }
  };

  useEffect(() => {
    if (!userId || !token) {
      setStatus("error");
      setMessage("Invalid link parameters.");
      return;
    }

    const confirmEmail = async () => {
      try {
        await axiosClient.get("/api/users/auth/confirm-email", {
          params: { userId, token },
        });

        setStatus("success");
        setMessage("Email confirmed successfully! You can now log in.");
      } catch (error) {
        console.error("Confirmation error:", error);
        setStatus("error");
        setMessage(
          error.response?.data?.Message ||
            error.response?.data?.error ||
            "Failed to confirm email.",
        );
      }
    };

    confirmEmail();
  }, [userId, token]);

  return (
    <div className="confirmation-page">
      <div className="confirmation-box">
        {status === "loading" && (
          <h2 className="confirmation-title">Verifying...</h2>
        )}

        {status === "success" && (
          <>
            <h2 className="confirmation-title">Success! 🎉</h2>
            <p className="confirmation-text">{message}</p>
            <button
              className="confirmation-btn"
              onClick={() => navigate("/login")}>
              Go to Login
            </button>
          </>
        )}

        {status === "error" && (
          <>
            <h2 className="confirmation-title error">Error ⚠️</h2>
            <p className="confirmation-text">{message}</p>

            <div className="resend-section">
              <p className="resend-hint">
                Enter your email to receive a new link:
              </p>
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
                  ? "Sending..."
                  : countdown > 0
                    ? `Resend in ${countdown}s`
                    : "Resend Confirmation Email"}
              </button>
            </div>

            <button
              className="confirmation-btn outline"
              onClick={() => navigate("/login")}>
              Go to Login
            </button>
          </>
        )}
      </div>
    </div>
  );
}
