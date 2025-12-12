// src/pages/Login/Login.jsx
import { useState } from 'react';
import './Login.css';

export default function Login({ onSwitchToRegister, onLoginSuccess }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);

  const handleSignIn = () => {
    if (onLoginSuccess) {
      // поки що просто передаємо email або "name"
      onLoginSuccess(email || 'name');
    }
  };

  return (
    <div className="login-page">
      {/* HEADER з логотипом */}
      <header className="login-header">
        <div className="login-logo-bar">[LOGO]</div>
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
                className="input-base"
                placeholder="youremail@gmail.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
              <span className="input-icon right">✉️</span>
            </div>
          </div>

          {/* Password */}
          <div className="form-field">
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
