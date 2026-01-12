import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import "./Login.css";
import { useAuth } from "../../context/useAuth";

// Icons
import logoIcon from "../../assets/icons/logo.svg";
import lockIcon from "../../assets/icons/lock.svg";
import hideIcon from "../../assets/icons/hide.svg";
import showIcon from "../../assets/icons/show.svg";
import messageIcon from "../../assets/icons/message.svg";
import toast from "react-hot-toast";

// Схема валідації
const schema = z.object({
  email: z.string().min(1, "Email is required").email("Invalid email format"),
  password: z.string().min(1, "Password is required"),
});

export default function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [showPassword, setShowPassword] = useState(false);

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
      console.log("Logging in...", data);
      await login(data); // Викликаємо функцію з контексту
      toast.success("Welcome back!");
      navigate("/cabinet", { replace: true });
    } catch (error) {
      console.error("Login failed:", error);
      toast.error("Login failed. Check your email or password.");
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
          <h2 className="login-title">SIGN IN</h2>

          <form onSubmit={handleSubmit(onSubmit)}>
            {/* Email */}
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

            {/* Submit Button */}
            <button type="submit" className="login-main-btn">
              Sign in
            </button>

            {/* Link to Register */}
            <div className="login-footer">
              <span>Don't have an account?</span>
              <button
                type="button"
                className="back-to-register-btn"
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
