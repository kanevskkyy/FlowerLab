import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import "./Login.css";
import { useAuth } from "../../context/useAuth";

// 1. Додаємо імпорт API клієнта
import axiosClient from "../../api/axiosClient";

// Icons
import logoIcon from "../../assets/icons/logo.svg";
import lockIcon from "../../assets/icons/lock.svg";
import hideIcon from "../../assets/icons/hide.svg";
import showIcon from "../../assets/icons/show.svg";
import messageIcon from "../../assets/icons/message.svg";
import toast from "react-hot-toast";

const schema = z.object({
  email: z.string().min(1, "Email is required").email("Invalid email format"),
  password: z.string().min(1, "Password is required"),
});

export default function Login() {
  const navigate = useNavigate();
  const { setAuth } = useAuth(); // Використовуємо метод для оновлення стану
  const [showPassword, setShowPassword] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }, // Додав isSubmitting
  } = useForm({
    resolver: zodResolver(schema),
    mode: "onBlur",
  });

  // 2. Оновлена логіка входу
  const onSubmit = async (data) => {
    try {
      console.log("Logging in...", data);


      const response = await axiosClient.post("/api/users/auth/login", {
        email: data.email,
        password: data.password,
      });

      // Отримуємо токени з відповіді (твоє API повертає accessToken та refreshToken)
      // Отримуємо токени з відповіді. Враховуємо можливий PascalCase від .NET
      const accessToken = response.data.accessToken || response.data.AccessToken;
      const refreshToken = response.data.refreshToken || response.data.RefreshToken;

      if (accessToken) {
        // Зберігаємо токени
        localStorage.setItem("accessToken", accessToken);
        localStorage.setItem("refreshToken", refreshToken);

        // Оновлюємо стан авторизації в контексті
        setAuth(accessToken);

        toast.success("Welcome back!");
        navigate("/cabinet", { replace: true });
      } else {
          // Якщо токен не прийшов (наприклад, null)
           throw new Error("No access token received from server");
      }
    } catch (error) {
      console.error("Login failed:", error);

      // Обробка помилок від бекенду
      // ExceptionHandlingMiddleware повертає { error: "message", type: "..." }
      const errorMsg =
        error.response?.data?.error ||
        error.response?.data?.Detail ||
        error.response?.data?.message ||
        "Login failed. Check your email or password.";

      toast.error(typeof errorMsg === "string" ? errorMsg : "Login failed.");
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
          <h2 className="login-title">Sign in</h2>

          <form onSubmit={handleSubmit(onSubmit)}>
            {/* Email Field */}
            <div className="form-field full-width">
              <label className="field-label">Email</label>
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
              <label className="field-label">Password</label>
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
              {isSubmitting ? "Signing in..." : "Sign in"}
            </button>

            {/* Forgot Password Link */}
            <button
              type="button"
              className="forgot-password-link"
              onClick={() => navigate("/forgot-password")}>
              Forgot your password?
            </button>

            {/* Updated Footer Section */}
            <div className="login-footer">
              <span>Don't have an account yet?</span>
              <button
                type="button"
                className="signup-outlined-btn"
                onClick={() => navigate("/register")}>
                Sign up
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
}
