import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Cabinet.css";

import Header from "../../components/Header/Header";
import PopupMenu from "../popupMenu/PopupMenu";
import { useAuth } from "../../context/useAuth";

/* ICONS */
import UserInfoIcon from "../../assets/icons/userinfo.svg";
import OrderIcon from "../../assets/icons/orders.svg";
import AddressIcon from "../../assets/icons/address.svg";
import ExitIcon from "../../assets/icons/exit.svg";
import MessageIcon from "../../assets/icons/message.svg";
import LockIcon from "../../assets/icons/lock.svg";
import TrashIcon from "../../assets/icons/trash.svg";

/* IMAGES (orders demo) */
import bouquet1 from "../../assets/images/bouquet1L.jpg";
import bouquet2 from "../../assets/images/bouquet2L.jpg";
import bouquet3 from "../../assets/images/bouquet3L.jpg";
import bouquet4 from "../../assets/images/bouquet3L.jpg";

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

  const [addresses, setAddresses] = useState({
    address1: "Chernivtsi, Street Vyshneva, 32",
    address2: "Chernivtsi, Street Medova, 19",
  });

  const onChange = (key) => (e) =>
    setForm((p) => ({ ...p, [key]: e.target.value }));

  const onAddressChange = (key) => (e) =>
    setAddresses((p) => ({ ...p, [key]: e.target.value }));

  const handleSignOut = () => {
    logout();
    navigate("/login", { replace: true });
  };

  const handleSaveAddresses = () => {
    // тут потім буде запит на бек
    // зараз просто демо:
    console.log("Saved addresses:", addresses);
  };

  /* ===== MOCK ORDERS ===== */
  const orders = [
    {
      id: "№1006061",
      date: "10:06 · 10.25.10.25",
      type: "single",
      status: "being processed",
      total: 1000,
      currency: "₴",
      items: [{ title: "Bouquet Orchids", qty: "1 pc", img: bouquet1 }],
    },
    {
      id: "№1006062",
      date: "10:06 · 10.25.10.25",
      type: "multi",
      status: "completed",
      total: 3000,
      currency: "₴",
      items: [
        { title: "Bouquet 101 Roses", img: bouquet2 },
        { title: "Bouquet Pinky Promise", img: bouquet3 },
        { title: "Bouquet Ranunculus", img: bouquet4 },
      ],
    },
  ];

  return (
    <div className="cabinet-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="cabinet-main">
        <div className="cabinet-shell">
          {/* ===== SIDEBAR ===== */}
          <aside className="cabinet-sidebar">
            <button
              className={`cabinet-nav-item ${
                activeTab === TABS.PERSONAL ? "active" : ""
              }`}
              onClick={() => setActiveTab(TABS.PERSONAL)}
              type="button"
            >
              <img src={UserInfoIcon} className="cabinet-nav-icon" alt="" />
              <span>Personal information</span>
            </button>

            <button
              className={`cabinet-nav-item ${
                activeTab === TABS.ORDERS ? "active" : ""
              }`}
              onClick={() => setActiveTab(TABS.ORDERS)}
              type="button"
            >
              <img src={OrderIcon} className="cabinet-nav-icon" alt="" />
              <span>My orders</span>
            </button>

            <button
              className={`cabinet-nav-item ${
                activeTab === TABS.ADDRESSES ? "active" : ""
              }`}
              onClick={() => setActiveTab(TABS.ADDRESSES)}
              type="button"
            >
              <img src={AddressIcon} className="cabinet-nav-icon" alt="" />
              <span>Saved addresses</span>
            </button>

            <div className="cabinet-sidebar-spacer" />

            <button className="cabinet-signout" onClick={handleSignOut} type="button">
              <img src={ExitIcon} className="cabinet-nav-icon" alt="" />
              <span>Sign out</span>
            </button>
          </aside>

          {/* ===== PANEL ===== */}
          <section className="cabinet-panel">
            {/* ================= PERSONAL ================= */}
            {activeTab === TABS.PERSONAL && (
              <div className="cabinet-panel-inner">
                <h1 className="cabinet-title">Personal information</h1>

                <div className="cabinet-grid-2">
                  <div className="cabinet-field">
                    <label>First Name</label>
                    <input
                      value={form.firstName}
                      onChange={onChange("firstName")}
                      placeholder="Name"
                    />
                  </div>

                  <div className="cabinet-field">
                    <label>Last Name</label>
                    <input
                      value={form.lastName}
                      onChange={onChange("lastName")}
                      placeholder="Name"
                    />
                  </div>
                </div>

                <div className="cabinet-grid-1">
                  <div className="cabinet-field">
                    <label>Phone Number</label>
                    <input
                      value={form.phone}
                      onChange={onChange("phone")}
                      placeholder="+38 066 000 03 01"
                    />
                  </div>
                </div>

                <h2 className="cabinet-subtitle">Account information</h2>

                <div className="cabinet-grid-2">
                  <div className="cabinet-pill">
                    <div className="cabinet-pill-left">
                      <img src={MessageIcon} className="cabinet-pill-icon" alt="" />
                      <span className="cabinet-pill-text">{form.email}</span>
                    </div>
                    <button className="cabinet-pill-btn" type="button">Change</button>
                  </div>

                  <div className="cabinet-pill">
                    <div className="cabinet-pill-left">
                      <img src={LockIcon} className="cabinet-pill-icon" alt="" />
                      <span className="cabinet-pill-text">Password</span>
                    </div>
                    <button className="cabinet-pill-btn" type="button">Change</button>
                  </div>
                </div>

                <div className="cabinet-grid-2 cabinet-grid-single-left">
                  <div className="cabinet-pill danger">
                    <div className="cabinet-pill-left">
                      <img src={TrashIcon} className="cabinet-pill-icon" alt="" />
                      <span className="cabinet-pill-text">Delete account</span>
                    </div>
                  </div>
                </div>

                <button className="cabinet-save" type="button">Save changes</button>
              </div>
            )}

            {/* ================= ORDERS ================= */}
            {activeTab === TABS.ORDERS && (
              <div className="cabinet-panel-inner cabinet-orders">
                <div className="orders-top">
                  <h1 className="cabinet-title">Order history</h1>

                  <div className="orders-sort">
                    <span className="orders-sort-label">SORT BY</span>
                    <select className="orders-sort-select">
                      <option>Date</option>
                      <option>Status</option>
                      <option>Total</option>
                    </select>
                  </div>
                </div>

                <div className="orders-list">
                  {orders.map((order) => (
                    <div key={order.id} className="order-card">
                      <div className="order-meta">
                        <span className="order-meta-id">{order.id}</span>{" "}
                        <span className="order-meta-date">{order.date}</span>
                      </div>

                      {order.type === "single" && (
                        <div className="order-single">
                          <div className="order-single-img">
                            <img src={order.items[0].img} alt="" />
                          </div>

                          <div>
                            <div className="order-single-title">
                              {order.items[0].title}
                            </div>
                            <div className="order-single-qty">{order.items[0].qty}</div>
                          </div>

                          <div className="order-single-right">
                            <div className="order-status">
                              Status:{" "}
                              <span className="order-status-value">{order.status}</span>
                            </div>
                            <div className="order-total">
                              Order Total: {order.total} {order.currency}
                            </div>
                          </div>
                        </div>
                      )}

                      {order.type === "multi" && (
                        <>
                          <div className="order-multi-grid">
                            {order.items.map((item, i) => (
                              <div key={i}>
                                <div className="order-item-img">
                                  <img src={item.img} alt="" />
                                </div>
                                <div className="order-item-title">{item.title}</div>
                                <div className="order-item-bottom">
                                  <span>{order.total / 3} ₴</span>
                                  <span>1 pc</span>
                                </div>
                              </div>
                            ))}
                          </div>

                          <div className="order-multi-footer">
                            <div className="order-total">
                              Order Total: {order.total} {order.currency}
                            </div>
                            <div className="order-status">
                              Status:{" "}
                              <span className="order-status-value">{order.status}</span>
                            </div>
                          </div>
                        </>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* ================= ADDRESSES ================= */}
            {activeTab === TABS.ADDRESSES && (
              <div className="cabinet-panel-inner cabinet-addresses">
                <h1 className="cabinet-title">Saved Addresses</h1>

                <div className="cabinet-grid-1">
                  <div className="cabinet-field">
                    <label>Address</label>
                    <input
                      value={addresses.address1}
                      onChange={onAddressChange("address1")}
                      placeholder="Chernivtsi, Street Vyshneva, 32"
                    />
                  </div>
                </div>

                <div className="cabinet-grid-1">
                  <div className="cabinet-field">
                    <label>Secondary address*</label>
                    <input
                      value={addresses.address2}
                      onChange={onAddressChange("address2")}
                      placeholder="Chernivtsi, Street Medova, 19"
                    />
                  </div>
                </div>

                <button
                  className="cabinet-save cabinet-addresses-save"
                  type="button"
                  onClick={handleSaveAddresses}
                >
                  Save changes
                </button>
              </div>
            )}
          </section>
        </div>
      </main>
    </div>
  );
}
