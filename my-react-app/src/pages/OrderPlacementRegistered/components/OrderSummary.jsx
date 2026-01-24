import React from "react";
import { useFormContext } from "react-hook-form";
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
  const { register } = useFormContext();

  return (
    <div className="order-summary">
      <h2>YOUR ORDER:</h2>

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
                  Size: {item.sizeName}
                </p>
              )}
              <p className="item-price">
                {typeof item.price === "string"
                  ? item.price
                  : `${item.price} ₴`}
              </p>
              <p className="item-quantity">{item.qty || 1} pc</p>
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
                <p className="item-quantity">1 pc</p>
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
              <p className="item-name">Greeting Card</p>
              <p className="item-price">50 ₴</p>
              <p className="item-quantity">1 pc</p>
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
          <span>Subtotal:</span>
          <span>{subtotal} ₴</span>
        </div>
        <div className="calc-row discount">
          <span>Discount:</span>
          <span>{discount > 0 ? "10%" : "0%"}</span>
        </div>
        <div className="calc-row total">
          <span>TOTAL:</span>
          <span>{total} ₴</span>
        </div>
      </div>

      <div className="add-card-section">
        <button type="button" className="add-card-btn" onClick={toggleCard}>
          {isCardAdded ? "- Remove card" : "+ Add a card"}
        </button>
        <span className="card-price">+ 50 ₴</span>
      </div>

      {isCardAdded && (
        <input
          type="text"
          className="card-message-input"
          placeholder="Message for the card"
          {...register("cardMessage")}
        />
      )}

      <textarea
        className="message-input"
        placeholder="Input your message"
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
        CONFIRM ORDER
      </button>
    </div>
  );
};

export default OrderSummary;
