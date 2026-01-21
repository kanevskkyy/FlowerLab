import React from "react";

const GiftModal = ({ isOpen, gift, onClose, onAddToCart }) => {
  if (!isOpen || !gift) return null;

  return (
    <div className="gift-modal-overlay" onClick={onClose}>
      <div className="gift-modal" onClick={(e) => e.stopPropagation()}>
        <button className="modal-close" onClick={onClose}>
          ×
        </button>
        <div className="modal-image">
          <img src={gift.image} alt={gift.title} />
        </div>
        <h3>{gift.title}</h3>
        <p className="modal-price">{gift.price}</p>
        <p className="modal-question">Add to cart?</p>
        <div className="modal-actions">
          <button className="modal-btn cancel-btn" onClick={onClose}>
            Cancel
          </button>
          <button className="modal-btn add-btn" onClick={onAddToCart}>
            Add to Cart
          </button>
        </div>
      </div>
    </div>
  );
};

export default GiftModal;
