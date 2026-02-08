import React, { useState, useEffect, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import "./Login.css";
import { useAuth } from "../../context/useAuth";

// 1. Додаємо імпорт API клієнта
import authService from "../../services/authService";

// Icons
import logoIcon from "../../assets/icons/logo.svg";
import lockIcon from "../../assets/icons/lock.svg";
import hideIcon from "../../assets/icons/hide.svg";
import showIcon from "../../assets/icons/show.svg";
import messageIcon from "../../assets/icons/message.svg";
import toast from "react-hot-toast";
import { extractErrorMessage } from "../../utils/errorUtils";

export default function Login() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const { setAuth } = useAuth(); // Використовуємо метод для оновлення стану
  const [showPassword, setShowPassword] = useState(false);

  const schema = useMemo(
    () =>
      z.object({
        email: z
          .string()
          .min(1, t("validation.email_required"))
          .email(t("validation.email_invalid")),
        password: z.string().min(1, t("validation.password_required")),
      }),
    [t],
  );

  const {
    register,
    handleSubmit,
    trigger,
    formState: { errors, isSubmitting },
  } = useForm({
    resolver: zodResolver(schema),
    mode: "onBlur",
  });

  // Re-validate when language changes
  useEffect(() => {
    if (Object.keys(errors).length > 0) {
      trigger();
    }
  }, [i18n.language, trigger]);

  // 2. Оновлена логіка входу
  const onSubmit = async (data) => {
    try {
      const loginResponse = await authService.login(data.email, data.password);

      const accessToken =
        loginResponse.accessToken || loginResponse.AccessToken;
      const refreshToken =
        loginResponse.refreshToken || loginResponse.RefreshToken;

      if (accessToken) {
        // Оновлюємо стан авторизації в контексті (токени вже в Cookie)
        setAuth(accessToken);

        toast.success(t("auth.welcome_back"));
        navigate("/cabinet", { replace: true });
      } else {
        // Якщо токен не прийшов (наприклад, null)
        throw new Error("No access token received from server");
      }
    } catch (error) {
      console.error("Login failed:", error);

      // Обробка помилок від бекенду
      // ExceptionHandlingMiddleware повертає { error: "message", type: "..." }
      toast.error(t(extractErrorMessage(error, "auth.login_failed")));
    }
  };

  return (
    <div className="login-page">
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

      <main className="login-content">
        <div className="login-box">
          <h2 className="login-title">{t("auth.login")}</h2>

          <form onSubmit={handleSubmit(onSubmit)}>
            {/* Email Field */}
            <div className="form-field full-width">
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
                  <img src={messageIcon} alt="email" className="field-icon" />
                </span>
              </div>
              {errors.email && (
                <p className="error-text">{errors.email.message}</p>
              )}
            </div>
            {/* Password Field */}
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
                  placeholder="Password"
                  {...register("password")}
                />

                <button
                  type="button"
                  className="input-icon-btn right"
                  onClick={() => setShowPassword((v) => !v)}>
                  <img
                    src={showPassword ? showIcon : hideIcon}
                    alt={showPassword ? "show" : "hide"}
                    className="field-icon"
                  />
                </button>
              </div>
              {errors.password && (
                <p className="error-text">{errors.password.message}</p>
              )}
            </div>
            {/* Main Sign In Button */}
            <button
              type="submit"
              className="login-main-btn"
              disabled={isSubmitting}>
              {isSubmitting ? t("auth.signing_in") : t("auth.login")}
            </button>
            {/* Forgot Password Link */}
            <button
              type="button"
              className="forgot-password-link"
              onClick={() => navigate("/forgot-password")}>
              {t("auth.forgot_password")}
            </button>
            {/* Updated Footer Section */}
            <div className="login-footer">
              <span>{t("auth.no_account")}</span>
              <button
                type="button"
                className="signup-outlined-btn"
                onClick={() => navigate("/register")}>
                {t("auth.signup")}
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
}
