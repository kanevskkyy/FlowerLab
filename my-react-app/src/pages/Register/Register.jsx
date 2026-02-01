import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import "./Register.css";

// 1. Імпорт API клієнта
import authService from "../../services/authService";

// SVG-іконки
import logoIcon from "../../assets/icons/logo.svg";
import lockIcon from "../../assets/icons/lock.svg";
import hideIcon from "../../assets/icons/hide.svg";
import showIcon from "../../assets/icons/show.svg";
import messageIcon from "../../assets/icons/message.svg";
import toast from "react-hot-toast";

// Схема валідації
export default function Register() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const schema = z
    .object({
      firstName: z.string().min(1, t("validation.first_name_required")),
      lastName: z.string().min(1, t("validation.last_name_required")),
      phone: z.string().regex(/^\+380\d{9}$/, t("validation.phone_format")),
      email: z
        .string()
        .min(1, t("validation.email_required"))
        .email(t("validation.email_invalid")),
      password: z.string().min(8, t("validation.password_min")),
      confirmPassword: z
        .string()
        .min(1, t("validation.confirm_password_required")),
    })
    .refine((data) => data.password === data.confirmPassword, {
      message: t("validation.passwords_mismatch"),
      path: ["confirmPassword"],
    });

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm({
    resolver: zodResolver(schema),
    mode: "onBlur",
  });

  // 2. Оновлена функція відправки на Бекенд
  const onSubmit = async (data) => {
    try {
      // Формуємо DTO
      const payload = {
        firstName: data.firstName,
        lastName: data.lastName,
        phoneNumber: data.phone,
        email: data.email,
        password: data.password,
        confirmPassword: data.confirmPassword,
      };

      const authService = (await import("../../services/authService")).default;
      await authService.register(payload);

      toast.success(t("toasts.registration_success"));
      navigate("/email-confirmation-pending", { state: { email: data.email } });
    } catch (error) {
      console.error("Registration error:", error);

      let errorMsg = t("toasts.registration_failed");

      if (error.response?.data) {
        const data = error.response.data;
        // Standard DTO/FluentValidation errors object
        if (data.errors) {
          const firstKey = Object.keys(data.errors)[0];
          const firstError = data.errors[firstKey];
          errorMsg = Array.isArray(firstError) ? firstError[0] : firstError;
        } else {
          errorMsg = data.error || data.Message || data.message || errorMsg;
        }
      }

      toast.error(
        typeof errorMsg === "string"
          ? errorMsg
          : t("toasts.registration_failed"),
      );
    }
  };

  return (
    <div className="register-page">
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

      <main className="signup-content">
        <div className="signup-box">
          <h2 className="signup-title">{t("auth.signup")}</h2>

          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="signup-grid">
              {/* First Name */}
              <div className="form-field">
                <label className="field-label">{t("auth.first_name")}</label>
                <input
                  type="text"
                  className={`input-base ${
                    errors.firstName ? "input-error" : ""
                  }`}
                  placeholder="Name"
                  {...register("firstName")}
                />
                {errors.firstName && (
                  <p className="error-text">{errors.firstName.message}</p>
                )}
              </div>

              {/* Last Name */}
              <div className="form-field">
                <label className="field-label">{t("auth.last_name")}</label>
                <input
                  type="text"
                  className={`input-base ${
                    errors.lastName ? "input-error" : ""
                  }`}
                  placeholder="Name"
                  {...register("lastName")}
                />
                {errors.lastName && (
                  <p className="error-text">{errors.lastName.message}</p>
                )}
              </div>

              {/* Phone */}
              <div className="form-field">
                <label className="field-label">{t("auth.phone")}</label>
                <input
                  type="tel"
                  className={`input-base ${errors.phone ? "input-error" : ""}`}
                  placeholder="+380..."
                  {...register("phone")}
                />
                {errors.phone && (
                  <p className="error-text">{errors.phone.message}</p>
                )}
              </div>

              {/* Email */}
              <div className="form-field">
                <label className="field-label">{t("auth.email")}</label>
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
                    <img
                      src={messageIcon}
                      alt="email icon"
                      className="field-icon"
                    />
                  </span>
                </div>
                {errors.email && (
                  <p className="error-text">{errors.email.message}</p>
                )}
              </div>
            </div>

            {/* Password */}
            <div className="form-field full-width">
              <label className="field-label">{t("auth.password")}</label>
              <div className="input-row">
                <span className="input-icon left">
                  <img src={lockIcon} alt="lock" className="field-icon" />
                </span>
                <input
                  type={showPassword ? "text" : "password"}
                  className={`input-base with-left-icon with-right-icon ${
                    errors.password ? "input-error" : ""
                  }`}
                  placeholder="••••••••"
                  {...register("password")}
                />
                <button
                  type="button"
                  className="input-icon-btn right"
                  onClick={() => setShowPassword((v) => !v)}>
                  <img
                    src={showPassword ? showIcon : hideIcon}
                    alt={showPassword ? "show password" : "hide password"}
                    className="field-icon"
                  />
                </button>
              </div>
              {errors.password && (
                <p className="error-text">{errors.password.message}</p>
              )}
            </div>

            {/* Confirm Password */}
            <div className="form-field full-width">
              <label className="field-label">
                {t("auth.confirm_password") || "Confirm password"}
              </label>
              <div className="input-row">
                <span className="input-icon left">
                  <img src={lockIcon} alt="lock" className="field-icon" />
                </span>
                <input
                  type={showConfirmPassword ? "text" : "password"}
                  className={`input-base with-left-icon with-right-icon ${
                    errors.confirmPassword ? "input-error" : ""
                  }`}
                  placeholder="Password"
                  {...register("confirmPassword")}
                />
                <button
                  type="button"
                  className="input-icon-btn right"
                  onClick={() => setShowConfirmPassword((v) => !v)}>
                  <img
                    src={showConfirmPassword ? showIcon : hideIcon}
                    alt={
                      showConfirmPassword ? "show password" : "hide password"
                    }
                    className="field-icon"
                  />
                </button>
              </div>
              {errors.confirmPassword && (
                <p className="error-text">{errors.confirmPassword.message}</p>
              )}
            </div>

            <button
              type="submit"
              className="signup-main-btn"
              disabled={isSubmitting}>
              {isSubmitting ? t("auth.creating_account") : t("auth.signup")}
            </button>

            <button
              type="button"
              className="back-to-login-btn"
              onClick={() => navigate("/login")}>
              {t("auth.back_to_login") || "Back to Sign in"}
            </button>
          </form>
        </div>
      </main>
    </div>
  );
}
