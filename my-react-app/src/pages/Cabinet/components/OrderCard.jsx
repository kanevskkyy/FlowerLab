import React from "react";
import "./OrderCard.css";
import CardIcon from "../../../assets/icons/message.svg";

const OrderCard = ({ order }) => {
  return (
    <div className="history-card">
      {/* HEADER: ID and Date */}
      <div className="history-header">
        <span className="history-id">{order.id}</span>
        <span className="history-date">at {order.date}</span>
      </div>

      {/* CONTENT: Grid of items */}
      <div className="history-items-grid">
        {order.items.map((item, idx) => (
          <div key={`${item.id}-${idx}`} className="history-grid-item">
            <div className="grid-img-wrapper">
              {item.img && (
                <img src={item.img} alt={item.title} loading="lazy" />
              )}
            </div>
            <div className="grid-item-info">
              <span className="grid-title">{item.title}</span>
              {item.size && (
                <span className="grid-size">Size: {item.size}</span>
              )}
              <div className="grid-price-row">
                <span className="grid-price">
                  {item.price} {order.currency}
                </span>
                <span className="grid-qty">{item.qty}</span>
              </div>
            </div>
          </div>
        ))}

        {/* Postcard Item */}
        {order.cardText && (
          <div className="history-grid-item card-item">
            <div className="grid-card-wrapper">
              <img src={CardIcon} alt="Card" className="card-icon-svg" />
              <span className="card-label">POSTCARD</span>
            </div>
            <div className="grid-card-text">"{order.cardText}"</div>
          </div>
        )}
      </div>

      {/* FOOTER: Total and Status */}
      <div className="history-footer">
        <div className="history-total">
          Order Total: {order.total} {order.currency}
        </div>
        <div className="history-status">
          Status:{" "}
          <span className={`status-badge ${order.status.toLowerCase()}`}>
            {order.status}
          </span>
        </div>
      </div>
    </div>
  );
};

export default OrderCard;
