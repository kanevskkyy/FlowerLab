// src/pages/Cabinet/Cabinet.jsx
import React, { useState } from "react";
import Header from "../../components/Header/Header";
import PopupMenu from "../popupMenu/PopupMenu";
import "./Cabinet.css";

const Cabinet = ({ userName, onSignOut }) => {
  const [activeTab, setActiveTab] = useState("personal");
  const [menuOpen, setMenuOpen] = useState(false);
  const [formData, setFormData] = useState({
    firstName: userName || "",
    lastName: "",
    phone: "+38 066 002 03 01",
    email: "youremail@gmail.com",
  });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSaveChanges = () => {
    console.log("Saving changes:", formData);
  };

  const handleSignOut = () => {
    console.log("Signing out...");
    if (onSignOut) {
      onSignOut();
    }
  };

  return (
    // 🔥 БІЛЬШЕ НІЯКОГО "page-wrapper" – тільки наш клас
    <div className="cabinet-page">
      {/* Header як є */}
      <Header onMenuOpen={() => setMenuOpen(true)} />

      {/* Popup меню як у каталозі */}
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      {/* Власне кабінет */}
      <div className="cabinet-container">
        {/* Sidebar */}
        <aside className="cabinet-sidebar">
          <nav className="cabinet-nav">
            <button
              className={`nav-item ${
                activeTab === "personal" ? "active" : ""
              }`}
              onClick={() => setActiveTab("personal")}
            >
              <span className="nav-icon">👤</span>
              <span className="nav-text">Personal information</span>
            </button>

            <button
              className={`nav-item ${activeTab === "orders" ? "active" : ""}`}
              onClick={() => setActiveTab("orders")}
            >
              <span className="nav-icon">📋</span>
              <span className="nav-text">My orders</span>
            </button>

            <button
              className={`nav-item ${
                activeTab === "addresses" ? "active" : ""
              }`}
              onClick={() => setActiveTab("addresses")}
            >
              <span className="nav-icon">🏠</span>
              <span className="nav-text">Saved addresses</span>
            </button>
          </nav>

          <button className="sign-out-btn" onClick={handleSignOut}>
            <span className="nav-icon">🚪</span>
            <span className="nav-text">Sign out</span>
          </button>
        </aside>

        {/* Main Content */}
        <main className="cabinet-main">
          <h1 className="cabinet-title">Personal information</h1>

          <form className="cabinet-form">
            {/* Name Fields */}
            <div className="form-row">
              <div className="form-group">
                <label className="form-label">First Name</label>
                <input
                  type="text"
                  name="firstName"
                  className="form-input"
                  placeholder="Name"
                  value={formData.firstName}
                  onChange={handleInputChange}
                />
              </div>

              <div className="form-group">
                <label className="form-label">Last Name</label>
                <input
                  type="text"
                  name="lastName"
                  className="form-input"
                  placeholder="Name"
                  value={formData.lastName}
                  onChange={handleInputChange}
                />
              </div>
            </div>

            {/* Phone Number */}
            <div className="form-group">
              <label className="form-label">Phone Number</label>
              <input
                type="tel"
                name="phone"
                className="form-input phone-input"
                placeholder="+38 066 002 03 01"
                value={formData.phone}
                onChange={handleInputChange}
              />
            </div>

            {/* Account Information Section */}
            <h2 className="section-title">Account information</h2>

            {/* Email and Password Row */}
            <div className="form-row account-row">
              <button type="button" className="account-btn">
                <span className="account-icon">✉️</span>
                <span className="account-text">{formData.email}</span>
                <span className="change-btn">Change</span>
              </button>

              <button type="button" className="account-btn">
                <span className="account-icon">🔒</span>
                <span className="account-text">Password</span>
                <span className="change-btn">Change</span>
              </button>
            </div>

            {/* Delete Account */}
            <button type="button" className="delete-account-btn">
              <span className="account-icon">🗑️</span>
              <span className="account-text">Delete account</span>
            </button>

            {/* Save Button */}
            <button
              type="button"
              className="save-btn"
              onClick={handleSaveChanges}
            >
              Save changes
            </button>
          </form>
        </main>
      </div>
    </div>
  );
};

export default Cabinet;
