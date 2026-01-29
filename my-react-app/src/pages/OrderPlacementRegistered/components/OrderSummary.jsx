import React from "react";
import { useFormContext } from "react-hook-form";
import { useTranslation } from "react-i18next";
import trash from "../../../assets/icons/trash.svg";
import message from "../../../assets/icons/message.svg";

const OrderSummary = ({
  cartItems,
  removeItem,
  selectedGifts,
  gifts,
  toggleGift,
  subtotal,
  discount,
  total,
  isCardAdded,
  toggleCard,
}) => {
  const { t } = useTranslation();
  const { register } = useFormContext();

  return (
    <div className="order-summary">
      <h2>{t("checkout.your_order")}</h2>

      <div className="order-items">
        {cartItems.map((item) => (
          <div key={item.id} className="order-item">
            <img src={item.img} alt={item.title} />
            <div className="item-details">
              <p className="item-name">{item.title}</p>
              {item.sizeName && (
                <p
                  className="item-size"
                  style={{
                    fontSize: "12px",
                    color: "#888",
                    margin: "-4px 0 4px 0",
                  }}>
                  {t("cart.size")}: {item.sizeName}
                </p>
              )}
              <p className="item-price">
                {typeof item.price === "string"
                  ? item.price
                  : `${item.price} ₴`}
              </p>
              <p className="item-quantity">
                {item.qty || 1} {t("checkout.pc")}
              </p>
            </div>
            <button
              type="button"
              className="remove-item-btn"
              onClick={() => removeItem(item.id)}>
              <img src={trash} alt="Remove" />
            </button>
          </div>
        ))}

        {selectedGifts.map((giftId) => {
          const gift = gifts.find((g) => g.id === giftId);
          if (!gift) return null;
          return (
            <div key={`gift-${gift.id}`} className="order-item">
              <img src={gift.imageUrl || "/placeholder.png"} alt={gift.name} />
              <div className="item-details">
                <p className="item-name">{gift.name}</p>
                <p className="item-price">{gift.price} ₴</p>
                <p className="item-quantity">1 {t("checkout.pc")}</p>
              </div>
              <button
                type="button"
                className="remove-item-btn"
                onClick={() => toggleGift(gift.id)}>
                <img src={trash} alt="Remove" />
              </button>
            </div>
          );
        })}

        {isCardAdded && (
          <div className="order-item">
            <img
              src={message}
              alt="Greeting Card"
              className="order-card-icon"
            />
            <div className="item-details">
              <p className="item-name">{t("checkout.greeting_card")}</p>
              <p className="item-price">50 ₴</p>
              <p className="item-quantity">1 {t("checkout.pc")}</p>
            </div>
            <button
              type="button"
              className="remove-item-btn"
              onClick={toggleCard}>
              <img src={trash} alt="Remove" />
            </button>
          </div>
        )}
      </div>

      <div className="order-calculations">
        <div className="calc-row">
          <span>{t("checkout.subtotal")}</span>
          <span>{subtotal} ₴</span>
        </div>
        <div className="calc-row discount">
          <span>{t("checkout.discount")}</span>
          <span>{discount > 0 ? "10%" : "0%"}</span>
        </div>
        <div className="calc-row total">
          <span>{t("checkout.total")}</span>
          <span>{total} ₴</span>
        </div>
      </div>

      <div className="add-card-section">
        <button type="button" className="add-card-btn" onClick={toggleCard}>
          {isCardAdded ? t("checkout.remove_card") : t("checkout.add_card")}
        </button>
        <span className="card-price">+ 50 ₴</span>
      </div>

      {isCardAdded && (
        <input
          type="text"
          className="card-message-input"
          placeholder={t("checkout.card_msg_placeholder")}
          {...register("cardMessage")}
        />
      )}

      <textarea
        className="message-input"
        placeholder={t("checkout.order_msg_placeholder")}
        {...register("message")}
      />

      <button
        type="submit"
        className="confirm-btn"
        disabled={total <= 0}
        style={{
          opacity: total <= 0 ? 0.5 : 1,
          cursor: total <= 0 ? "not-allowed" : "pointer",
        }}>
        {t("checkout.confirm_btn")}
      </button>
    </div>
  );
};

export default OrderSummary;
