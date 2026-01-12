import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import "./AdminOrderDetails.css";

// Імпорт заглушок зображень (можна використовувати ті ж самі)
import testphoto from "../../assets/images/testphoto.jpg";
import gift1 from "../../assets/images/gift1.jpg";

// Демо-дані замовлення (імітація бекенду)
const MOCK_ORDER = {
  id: "1001",
  date: "25.10.2025 10:06",
  status: "New",
  customer: {
    firstName: "Oleh",
    lastName: "Vynnyk",
    phone: "+38 066 123 45 67",
    email: "oleh@example.com"
  },
  delivery: {
    method: "Courier Delivery", // або "Pickup"
    address: "Chernivtsi, Holovna str. 446, apt. 12",
    comment: "Please call 10 mins before arrival. Door code is 1234."
  },
  items: [
    {
      id: 1,
      title: "Bouquet Orchids",
      price: 1000,
      qty: 1,
      size: "M",
      img: testphoto
    }
  ],
  gifts: [
    {
      id: 101,
      title: "Teddy Bear",
      price: 350,
      img: gift1
    }
  ],
  hasCard: true, // Листівка
  totals: {
    subtotal: 1350,
    delivery: 100,
    discount: 135,
    total: 1365
  }
};

const ORDER_STATUSES = ["New", "Processing", "Shipped", "Delivered", "Cancelled"];

export default function AdminOrderDetails() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [order, setOrder] = useState(null);

  useEffect(() => {
    // Імітація завантаження даних
    setTimeout(() => {
      setOrder({ ...MOCK_ORDER, id }); // Використовуємо ID з URL
    }, 100);
  }, [id]);

  const handleStatusChange = (newStatus) => {
    setOrder(prev => ({ ...prev, status: newStatus }));
    // Тут мав би бути запит на сервер для оновлення статусу
  };

  if (!order) return <div className="aod-loading">Loading order details...</div>;

  return (
    <div className="aod-page">
      <div className="aod-container">
        {/* HEADER */}
        <header className="aod-header">
          <div className="aod-header-left">
            <button className="aod-back-btn" onClick={() => navigate("/admin")}>
              ← Back to Orders
            </button>
            <h1 className="aod-title">Order #{order.id}</h1>
            <span className="aod-date">{order.date}</span>
          </div>
          
          <div className="aod-status-wrapper">
            <label>Status:</label>
            <select 
              className={`aod-status-select status-${order.status.toLowerCase()}`}
              value={order.status}
              onChange={(e) => handleStatusChange(e.target.value)}
            >
              {ORDER_STATUSES.map(s => (
                <option key={s} value={s}>{s}</option>
              ))}
            </select>
          </div>
        </header>

        <div className="aod-content">
          {/* LEFT COLUMN: ITEMS */}
          <div className="aod-main">
            
            {/* Products */}
            <section className="aod-card">
              <h2 className="aod-section-title">Order Items</h2>
              <div className="aod-items-list">
                {order.items.map((item, idx) => (
                  <div key={idx} className="aod-item">
                    <img src={item.img} alt={item.title} className="aod-item-img" />
                    <div className="aod-item-info">
                      <div className="aod-item-name">{item.title}</div>
                      <div className="aod-item-meta">Size: {item.size}</div>
                    </div>
                    <div className="aod-item-qty">{item.qty} pc</div>
                    <div className="aod-item-price">{item.price} ₴</div>
                  </div>
                ))}
              </div>
            </section>

            {/* Gifts & Extras */}
            {(order.gifts.length > 0 || order.hasCard) && (
              <section className="aod-card">
                <h2 className="aod-section-title">Gifts & Extras</h2>
                <div className="aod-items-list">
                  {order.gifts.map((gift) => (
                    <div key={gift.id} className="aod-item">
                      <img src={gift.img} alt={gift.title} className="aod-item-img" />
                      <div className="aod-item-info">
                        <div className="aod-item-name">{gift.title}</div>
                        <div className="aod-item-meta">Gift</div>
                      </div>
                      <div className="aod-item-qty">1 pc</div>
                      <div className="aod-item-price">{gift.price} ₴</div>
                    </div>
                  ))}
                  
                  {order.hasCard && (
                    <div className="aod-item">
                      <div className="aod-item-placeholder">💌</div>
                      <div className="aod-item-info">
                        <div className="aod-item-name">Greeting Card</div>
                        <div className="aod-item-meta">Added option</div>
                      </div>
                      <div className="aod-item-qty">1 pc</div>
                      <div className="aod-item-price">50 ₴</div>
                    </div>
                  )}
                </div>
              </section>
            )}

            {/* Comment */}
            {order.delivery.comment && (
              <section className="aod-card">
                <h2 className="aod-section-title">Customer Comment</h2>
                <p className="aod-comment">"{order.delivery.comment}"</p>
              </section>
            )}
          </div>

          {/* RIGHT COLUMN: INFO */}
          <div className="aod-sidebar">
            
            {/* Customer Info */}
            <section className="aod-card">
              <h2 className="aod-section-title">Customer</h2>
              <div className="aod-info-row">
                <span className="label">Name:</span>
                <span className="value">{order.customer.firstName} {order.customer.lastName}</span>
              </div>
              <div className="aod-info-row">
                <span className="label">Phone:</span>
                <span className="value">{order.customer.phone}</span>
              </div>
              <div className="aod-info-row">
                <span className="label">Email:</span>
                <span className="value">{order.customer.email}</span>
              </div>
            </section>

            {/* Delivery Info */}
            <section className="aod-card">
              <h2 className="aod-section-title">Delivery Details</h2>
              <div className="aod-info-block">
                <div className="label">Method:</div>
                <div className="value highlight">{order.delivery.method}</div>
              </div>
              <div className="aod-info-block">
                <div className="label">Address:</div>
                <div className="value">{order.delivery.address}</div>
              </div>
            </section>

            {/* Summary */}
            <section className="aod-card aod-summary">
              <h2 className="aod-section-title">Payment Summary</h2>
              <div className="aod-calc-row">
                <span>Subtotal</span>
                <span>{order.totals.subtotal} ₴</span>
              </div>
              <div className="aod-calc-row">
                <span>Delivery</span>
                <span>{order.totals.delivery} ₴</span>
              </div>
              <div className="aod-calc-row discount">
                <span>Discount</span>
                <span>-{order.totals.discount} ₴</span>
              </div>
              <div className="aod-divider"></div>
              <div className="aod-total-row">
                <span>Total</span>
                <span>{order.totals.total} ₴</span>
              </div>
            </section>

          </div>
        </div>
      </div>
    </div>
  );
}