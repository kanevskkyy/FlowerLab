// src/pages/Register/Register.jsx
import { useState } from 'react';
import './Register.css';

export default function Register({ onSwitchToLogin, onRegisterSuccess }) {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName]   = useState('');
  const [phone, setPhone]         = useState('');
  const [email, setEmail]         = useState('');
  const [password, setPassword]   = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [showPassword, setShowPassword]             = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const handleSignUp = () => {
    // сюди потім додаси валідацію + запит на бекенд
    if (onRegisterSuccess) {
      onRegisterSuccess(firstName || 'name');
    }
  };

  return (
    <div className="register-page">
      {/* HEADER з логотипом, як на макеті */}
      <header className="login-header">
        <div className="login-logo-bar">[LOGO]</div>
      </header>

      {/* MAIN */}
      <main className="signup-content">
        <div className="signup-box">
          <h2 className="signup-title">Registration</h2>

          {/* 2 колонки: First / Last name, Phone / Email */}
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
                  className="input-base"
                  placeholder="youremail@gmail.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
                <span className="input-icon right">✉️</span>
              </div>
            </div>
          </div>

          {/* Password */}
          <div className="form-field full-width">
            <label className="field-label">Password</label>
            <div className="input-row">
              <span className="input-icon left">🔒</span>
              <input
                type={showPassword ? 'text' : 'password'}
                className="input-base with-left-icon"
                placeholder="••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              <button
                type="button"
                className="input-icon-btn right"
                onClick={() => setShowPassword((v) => !v)}
              >
                {showPassword ? '🙈' : '👁️'}
              </button>
            </div>
          </div>

          {/* Confirm password */}
          <div className="form-field full-width">
            <label className="field-label">Confirm password</label>
            <div className="input-row">
              <span className="input-icon left">🔒</span>
              <input
                type={showConfirmPassword ? 'text' : 'password'}
                className="input-base with-left-icon"
                placeholder="Password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
              />
              <button
                type="button"
                className="input-icon-btn right"
                onClick={() => setShowConfirmPassword((v) => !v)}
              >
                {showConfirmPassword ? '🙈' : '👁️'}
              </button>
            </div>
          </div>

          {/* Кнопка Sign up */}
          <button
            type="button"
            className="signup-main-btn"
            onClick={handleSignUp}
          >
            Sign up
          </button>

          {/* Опціонально: повернення на Sign in */}
          {onSwitchToLogin && (
            <button
              type="button"
              className="back-to-login-btn"
              onClick={onSwitchToLogin}
            >
              Back to Sign in
            </button>
          )}
        </div>
      </main>
    </div>
  );
}
