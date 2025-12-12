import React from "react";
import "./CartPopup.css";
import { useCart } from "../../context/CartContext";
import TrashIcon from "../../assets/images/trash-icon.svg";
import CloseIcon from "../../assets/images/close-icon.svg";

const CartPopup = ({ isOpen, onClose }) => {
  const { cartItems, increaseQty, decreaseQty, removeItem } = useCart();

  const total = cartItems.reduce(
    (sum, item) => sum + (item.qty || 1) * parseFloat(item.price),
    0
  );

  return (
    <div className={`cart-overlay ${isOpen ? "open" : ""}`} onClick={onClose}>
      <div className="cart-popup" onClick={(e) => e.stopPropagation()}>
        <div className="cart-popup-header">
          <button className="close-btn" onClick={onClose}>
            <img src={CloseIcon} alt="Close" />
          </button>
          <h2>YOUR CART</h2>
        </div>

        {/* EMPTY STATE */}
        {cartItems.length === 0 && (
          <div className="cart-empty">
            <p className="empty-message">
              You haven't added any items<br />to the cart yet.
            </p>
            <button className="shop-btn" onClick={onClose}>GO SHOPPING</button>
          </div>
        )}

        {/* ITEMS LIST */}
        {cartItems.length > 0 && (
          <>
            <div className="cart-content">
              {cartItems.map((item) => (
                <div className="cart-item" key={item.id}>
                  <img src={item.img} alt={item.title} className="cart-img" />

                  <div className="cart-item-details">
                    <div className="cart-item-header">
                      <p className="cart-item-title">{item.title}</p>
                    </div>

                    <p className="cart-item-price">{item.price} ₴</p>

                    <div className="qty-controls">
                      <button
                        className="qty-btn"
                        onClick={() => decreaseQty(item.id)}
                      >
                        −
                      </button>
                      <span className="qty-value">{item.qty || 1}</span>
                      <button
                        className="qty-btn"
                        onClick={() => increaseQty(item.id)}
                      >
                        +
                      </button>
                      
                      <button
                        className="trash-btn"
                        onClick={() => removeItem(item.id)}
                        aria-label="Remove item"
                      >
                        <img src={TrashIcon} alt="Remove" />
                      </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>

            <div className="cart-footer">
              <div className="cart-total">
                <span>TOTAL:</span>
                <span className="total-amount">{total} ₴</span>
              </div>

              <button className="checkout-btn">
                PROCEED TO CHECKOUT
              </button>

              <button className="continue-shopping" onClick={onClose}>
                ‹ Continue Shopping
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
};

export default CartPopup;