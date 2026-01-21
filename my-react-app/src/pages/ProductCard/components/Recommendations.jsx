import React from "react";

const Recommendations = ({ items, onItemClick }) => {
  if (!items || items.length === 0) return null;

  return (
    <section className="recommendations-section">
      <h2 className="section-title">You might like</h2>

      <div className="recommendations-grid">
        {items.map((item) => (
          <div
            key={item.id}
            className="recommendation-card"
            onClick={() => onItemClick(item.id)}>
            <div className="rec-image">
              <img src={item.image} alt={item.title} />
            </div>
            <div className="rec-details">
              <p className="rec-title">{item.title}</p>
              <p className="rec-price">from {item.price} ₴</p>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
};

export default Recommendations;
