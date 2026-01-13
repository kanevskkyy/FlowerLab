import React from "react";
import { useNavigate } from "react-router-dom";
import "./EmailConfirmation.css"; // We will create this CSS file

export default function EmailConfirmationPending() {
  const navigate = useNavigate();

  return (
    <div className="confirmation-page">
      <div className="confirmation-box">
        <h2 className="confirmation-title">Registration Successful! 🎉</h2>
        <p className="confirmation-text">
          We have sent a confirmation link to your email.
          <br />
          Please check your inbox and click the link to activate your account.
        </p>
        <button className="confirmation-btn" onClick={() => navigate("/login")}>
          Back to Login
        </button>
      </div>
    </div>
  );
}
