// src/pages/Cabinet/Cabinet.jsx
import { useState } from 'react';
import './Cabinet.css';

const TABS = {
  PERSONAL: 'personal',
  ORDERS: 'orders',
  ADDRESSES: 'addresses',
};

export default function Cabinet({ userName = 'name', onSignOut }) {
  const [activeTab, setActiveTab] = useState(TABS.PERSONAL);
  const [isSidebarOpen, setIsSidebarOpen] = useState(false); // NEW

  const handleTabClick = (tab) => {
    setActiveTab(tab);
    setIsSidebarOpen(false); // щоб на мобілі меню закривалось після вибору
  };

  const handleSignOutClick = () => {
    setIsSidebarOpen(false);
    onSignOut && onSignOut();
  };

  const toggleSidebar = () => {
    setIsSidebarOpen((prev) => !prev);
  };

  const renderContent = () => {
    switch (activeTab) {
      case TABS.PERSONAL:
        return (
          <div className="cabinet-section">
            <h2 className="cabinet-title">Personal information</h2>

            <div className="two-cols">
              <div className="form-field">
                <label>First Name</label>
                <input type="text" placeholder="Name" />
              </div>
              <div className="form-field">
                <label>Last Name</label>
                <input type="text" placeholder="Name" />
              </div>
            </div>

            <div className="one-col">
              <div className="form-field">
                <label>Phone Number</label>
                <input type="tel" placeholder="+38 050 159 19 12" />
              </div>
            </div>

            <h3 className="block-subtitle">Account information</h3>

            <div className="two-cols">
              <div className="form-field with-action">
                <label>Email</label>
                <div className="input-row">
                  <span className="input-icon left">✉️</span>
                  <input
                    type="email"
                    placeholder="youremail@gmail.com"
                    className="with-left-icon"
                  />
                  <button type="button" className="small-action">
                    Change
                  </button>
                </div>
              </div>

              <div className="form-field with-action">
                <label>Password</label>
                <div className="input-row">
                  <span className="input-icon left">🔒</span>
                  <input
                    type="password"
                    placeholder="Password"
                    className="with-left-icon"
                  />
                  <button type="button" className="small-action">
                    Change
                  </button>
                </div>
              </div>
            </div>

            <div className="one-col">
              <div className="form-field">
                <div className="input-row">
                  <span className="input-icon left">🗑️</span>
                  <button type="button" className="delete-account-btn">
                    Delete account
                  </button>
                </div>
              </div>
            </div>

            <button type="button" className="primary-btn">
              Save changes
            </button>
          </div>
        );

      case TABS.ORDERS:
  return (
    <div className="cabinet-section orders-section">
      <h2 className="cabinet-title">Orders history</h2>

      {/* Order card 1 */}
      <div className="order-card">
        <div className="order-left">
          <div className="order-image large" />
        </div>

        <div className="order-middle">
          <div>
            <div className="order-title">Bouquet name</div>
            <div className="order-qty">1 pc</div>
          </div>
          <div className="order-meta">
            №1006061&nbsp;&nbsp; at 10:06:10 25.10.25
          </div>
        </div>

        <div className="order-right">
          <div className="order-total-label">Order Total:</div>
          <div className="order-total-value">1000 ₴</div>
        </div>
      </div>

      {/* Order card 2 */}
      <div className="order-card">
        <div className="order-left multi">
          <div className="order-image small" />
          <div className="order-image small" />
          <div className="order-image small" />
        </div>

        <div className="order-middle">
          <div className="order-meta top">
            №1006061&nbsp;&nbsp; at 10:06:10 25.10.25
          </div>

          <div className="order-products-list">
            <div className="product-row">
              <span>Bouquet name</span>
              <span>1 pc</span>
              <span>1000 ₴</span>
            </div>
            <div className="product-row">
              <span>Bouquet name</span>
              <span>1 pc</span>
              <span>1000 ₴</span>
            </div>
            <div className="product-row">
              <span>Bouquet name</span>
              <span>1 pc</span>
              <span>1000 ₴</span>
            </div>
          </div>
        </div>

        <div className="order-right">
          <div className="order-total-label">Order Total:</div>
          <div className="order-total-value">3000 ₴</div>
        </div>
      </div>
    </div>
  );

      case TABS.ADDRESSES:
        return (
          <div className="cabinet-section">
            <h2 className="cabinet-title">Saved Addresses</h2>

            <div className="one-col">
              <div className="form-field">
                <label>Address</label>
                <input
                  type="text"
                  placeholder="Chernivtsi, Street Vyshneva, 32"
                />
              </div>
            </div>

            <div className="one-col">
              <div className="form-field">
                <label>Secondary address*</label>
                <input
                  type="text"
                  placeholder="Chernivtsi, Street Medova, 19"
                />
              </div>
            </div>

            <button type="button" className="primary-btn">
              Save changes
            </button>
          </div>
        );

      default:
        return null;
    }
  };

  return (
    <div className="cabinet-page">
      {/* HEADER */}
      <header className="header">
        <div className="header-left">
          <button className="burger" onClick={toggleSidebar}>
            <span></span>
            <span></span>
            <span></span>
          </button>
          <span className="header-text">UA/ENG</span>
        </div>

        <div className="header-center">[LOGO]</div>

        <div className="header-right">
          <span className="header-text">UAH/USD</span>
          <span className="icon">🛍</span>
          <div className="profile">
            <span className="icon">👤</span>
            <span className="profile-name">{userName}</span>
          </div>
        </div>
      </header>

      {/* MAIN LAYOUT: sidebar + content */}
      <main className="cabinet-main">
        <aside
          className={`cabinet-sidebar ${isSidebarOpen ? 'open' : ''}`}
        >
          <button
            type="button"
            className={`sidebar-item ${
              activeTab === TABS.PERSONAL ? 'active' : ''
            }`}
            onClick={() => handleTabClick(TABS.PERSONAL)}
          >
            <span className="sidebar-icon">👥</span>
            <span>Personal information</span>
          </button>

          <button
            type="button"
            className={`sidebar-item ${
              activeTab === TABS.ORDERS ? 'active' : ''
            }`}
            onClick={() => handleTabClick(TABS.ORDERS)}
          >
            <span className="sidebar-icon">📄</span>
            <span>My orders</span>
          </button>

          <button
            type="button"
            className={`sidebar-item ${
              activeTab === TABS.ADDRESSES ? 'active' : ''
            }`}
            onClick={() => handleTabClick(TABS.ADDRESSES)}
          >
            <span className="sidebar-icon">🏠</span>
            <span>Saved addresses</span>
          </button>

          <div className="sidebar-bottom">
            <button
              type="button"
              className="signout-btn"
              onClick={handleSignOutClick}
            >
              <span className="sidebar-icon">↩️</span>
              <span>Sign out</span>
            </button>
          </div>
        </aside>

        <section className="cabinet-content">{renderContent()}</section>
      </main>

      {/* FOOTER */}
      <footer className="footer">
        <div className="footer-item">
          <span className="icon">📍</span>
          <span>м. Чернівці, вул. Герцена 2а</span>
        </div>
        <div className="footer-item">
          <span className="icon">📞</span>
          <span>+38 050 159 19 12</span>
        </div>
        <div className="footer-item">
          <span className="icon">📷</span>
          <span>@flowerlab_vlada</span>
        </div>
      </footer>
    </div>
  );
}
