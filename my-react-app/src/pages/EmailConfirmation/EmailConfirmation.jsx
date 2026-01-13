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
            "Failed to confirm email."
        );
      }
    };

    confirmEmail();
  }, [userId, token]);

  return (
    <div className="confirmation-page">
      <div className="confirmation-box">
        {status === "loading" && <h2 className="confirmation-title">Verifying...</h2>}
        
        {status === "success" && (
            <>
                <h2 className="confirmation-title">Success! 🎉</h2>
                <p className="confirmation-text">{message}</p>
                <button className="confirmation-btn" onClick={() => navigate("/login")}>
                  Go to Login
                </button>
            </>
        )}

        {status === "error" && (
            <>
                <h2 className="confirmation-title error">Error ⚠️</h2>
                <p className="confirmation-text">{message}</p>
                <button className="confirmation-btn" onClick={() => navigate("/login")}>
                  Go to Login
                </button>
            </>
        )}
      </div>
    </div>
  );
}
