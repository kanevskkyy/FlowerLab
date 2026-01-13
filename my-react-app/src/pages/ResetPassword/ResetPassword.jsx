import React, { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import toast from "react-hot-toast";
import "./ResetPassword.css";

// Icons
import logoIcon from "../../assets/icons/logo.svg";
import hideIcon from "../../assets/icons/hide.svg";
import showIcon from "../../assets/icons/show.svg";

// Validation schema
const schema = z
  .object({
    password: z.string().min(6, "Password must be at least 6 characters"),
    confirmPassword: z.string().min(1, "Confirm password is required"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

export default function ResetPassword() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  // Token check simulation
  useEffect(() => {
    const token = searchParams.get("token");
    if (!token) {
      // toast.error("Invalid token");
    }
  }, [searchParams]);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm({
    resolver: zodResolver(schema),
    mode: "onBlur",
  });

  const onSubmit = async (data) => {
    try {
      console.log("Setting new password:", data.password);
      toast.success("Password successfully changed!");
      setTimeout(() => navigate("/login"), 2000);
    } catch (error) {
      console.error(error);
      toast.error("Failed to reset password.");
    }
  };

  return (
    <div className="rp-page">
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

      <main className="rp-content">
        <div className="rp-box">
          <h2 className="rp-title">Password recovery</h2>

          <form onSubmit={handleSubmit(onSubmit)}>
            {/* Set New Password */}
            <div className="form-field full-width">
              <label className="field-label">Set New Password</label>
              <div className="input-row">
                <input
                  type={showPassword ? "text" : "password"}
                  className={`input-base with-right-icon ${
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
                    alt="toggle"
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
              <label className="field-label">Confirm Password</label>
              <div className="input-row">
                <input
                  type={showConfirmPassword ? "text" : "password"}
                  className={`input-base with-right-icon ${
                    errors.confirmPassword ? "input-error" : ""
                  }`}
                  placeholder="................"
                  {...register("confirmPassword")}
                />
                <button
                  type="button"
                  className="input-icon-btn right"
                  onClick={() => setShowConfirmPassword((v) => !v)}>
                  <img
                    src={showConfirmPassword ? showIcon : hideIcon}
                    alt="toggle"
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
              className="rp-main-btn"
              disabled={isSubmitting}>
              {isSubmitting ? "Updating..." : "Set New Password"}
            </button>

          </form>
        </div>
      </main>
    </div>
  );
}
