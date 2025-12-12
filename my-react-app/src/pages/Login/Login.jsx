// src/pages/Login/Login.jsx
import { useState } from 'react';
import './Login.css';

// SVG-іконки
import logoIcon from '../../assets/icons/logo.svg';
import lockIcon from '../../assets/icons/lock.svg';
import hideIcon from '../../assets/icons/hide.svg';
import showIcon from '../../assets/icons/show.svg';
import messageIcon from '../../assets/icons/message.svg';

export default function Login({ onSwitchToRegister, onLoginSuccess }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);

  const handleSignIn = () => {
    if (onLoginSuccess) {
      onLoginSuccess(email || 'name');
    }
  };

  return (
    <div className="login-page">
      {/* HEADER як у catalog/register, з logo.svg */}
      <header className="header">
        <div className="header-left"></div>

        <div className="logo">
          <img src={logoIcon} alt="FlowerLab" className="logo-img" />
        </div>

        <div className="header-right"></div>
      </header>

      {/* MAIN */}
      <main className="signin-content">
        <div className="signin-box">
          <h2 className="signin-title">Sign in</h2>

          {/* Email */}
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
                <img
                  src={messageIcon}
                  alt="email icon"
                  className="field-icon"
                />
              </span>
            </div>
          </div>

          {/* Password */}
          <div className="form-field">
            <label className="field-label">Password</label>
            <div className="input-row">
              <span className="input-icon left">
                <img
                  src={lockIcon}
                  alt="lock"
                  className="field-icon"
                />
              </span>
              <input
                type={showPassword ? 'text' : 'password'}
                className="input-base with-left-icon with-right-icon"
                placeholder="      ••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              <button
                type="button"
                className="input-icon-btn right"
                onClick={() => setShowPassword(v => !v)}
              >
                <img
                  src={showPassword ? showIcon : hideIcon}
                  alt={showPassword ? 'show password' : 'hide password'}
                  className="field-icon"
                />
              </button>
            </div>
          </div>

          {/* Sign in button */}
          <button
            type="button"
            className="signin-btn"
            onClick={handleSignIn}
          >
            Sign in
          </button>

          {/* Sign up block */}
          <div className="signup-block">
            <p className="signup-text">Don’t have an account yet?</p>
            <button
              type="button"
              className="signup-btn"
              onClick={onSwitchToRegister}
            >
              Sign up
            </button>
          </div>
        </div>
      </main>
    </div>
  );
}
