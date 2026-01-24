import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import authService from "../../services/authService";
import toast from "react-hot-toast";
import "./EmailConfirmation.css";

export default function EmailConfirmationPending() {
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
      toast.error("User email not found. Please try to log in.");
      return;
    }

    setLoading(true);
    try {
      await authService.resendConfirmationEmail(email);
      toast.success("New confirmation link sent! 📧");
      setCountdown(60);
    } catch (error) {
      console.error("Resend error:", error);
      const msg = error.response?.data?.Message || "Failed to resend email.";
      toast.error(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="confirmation-page">
      <div className="confirmation-box">
        <h2 className="confirmation-title">Registration Successful! 🎉</h2>
        <p className="confirmation-text">
          We have sent a confirmation link to{" "}
          <strong>{email || "your email"}</strong>.
          <br />
          Please check your inbox and click the link to activate your account.
        </p>

        <div className="resend-section">
          <p className="resend-hint">Didn't receive the email?</p>
          <button
            className="confirmation-btn resend-btn"
            onClick={handleResend}
            disabled={loading || countdown > 0}>
            {loading
              ? "Sending..."
              : countdown > 0
                ? `Resend in ${countdown}s`
                : "Resend Link"}
          </button>
        </div>

        <button
          className="confirmation-btn outline"
          onClick={() => navigate("/login")}>
          Back to Login
        </button>
      </div>
    </div>
  );
}
