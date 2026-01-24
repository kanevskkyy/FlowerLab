import { useState } from "react";
import { useNavigate } from "react-router-dom";
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
const schema = z
  .object({
    firstName: z.string().min(1, "First name is required"),
    lastName: z.string().min(1, "Last name is required"),
    phone: z.string().regex(/^\+380\d{9}$/, "Format: +380XXXXXXXXX"),
    email: z.string().min(1, "Email is required").email("Invalid email"),
    password: z.string().min(8, "Min 8 characters"),
    confirmPassword: z.string().min(1, "Confirm password is required"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

export default function Register() {
  const navigate = useNavigate();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

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

      toast.success("Registration successful! Please check your email.");
      navigate("/email-confirmation-pending", { state: { email: data.email } });
    } catch (error) {
      console.error("Registration error:", error);

      let errorMsg = "Registration failed.";

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
        typeof errorMsg === "string" ? errorMsg : "Registration failed.",
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
          <h2 className="signup-title">Registration</h2>

          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="signup-grid">
              {/* First Name */}
              <div className="form-field">
                <label className="field-label">First Name</label>
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
                <label className="field-label">Last Name</label>
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
                <label className="field-label">Phone Number</label>
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
              <label className="field-label">Confirm password</label>
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
              {isSubmitting ? "Signing up..." : "Sign up"}
            </button>

            <button
              type="button"
              className="back-to-login-btn"
              onClick={() => navigate("/login")}>
              Back to Sign in
            </button>
          </form>
        </div>
      </main>
    </div>
  );
}
