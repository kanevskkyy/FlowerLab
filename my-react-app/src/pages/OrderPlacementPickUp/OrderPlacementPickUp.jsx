import React, { useState } from "react";
import PopupMenu from "../popupMenu/PopupMenu";
import "./OrderPlacementPickUp.css";

const OrderPlacementPickUp = () => {
  const [menuOpen, setMenuOpen] = useState(false);
  const [receiver, setReceiver] = useState("me");
  const [pickupPlace, setPickupPlace] = useState("hercena");

  return (
    <div className="page-wrapper order-page">
      {/* ================= HEADER ================= */}
      <header className="header">
        <div className="header-left">
          <div className="logo">[LOGO]</div>
        </div>

        <div className="header-center">ORDER PLACEMENT</div>

        <div className="header-right">
          <button className="cart-btn">🛒</button>
          <button className="profile-btn">👤</button>
        </div>
      </header>

      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      {/* ================= MAIN CONTENT ================= */}
      <main className="order-main">
        {/* -------- верхній ряд: контакт + замовлення -------- */}
        <div className="order-top">
          {/* CONTACT INFO */}
          <section className="card contact-card">
            <h2 className="card-title">Your Contact Information</h2>

            <div className="button-row">
              <button className="switch-btn active">I am registered</button>
              <button className="switch-btn">Buy without registering</button>
            </div>

            <div className="field-group">
              <label className="field-label">First Name</label>
              <input type="text" className="input-field" />
            </div>

            <div className="field-group">
              <label className="field-label">Last Name</label>
              <input type="text" className="input-field" />
            </div>

            <div className="field-group">
              <label className="field-label">Phone Number</label>
              <input type="text" className="input-field" />
            </div>

            <div className="field-group">
              <label className="field-label">Receiver:</label>
              <div className="radio-group">
                <label>
                  <input
                    type="radio"
                    checked={receiver === "me"}
                    onChange={() => setReceiver("me")}
                  />
                  I am the receiver
                </label>
                <label>
                  <input
                    type="radio"
                    checked={receiver === "other"}
                    onChange={() => setReceiver("other")}
                  />
                  The receiver is other person
                </label>
              </div>
            </div>
          </section>

          {/* ORDER SUMMARY (правий вузький блок) */}
          <section className="card summary-card">
            <h3 className="card-title small">YOUR ORDER:</h3>

            <div className="summary-item">
              <div className="summary-img" />
              <div className="summary-info">
                <p>Bouquet 1</p>
                <p>1000 ₴</p>
                <p>1 pc</p>
              </div>
            </div>

            <div className="summary-prices">
              <div className="price-row">
                <span>Delivery:</span>
                <span>100 ₴</span>
              </div>
              <div className="price-row total">
                <span>TOTAL:</span>
                <span>1100 ₴</span>
              </div>
            </div>

            <div className="field-group compact">
              <label className="field-label">Notes:</label>
              <textarea className="notes-field" />
            </div>

            <button className="confirm-btn">CONFIRM ORDER</button>
          </section>
        </div>

        {/* -------- блок Delivery -------- */}
        <section className="card delivery-card">
          <h2 className="card-title">Delivery</h2>

          <div className="delivery-tabs">
            <button className="delivery-btn active">Pickup at the shop</button>
            <button className="delivery-btn disabled">Delivery</button>
          </div>

          <div className="radio-group">
            <label>
              <input
                type="radio"
                name="pickup"
                checked={pickupPlace === "hercena"}
                onChange={() => setPickupPlace("hercena")}
              />
              м. Чернівці, вул. Герцена 2а
            </label>
            <label>
              <input
                type="radio"
                name="pickup"
                checked={pickupPlace === "vasile"}
                onChange={() => setPickupPlace("vasile")}
              />
              м. Чернівці, вул. Васіле Александрі 1
            </label>
          </div>
        </section>

        {/* -------- Gifts -------- */}
        <section className="card gifts-card">
          <h2 className="card-title">Gifts</h2>

          <div className="gifts-row">
            {[1, 2, 3].map((g) => (
              <div className="gift-card" key={g}>
                <div className="gift-img" />
                <div className="gift-text">Gift</div>
                <div className="gift-price">1000 ₴</div>
                <button className="gift-add">+</button>
              </div>
            ))}
          </div>
        </section>
      </main>

      {/* ================= FOOTER ================= */}
      <footer className="footer">
        <div className="footer-col">
          <p>м. Чернівці, вул. Герцена 2а</p>
          <p>вул. Васіле Александрі, 1</p>
        </div>

        <div className="footer-col">
          <p>+38 050 159 19 12</p>
        </div>

        <div className="footer-col">
          <p>@flowerlab_vlada</p>
        </div>
      </footer>
    </div>
  );
};

export default OrderPlacementPickUp;
