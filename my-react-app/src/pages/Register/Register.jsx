import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Register.css";
import { useAuth } from "../../context/useAuth";


// SVG-іконки
import logoIcon from "../../assets/icons/logo.svg";
import lockIcon from "../../assets/icons/lock.svg";
import hideIcon from "../../assets/icons/hide.svg";
import showIcon from "../../assets/icons/show.svg";
import messageIcon from "../../assets/icons/message.svg";

export default function Register() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [phone, setPhone] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const handleSignUp = () => {
    // TODO: тут потім буде API + валідація
    login({
      name: firstName || "name",
      email: email || "youremail@gmail.com",
      phone,
      lastName,
    });

    navigate("/cabinet", { replace: true });
  };

  return (
    <div className="register-page">
      <header className="header">
        <div className="header-left"></div>

        <div className="logo">
          <img src={logoIcon} alt="FlowerLab" className="logo-img" />
        </div>

        <div className="header-right"></div>
      </header>

      <main className="signup-content">
        <div className="signup-box">
          <h2 className="signup-title">Registration</h2>

          <div className="signup-grid">
            <div className="form-field">
              <label className="field-label">First Name</label>
              <input
                type="text"
                className="input-base"
                placeholder="Name"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
              />
            </div>

            <div className="form-field">
              <label className="field-label">Last Name</label>
              <input
                type="text"
                className="input-base"
                placeholder="Name"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
              />
            </div>

            <div className="form-field">
              <label className="field-label">Phone Number</label>
              <input
                type="tel"
                className="input-base"
                placeholder="+380|501591912"
                value={phone}
                onChange={(e) => setPhone(e.target.value)}
              />
            </div>

            <div className="form-field">
              <label className="field-label">Email</label>
              <div className="input-row">
                <input
                  type="email"
                  className="input-base with-right-icon"
                  placeholder="youremail@gmail.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
                <span className="input-icon right">
                  <img src={messageIcon} alt="email icon" className="field-icon" />
                </span>
              </div>
            </div>
          </div>

          <div className="form-field full-width">
            <label className="field-label">Password</label>
            <div className="input-row">
              <span className="input-icon left">
                <img src={lockIcon} alt="lock" className="field-icon" />
              </span>

              <input
                type={showPassword ? "text" : "password"}
                className="input-base with-left-icon with-right-icon"
                placeholder="      ••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />

              <button
                type="button"
                className="input-icon-btn right"
                onClick={() => setShowPassword((v) => !v)}
              >
                <img
                  src={showPassword ? showIcon : hideIcon}
                  alt={showPassword ? "show password" : "hide password"}
                  className="field-icon"
                />
              </button>
            </div>
          </div>

          <div className="form-field full-width">
            <label className="field-label">Confirm password</label>
            <div className="input-row">
              <span className="input-icon left">
                <img src={lockIcon} alt="lock" className="field-icon" />
              </span>

              <input
                type={showConfirmPassword ? "text" : "password"}
                className="input-base with-left-icon with-right-icon"
                placeholder="      Password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
              />

              <button
                type="button"
                className="input-icon-btn right"
                onClick={() => setShowConfirmPassword((v) => !v)}
              >
                <img
                  src={showConfirmPassword ? showIcon : hideIcon}
                  alt={showConfirmPassword ? "show password" : "hide password"}
                  className="field-icon"
                />
              </button>
            </div>
          </div>

          <button type="button" className="signup-main-btn" onClick={handleSignUp}>
            Sign up
          </button>

          <button
            type="button"
            className="back-to-login-btn"
            onClick={() => navigate("/login")}
          >
            Back to Sign in
          </button>
        </div>
      </main>
    </div>
  );
}
