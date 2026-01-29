import React from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import toast from "react-hot-toast";
import "./ForgotPassword.css";
import axiosClient from "../../api/axiosClient";

// Icons
import logoIcon from "../../assets/icons/logo.svg";
import messageIcon from "../../assets/icons/message.svg";

const schema = z.object({
  email: z.string().min(1, "Email is required").email("Invalid email format"),
});

export default function ForgotPassword() {
  const { t } = useTranslation();
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(schema),
    mode: "onBlur",
  });

  const onSubmit = async (data) => {
    try {
      await axiosClient.post("/api/users/auth/forgot-password", data);
      toast.success(t("toasts.reset_link_sent"));

      // Optional: Navigate to home or stay here
      // setTimeout(() => navigate("/"), 2000);
    } catch (error) {
      console.error(error);
      const msg = error.response?.data?.message || t("toasts.error_default");
      toast.error(msg);
    }
  };

  return (
    <div className="fp-page">
      {/* Header */}
      <header className="header">
        <div className="header-left"></div>
        <div className="logo">
          <img
            src={logoIcon}
            alt="FlowerLab"
            className="logo-img"
            onClick={() => navigate("/")}
          />
        </div>
        <div className="header-right"></div>
      </header>

      <main className="fp-content">
        <div className="fp-box">
          <h2 className="fp-title">Password recovery</h2>

          <form onSubmit={handleSubmit(onSubmit)}>
            {/* Input Field */}
            <div className="form-field full-width">
              <label className="field-label">Your Email</label>
              <div className="input-row">
                <input
                  type="email"
                  className={`input-base with-right-icon ${
                    errors.email ? "input-error" : ""
                  }`}
                  placeholder="youremail@gmail.com"
                  {...register("email")}
                />
                <span className="input-icon right">
                  <img src={messageIcon} alt="email" className="field-icon" />
                </span>
              </div>
              {errors.email && (
                <p className="error-text">{errors.email.message}</p>
              )}
            </div>

            {/* Submit Button */}
            <button type="submit" className="fp-main-btn">
              Send code
            </button>

            {/* Description Text (moved below button per design) */}
            <p className="fp-description">
              A password recovery email will be sent to you.
            </p>
          </form>
        </div>
      </main>
    </div>
  );
}
