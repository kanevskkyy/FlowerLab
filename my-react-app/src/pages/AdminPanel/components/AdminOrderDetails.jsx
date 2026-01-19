import React from "react";
import "./AdminOrderDetails.css";

// This component acts as a Modal
export default function AdminOrderDetails({ order, isOpen, onClose, loading }) {
  if (!isOpen) return null;

  return (
    <div className="aod-overlay" onClick={onClose}>
      <div className="aod-modal" onClick={(e) => e.stopPropagation()}>
        <button className="aod-close" onClick={onClose}>
          ×
        </button>
        
        {loading || !order ? (
          <div className="aod-loading">Loading details...</div>
        ) : (
          <div className="aod-content">
            <header className="aod-header">
              <h2>Order #{order.id.substring(0, 8).toUpperCase()}</h2>
              <span className={`aod-status status-${order.status?.name?.toLowerCase() || "new"}`}>
                {order.status?.name || "Unknown"}
              </span>
            </header>

            <div className="aod-grid">
              {/* Left: Items */}
              <div className="aod-section">
                <h3>Items</h3>
                <div className="aod-items-list">
                  {order.items?.map((item, idx) => (
                    <div key={idx} className="aod-item">
                      <div className="aod-item-img">
                         {item.bouquetImage && <img src={item.bouquetImage} alt="" />}
                      </div>
                      <div className="aod-item-info">
                        <div className="aod-item-name">{item.bouquetName}</div>
                        <div className="aod-item-price">
                          {item.price} ₴ x {item.count}
                        </div>
                      </div>
                      <div className="aod-item-total">
                        {item.price * item.count} ₴
                      </div>
                    </div>
                  ))}
                </div>
                <div className="aod-total-row">
                   <span>Total Amount:</span>
                   <span className="aod-total-val">{order.totalPrice} ₴</span>
                </div>
              </div>

              {/* Right: Customer & Delivery */}
              <div className="aod-section">
                <h3>Customer Info</h3>
                <div className="aod-row">
                   <label>Name:</label>
                   <span>{order.firstName} {order.lastName}</span>
                </div>
                <div className="aod-row">
                   <label>Phone:</label>
                   <span>{order.phoneNumber}</span>
                </div>
                
                <h3 className="mt-4">Delivery</h3>
                <div className="aod-row">
                   <label>City:</label>
                   <span>{order.city}</span>
                </div>
                <div className="aod-row">
                   <label>Address:</label>
                   <span>{order.deliveryAddress}</span>
                </div>
                {order.comment && (
                  <div className="aod-row block">
                    <label>Comment:</label>
                    <p>{order.comment}</p>
                  </div>
                )}
                 {order.postcardText && (
                  <div className="aod-row block">
                    <label>Postcard:</label>
                    <p>"{order.postcardText}"</p>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
