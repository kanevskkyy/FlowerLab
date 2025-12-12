import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Cabinet.css";

import PopupMenu from "../popupMenu/PopupMenu";
import Header from "../../components/Header/Header";
import { useAuth } from "../../context/useAuth";


const TABS = {
  PERSONAL: "personal",
  ORDERS: "orders",
  ADDRESSES: "addresses",
};

export default function Cabinet() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const [menuOpen, setMenuOpen] = useState(false);
  const [activeTab, setActiveTab] = useState(TABS.PERSONAL);

  const [form, setForm] = useState({
    firstName: user?.name || "",
    lastName: user?.lastName || "",
    phone: user?.phone || "",
    email: user?.email || "youremail@gmail.com",
  });

  const onChange = (key) => (e) =>
    setForm((p) => ({ ...p, [key]: e.target.value }));

  const handleSignOut = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <div className="cabinet-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="cabinet-main">
        <div className="cabinet-shell">
          <aside className="cabinet-sidebar">
            <button
              className={`cabinet-nav-item ${activeTab === TABS.PERSONAL ? "active" : ""}`}
              onClick={() => setActiveTab(TABS.PERSONAL)}
              type="button"
            >
              <span className="cabinet-nav-icon">👤</span>
              <span>Personal information</span>
            </button>

            <button
              className={`cabinet-nav-item ${activeTab === TABS.ORDERS ? "active" : ""}`}
              onClick={() => setActiveTab(TABS.ORDERS)}
              type="button"
            >
              <span className="cabinet-nav-icon">🧾</span>
              <span>My orders</span>
            </button>

            <button
              className={`cabinet-nav-item ${activeTab === TABS.ADDRESSES ? "active" : ""}`}
              onClick={() => setActiveTab(TABS.ADDRESSES)}
              type="button"
            >
              <span className="cabinet-nav-icon">🏠</span>
              <span>Saved addresses</span>
            </button>

            <div className="cabinet-sidebar-spacer" />

            <button className="cabinet-signout" type="button" onClick={handleSignOut}>
              <span className="cabinet-nav-icon">↩</span>
              <span>SignOut</span>
            </button>
          </aside>

          <section className="cabinet-panel">
            {activeTab === TABS.PERSONAL && (
              <div className="cabinet-panel-inner">
                <h1 className="cabinet-title">Personal information</h1>

                <div className="cabinet-grid-2">
                  <div className="cabinet-field">
                    <label>First Name</label>
                    <input value={form.firstName} onChange={onChange("firstName")} placeholder="Name" />
                  </div>

                  <div className="cabinet-field">
                    <label>Last Name</label>
                    <input value={form.lastName} onChange={onChange("lastName")} placeholder="Name" />
                  </div>
                </div>

                <div className="cabinet-grid-1">
                  <div className="cabinet-field">
                    <label>Phone Number</label>
                    <input value={form.phone} onChange={onChange("phone")} placeholder="+38 066 000 03 01" />
                  </div>
                </div>

                <h2 className="cabinet-subtitle">Account information</h2>

                <div className="cabinet-grid-2">
                  <div className="cabinet-pill">
                    <div className="cabinet-pill-left">
                      <span className="cabinet-pill-icon">✉️</span>
                      <span className="cabinet-pill-text">{form.email}</span>
                    </div>
                    <button className="cabinet-pill-btn" type="button">
                      Change
                    </button>
                  </div>

                  <div className="cabinet-pill">
                    <div className="cabinet-pill-left">
                      <span className="cabinet-pill-icon">🔒</span>
                      <span className="cabinet-pill-text">Password</span>
                    </div>
                    <button className="cabinet-pill-btn" type="button">
                      Change
                    </button>
                  </div>
                </div>

                <div className="cabinet-grid-2 cabinet-grid-single-left">
                  <div className="cabinet-pill danger">
                    <div className="cabinet-pill-left">
                      <span className="cabinet-pill-icon">🗑️</span>
                      <span className="cabinet-pill-text">Delete account</span>
                    </div>
                  </div>
                </div>

                <button className="cabinet-save" type="button">
                  Save changes
                </button>
              </div>
            )}

            {activeTab === TABS.ORDERS && (
              <div className="cabinet-panel-inner">
                <h1 className="cabinet-title">My orders</h1>
                <div className="cabinet-placeholder">Orders list will be here</div>
              </div>
            )}

            {activeTab === TABS.ADDRESSES && (
              <div className="cabinet-panel-inner">
                <h1 className="cabinet-title">Saved addresses</h1>
                <div className="cabinet-placeholder">Addresses will be here</div>
              </div>
            )}
          </section>
        </div>
      </main>

    </div>
  );
}
