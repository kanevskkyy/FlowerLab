import React, { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import PopupMenu from "../popupMenu/PopupMenu";
import "./ProductCard.css";

const ProductCard = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [qty, setQty] = useState(1);
  const [selectedSize, setSelectedSize] = useState(null);
  const [menuOpen, setMenuOpen] = useState(false);

  const products = {
    1: { title: "Bouquet 1", price: "1000 ₴" },
    2: { title: "Bouquet 2", price: "1200 ₴" },
    3: { title: "Bouquet 3", price: "900 ₴" }
  };

  const product = products[id];

  return (
    <div className="page-wrapper">

      <header className="header">
        <div className="header-left">
          <button className="menu-btn" onClick={() => setMenuOpen(true)}>☰</button>
          <span className="lang">UA/ENG</span>
        </div>

        <div className="logo">[LOGO]</div>

        <div className="header-right">
          <span className="currency">UAH/USD</span>
          <button className="cart-btn">🛒</button>
          <button className="profile-btn">👤</button>
        </div>
      </header>
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      {/* ==================================================
         PRODUCT CARD CONTENT
      ================================================== */}
      <div className="product-card">

        {/* ------------------ Breadcrumbs ------------------ */}
        <div className="breadcrumbs">
        <span className="breadcrumb-link" onClick={() => navigate("/catalog")}>Catalog</span>
        <span className="separator">›</span><span>{product?.title}</span>
      </div>
        {/* ------------------ Main Product Layout ------------------ */}
        <div className="product-content">

          {/* Product Image */}
          <div className="product-image">
            <div className="image-placeholder">Image</div>
            <button className="image-nav left">‹</button>
            <button className="image-nav right">›</button>
          </div>

          {/* Product Info */}
          <div className="product-info">

            <h1 className="product-title">BOUQUET 1</h1>
            <p className="product-price">1000 ₴</p>

            {/* Description */}
            <div className="product-description">
              <h3>Description</h3>
              <p>
                A delicate bouquet combining soft pastel tones and natural textures.
                Perfect for any occasion — from friendly greetings to special celebrations.
              </p>
            </div>

            {/* Composition */}
            <div className="product-composition">
              <h3>Composition:</h3>
              <p>tulips, eustomas, greenery.</p>
            </div>

            {/* Size Buttons */}
            <div className="size-buttons">
            {["S", "M", "L", "XL"].map((size) => (<button key={size} className={selectedSize === size ? "active-size" : ""}
            onClick={() => setSelectedSize(size)}>
            {size}</button>))}
            </div>
            {/* Actions */}
            <div className="product-actions">
              <button className="buy-btn">BUY</button>

              <button className="add-to-cart-btn">
                ADD TO CART <span className="cart-icon">🛒</span>
              </button>

              <div className="quantity">
                <button onClick={() => setQty(qty > 1 ? qty - 1 : 1)}>-</button>
                <span>{qty}</span>
                <button onClick={() => setQty(qty + 1)}>+</button>
              </div>
            </div>

          </div>
        </div>


        {/* ==================================================
           RECOMMENDED SECTION
        ================================================== */}
        <section className="block-section">
          <h2>You might like</h2>

          <div className="block-items">
            <div className="small-placeholder"></div>
            <div className="small-placeholder"></div>
            <div className="small-placeholder"></div>
          </div>
        </section>


        {/* ==================================================
           BEST WITH SECTION
        ================================================== */}
        <section className="block-section">
          <h2>Best with</h2>

          <div className="block-items">
            <div className="small-placeholder"></div>
            <div className="small-placeholder"></div>
            <div className="small-placeholder"></div>
          </div>
        </section>


        {/* ==================================================
           REVIEWS SECTION
        ================================================== */}
        <section className="reviews">
          <h2>Reviews</h2>

          {/* Review 1 */}
          <div className="review-card">
            <div className="review-header">
              <div className="review-avatar"></div>
              <p className="review-name">[name surname]</p>
            </div>

            <p className="review-stars">★★★★★</p>

            <p className="review-text">
              I really like the bouquet and recommend this store! Will buy from here again.
            </p>
          </div>

          {/* Review 2 */}
          <div className="review-card">
            <div className="review-header">
              <div className="review-avatar"></div>
              <p className="review-name">[name surname]</p>
            </div>

            <p className="review-stars">★★★★★</p>

            <p className="review-text">
              Bought the bouquet for my mom, she liked it very much! Definitely buying again.
            </p>
          </div>

          {/* Show more */}
          <button className="show-more-btn">Show more</button>
        </section>


        {/* ==================================================
           BACK BUTTON
        ================================================== */}
        <button className="back-btn" onClick={() => navigate("/catalog")}>BACK TO THE CATALOG</button>


      </div>


      {/* ==================================================
         FOOTER
      ================================================== */}
      <footer className="footer">
        <div className="footer-col">
          <p>м. Київ, вул. Прикладна 7а</p>
          <p>Пн — Пт: 9:00 — 21:00</p>
        </div>

        <div className="footer-col">
          <p>+38 050 555 55 12</p>
          <p>info@example.com</p>
        </div>

        <div className="footer-col">
          <p>@florist_shop</p>
        </div>
      </footer>

    </div>
  );
};

export default ProductCard;
