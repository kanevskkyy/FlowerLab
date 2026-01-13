import React from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import toast from "react-hot-toast";
import "./ForgotPassword.css";

// Icons
import logoIcon from "../../assets/icons/logo.svg";
import messageIcon from "../../assets/icons/message.svg";

const schema = z.object({
  email: z.string().min(1, "Email is required").email("Invalid email format"),
});

export default function ForgotPassword() {
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
      console.log("Email sent to:", data.email);
      await new Promise((resolve) => setTimeout(resolve, 1000));
      toast.success("Recovery link sent!");

      setTimeout(() => {
        navigate("/reset-password?token=demo-token-123");
      }, 1500);
    } catch (error) {
      console.error(error);
      toast.error("Something went wrong.");
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
