import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-hot-toast";
import axiosClient from "../../api/axiosClient";
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
  const { user, login, logout } = useAuth();

  const [menuOpen, setMenuOpen] = useState(false);
  const [activeTab, setActiveTab] = useState(TABS.PERSONAL);

  const [form, setForm] = useState({
    firstName: user?.name || "",
    lastName: user?.lastName || "",
    phone: user?.phone || "",
    email: user?.email || "youremail@gmail.com",
    photoUrl: user?.photoUrl || "",
  });

  /* ===== AVATAR STATE ===== */
  const [selectedFile, setSelectedFile] = useState(null);
  const [photoPreview, setPhotoPreview] = useState(user?.photoUrl || "");


  /* ===== PASSWORD CHANGE STATE ===== */
  const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
  const [passwordForm, setPasswordForm] = useState({
    oldPassword: "",
    newPassword: "",
    confirmPassword: "",
  });

  /* ... */

  const onPasswordChange = (key) => (e) =>
    setPasswordForm((p) => ({ ...p, [key]: e.target.value }));


  /* ===== ADDRESSES STATE ===== */
  const [addressList, setAddressList] = useState([]);
  const [newAddress, setNewAddress] = useState("");
  
  /* ===== ORDERS STATE ===== */
  const [orders, setOrders] = useState([]);
  const [ordersLoading, setOrdersLoading] = useState(false);

  /* ... form handlers ... */
  const onChange = (key) => (e) =>
    setForm((p) => ({ ...p, [key]: e.target.value }));

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setSelectedFile(file);
      setPhotoPreview(URL.createObjectURL(file));
    }
  };


  /* ... address handlers ... */
  const fetchAddresses = async () => {
    try {
      const { data } = await axiosClient.get("/api/users/me/addresses");
      setAddressList(data);
    } catch (error) {
      console.error("Failed to fetch addresses:", error);
    }
  };

  const fetchOrders = async () => {
    setOrdersLoading(true);
    try {
      const { data } = await axiosClient.get("/api/orders/my?pageNumber=1&pageSize=100");
      
      const mappedOrders = data.items.map(order => ({
        id: `№${order.id.substring(0, 8).toUpperCase()}`,
        // Format date: "10:06 · 10.25.2023"
        date: new Date(order.createdAt).toLocaleString('uk-UA', { 
           hour: '2-digit', minute: '2-digit', day: 'numeric', month: 'numeric', year: 'numeric' 
        }), 
        type: order.items.length > 1 ? "multi" : "single",
        status: order.status.name, // Assuming status object has name
        total: order.totalPrice,
        currency: "₴",
        items: order.items.map(item => ({
           title: item.bouquetName,
           qty: `${item.count} pc`,
           img: item.bouquetImage,
           price: item.price
        }))
      }));
      setOrders(mappedOrders);
    } catch (error) {
      console.error("Failed to fetch orders:", error);
      // toast.error("Failed to load orders"); // Optional
    } finally {
      setOrdersLoading(false);
    }
  };

  useEffect(() => {
    if (activeTab === TABS.ADDRESSES) fetchAddresses();
    if (activeTab === TABS.ORDERS) fetchOrders();
  }, [activeTab]);

  /* ... handlers ... */
  const handleSignOut = () => {
    logout();
    navigate("/login", { replace: true });
  };

  const handleProfileUpdate = async () => {
    try {
      const formData = new FormData();
      formData.append("FirstName", form.firstName);
      formData.append("LastName", form.lastName);
      
      // Sanitize phone: remove spaces and dashes for the backend
      const cleanPhone = form.phone.replace(/[\s-]/g, "");
      formData.append("PhoneNumber", cleanPhone);
      
      if (selectedFile) {
        formData.append("Photo", selectedFile);
      }

      const response = await axiosClient.put("/api/users/me", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      // Refresh user data in AuthContext with NEW token
      const newToken = response.data.token || response.data.accessToken;
      if (newToken) {
        await login(newToken);
      }

      toast.success("Profile updated successfully! ✨");
    } catch (error) {
      console.error("Failed to update profile:", error);
      
      // Handle detailed validation errors from backend
      const responseData = error.response?.data;
      if (responseData?.errors) {
        // If errors is an array (our middleware format)
        if (Array.isArray(responseData.errors)) {
          const firstErr = responseData.errors[0];
          toast.error(firstErr?.error || firstErr?.Error || "Validation error");
        } 
        // If errors is an object (ASP.NET default format)
        else if (typeof responseData.errors === 'object') {
          const firstKey = Object.keys(responseData.errors)[0];
          const firstMsg = responseData.errors[firstKey][0];
          toast.error(`${firstKey}: ${firstMsg}`);
        }
      } else {
        toast.error(responseData?.error || responseData?.message || "Failed to update profile");
      }
    }
  };

  const handlePasswordChange = async (e) => {
    e.preventDefault();
    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      toast.error("New passwords do not match!");
      return;
    }

    try {
      await axiosClient.put("/api/users/me/password", {
        oldPassword: passwordForm.oldPassword,
        newPassword: passwordForm.newPassword,
        confirmPassword: passwordForm.confirmPassword,
      });

      toast.success("Password changed successfully! 🔐");
      setIsPasswordModalOpen(false);
      setPasswordForm({ oldPassword: "", newPassword: "", confirmPassword: "" });
    } catch (error) {
      console.error("Failed to change password:", error);
      toast.error(error.response?.data?.message || "Failed to change password");
    }
  };

  const handleSaveAddress = async () => {
    if (!newAddress.trim()) return;
    try {
      await axiosClient.post("/api/users/me/addresses", {
        address: newAddress,
        isDefault: false, // Defaulting to false for now
      });
      setNewAddress("");
      toast.success("Address added successfully!");
      fetchAddresses();
    } catch (error) {
      console.error("Failed to save address:", error);
      toast.error("Failed to save address.");
    }
  };

  const handleDeleteAddress = async (id) => {
    if (!window.confirm("Are you sure you want to delete this address?")) return;
    try {
      await axiosClient.delete(`/api/users/me/addresses/${id}`);
      toast.success("Address deleted!");
      fetchAddresses();
    } catch (error) {
      console.error("Failed to delete address:", error);
      toast.error("Failed to delete address.");
    }
  };

  /* ... existing orders code ... */

  /* ===== RENDER ===== */
  return (
    <div className="cabinet-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="cabinet-main">
        <div className="cabinet-shell">
          {/* ... sidebar ... */}
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

                <div className="cabinet-avatar-section">
                  <div className="cabinet-avatar-wrapper" onClick={() => document.getElementById("avatar-input").click()}>
                    {photoPreview ? (
                      <img src={photoPreview} alt="Avatar" className="cabinet-avatar-img" />
                    ) : (
                      <div className="cabinet-avatar-placeholder">
                        {form.firstName.charAt(0)}{form.lastName.charAt(0)}
                      </div>
                    )}
                    <div className="cabinet-avatar-overlay">
                      <span>Change Photo</span>
                    </div>
                  </div>
                  <input
                    id="avatar-input"
                    type="file"
                    accept="image/*"
                    onChange={handleFileChange}
                    style={{ display: "none" }}
                  />
                  <div className="cabinet-avatar-info">
                    <h3>Profile Photo</h3>
                    <p>Click to update your avatar</p>
                  </div>
                </div>

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
                    <button 
                      className="cabinet-pill-btn" 
                      type="button"
                      onClick={() => setIsPasswordModalOpen(true)}
                    >
                      Change
                    </button>
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

                <button 
                  className="cabinet-save" 
                  type="button"
                  onClick={handleProfileUpdate}
                >
                  Save changes
                </button>
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
                  {!ordersLoading && orders.length === 0 && (
                    <div className="orders-empty">
                       <img src={OrderIcon} className="orders-empty-icon" alt="" />
                       <span>You haven't placed any orders yet.</span>
                    </div>
                  )}

                  {orders.map((order) => (
                    <div key={order.id} className="order-card">
                      <div className="order-meta">
                        <span className="order-meta-id">{order.id}</span>{" "}
                        <span className="order-meta-date">{order.date}</span>
                      </div>

                      {order.type === "single" && order.items?.[0] && (
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
                            {order.items?.map((item, i) => (
                              <div key={i}>
                                <div className="order-item-img">
                                  {item?.img && <img src={item.img} alt="" />}
                                </div>
                                <div className="order-item-title">{item?.title}</div>
                                <div className="order-item-bottom">
                                  <span>{item?.price} ₴</span>
                                  <span>{item?.qty}</span>
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
                 {/* List Existing Addresses */}
                 {addressList.map((addr) => (
                    <div key={addr.id} className="cabinet-pill" style={{ marginBottom: "1rem" }}>
                      <div className="cabinet-pill-left">
                        <img src={AddressIcon} className="cabinet-pill-icon" alt="" />
                        <span className="cabinet-pill-text">{addr.address}</span>
                      </div>
                      <button 
                        className="cabinet-pill-btn cabinet-delete-btn" 
                        type="button"
                        onClick={() => handleDeleteAddress(addr.id)}
                        title="Delete address"
                      >
                         <img src={TrashIcon} alt="Delete" style={{width: 20, height: 20}}/>
                      </button>
                    </div>
                  ))}
                </div>

                <div className="cabinet-grid-1" style={{ marginTop: "2rem" }}>
                  <div className="cabinet-field">
                    <label>Add new address</label>
                    <input
                      value={newAddress}
                      onChange={(e) => setNewAddress(e.target.value)}
                      placeholder="Enter new address..."
                    />
                  </div>
                </div>

                <button
                  className="cabinet-save cabinet-addresses-save"
                  type="button"
                  onClick={handleSaveAddress}
                  disabled={!newAddress.trim()}
                >
                  Add Address
                </button>
              </div>
            )}
          </section>
        </div>
      </main>

      {/* ===== PASSWORD MODAL ===== */}
      {isPasswordModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content cabinet-password-modal">
            <h2 className="modal-title">Change Password</h2>
            <form onSubmit={handlePasswordChange}>
              <div className="cabinet-field">
                <label>Old Password</label>
                <input
                  type="password"
                  value={passwordForm.oldPassword}
                  onChange={onPasswordChange("oldPassword")}
                  placeholder="********"
                  required
                />
              </div>

              <div className="cabinet-field">
                <label>New Password</label>
                <input
                  type="password"
                  value={passwordForm.newPassword}
                  onChange={onPasswordChange("newPassword")}
                  placeholder="********"
                  required
                />
              </div>

              <div className="cabinet-field">
                <label>Confirm New Password</label>
                <input
                  type="password"
                  value={passwordForm.confirmPassword}
                  onChange={onPasswordChange("confirmPassword")}
                  placeholder="********"
                  required
                />
              </div>

              <div className="modal-actions">
                <button 
                  className="modal-btn-cancel" 
                  type="button" 
                  onClick={() => setIsPasswordModalOpen(false)}
                >
                  Cancel
                </button>
                <button className="modal-btn-save" type="submit">
                  Update Password
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
