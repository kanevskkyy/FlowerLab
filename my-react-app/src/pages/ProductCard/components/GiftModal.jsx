import React from "react";
import { useTranslation } from "react-i18next";

const GiftModal = ({ isOpen, gift, onClose, onAddToCart }) => {
  const { t } = useTranslation();
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
        <p className="modal-question">{t("product.add_to_cart_question")}</p>
        <div className="modal-actions">
          <button className="modal-btn cancel-btn" onClick={onClose}>
            {t("product.cancel")}
          </button>
          <button
            className={`modal-btn add-btn ${gift.availableCount <= 0 ? "disabled" : ""}`}
            onClick={onAddToCart}
            disabled={gift.availableCount <= 0}>
            {gift.availableCount <= 0
              ? t("gifts.oos")
              : t("product.add_to_cart")}
          </button>
        </div>
      </div>
    </div>
  );
};

export default GiftModal;
