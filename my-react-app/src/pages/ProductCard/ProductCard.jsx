import React, { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useCart } from "../../context/CartContext";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";
import "./ProductCard.css";
import ShoppingBagIcon from "../../assets/images/ShoppingBagIcon.svg";
// 1
import Bouquet1S from "../../assets/images/bouquet1S.jpg";
import Bouquet1M from "../../assets/images/bouquet1M.jpg";
import Bouquet1L from "../../assets/images/bouquet1L.jpg";
import Bouquet1XL from "../../assets/images/bouquet1XL.jpg";
//2
import Bouquet2S from "../../assets/images/bouquet2S.jpg";
import Bouquet2M from "../../assets/images/bouquet2M.jpg";
import Bouquet2L from "../../assets/images/bouquet2XL.jpg";
import Bouquet2XL from "../../assets/images/bouquet2L.jpg";
//3
import Bouquet3S from "../../assets/images/bouquet3S.jpg";
import Bouquet3M from "../../assets/images/bouquet3M.JPG";
import Bouquet3L from "../../assets/images/bouquet3L.jpg";
import Bouquet3XL from "../../assets/images/bouquet3XL.png";
// Gifts
import gift1 from "../../assets/images/gift1.jpg";
import gift2 from "../../assets/images/gift2.jpg";
import gift3 from "../../assets/images/gift3.png";

const ProductCard = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToCart } = useCart();

  const [selectedSize, setSelectedSize] = useState("M");
  const [menuOpen, setMenuOpen] = useState(false);
  const [giftModalOpen, setGiftModalOpen] = useState(false);
  const [selectedGift, setSelectedGift] = useState(null);

  // Дані про продукти
  const products = {
    1: { 
      title: "BOUQUET 1", 
      prices: {
        S: "800 ₴",
        M: "1000 ₴",
        L: "1200 ₴",
        XL: "1500 ₴"
      },
      description: "A delicate bouquet combining soft pastel tones and natural textures. Perfect for any occasion — from friendly greetings to special celebrations.",
      composition: "orchids, peonies, greenery.",
      images: {
        S: Bouquet1S, 
        M: Bouquet1M, 
        L: Bouquet1L, 
        XL: Bouquet1XL 
      }
    },
    2: { 
      title: "BOUQUET 2", 
      prices: {
        S: "1200 ₴",
        M: "1800 ₴",
        L: "2300₴",
        XL: "3000 ₴"
      },
      description: "Elegant arrangement with premium roses and seasonal flowers.",
      composition: "roses, tulips, eucalyptus.",
      images: {
        S: Bouquet2S,
        M: Bouquet2M,
        L: Bouquet2L,
        XL: Bouquet2XL
      }
    },
    3: { 
      title: "BOUQUET 3", 
      prices: {
        S: "700 ₴",
        M: "900 ₴",
        L: "1100 ₴",
        XL: "1300 ₴"
      },
      description: "Charming spring bouquet with bright colors.",
      composition: "tulips, eustomas, greenery.",
      images: {
        S: Bouquet3S,
        M: Bouquet3M,
        L: Bouquet3L,
        XL: Bouquet3XL
      }
    }
  };

  const product = products[id] || products[1];

  // Рекомендовані товари
  const recommendations = [
    { id: 1, image: Bouquet1S, title: "Bouquet 1" },
    { id: 2, image: Bouquet2M, title: "Bouquet 2" },
    { id: 3, image: Bouquet3XL, title: "Bouquet 3" }
  ];

  // Best with (подарунки)
  const gifts = [
    { id: "gift1", image: gift1, title: "Teddy Bear", price: "350 ₴" },
    { id: "gift2", image: gift2, title: "Chocolates", price: "250 ₴" },
    { id: "gift3", image: gift3, title: "Balloon", price: "150 ₴" }
  ];

  const handleAddToCart = () => {
    const cartProduct = {
      id: `${id}-${selectedSize}`,
      productId: id,
      title: product.title,
      price: product.prices[selectedSize], 
      size: selectedSize,
      img: product.images[selectedSize],
      qty: 1
    };
    addToCart(cartProduct);
  };

  const handleBuyNow = () => {
    handleAddToCart();
    navigate("/checkout");
  };

  const handleGiftClick = (gift) => {
    setSelectedGift(gift);
    setGiftModalOpen(true);
  };

  const handleAddGiftToCart = () => {
    const giftProduct = {
      id: selectedGift.id,
      title: selectedGift.title,
      price: selectedGift.price,
      image: selectedGift.image,
      qty: 1,
      isGift: true
    };
    addToCart(giftProduct);
    setGiftModalOpen(false);
    setSelectedGift(null);
  };

  return (
    <div className="product-page">
      
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <div className="product-card">

        {/* Breadcrumbs */}
        <div className="breadcrumbs">
          <span onClick={() => navigate("/catalog")} className="breadcrumb-link">
            Catalog
          </span>
          <span className="separator">›</span>
          <span className="current">{product.title}</span>
        </div>

        {/* Main Product Content */}
        <div className="product-content">

          {/* Product Image - змінюється залежно від розміру */}
          <div className="product-image-section">
            <div className="product-image">
              <img 
                src={product.images[selectedSize]} 
                alt={`${product.title} - Size ${selectedSize}`}
              />
            </div>
          </div>

          {/* Product Info */}
          <div className="product-info">

            <h1 className="product-title">{product.title}</h1>
            <p className="product-price">{product.prices[selectedSize]}</p>

            {/* Description */}
            <div className="info-block">
              <h3>Description</h3>
              <p>{product.description}</p>
            </div>

            {/* Composition */}
            <div className="info-block">
              <h3>Composition:</h3>
              <p>{product.composition}</p>
            </div>

            {/* Size Selection */}
            <div className="size-section">
              <h3>Size</h3>
              <div className="size-buttons">
                {["S", "M", "L", "XL"].map((size) => (
                  <button
                    key={size}
                    className={`size-btn ${selectedSize === size ? "active" : ""}`}
                    onClick={() => setSelectedSize(size)}
                  >
                    {size}
                  </button>
                ))}
              </div>
            </div>

            {/* Actions - без quantity control */}
            <div className="product-actions">
              <button className="buy-now-btn" onClick={handleBuyNow}>
                BUY NOW
              </button>
              <button className="add-cart-btn" onClick={handleAddToCart}>
                ADD TO CART
                <span className="cart-icon"><img src={ShoppingBagIcon}></img></span>
              </button>
            </div>

          </div>
        </div>

        {/* You might like */}
        <section className="recommendations-section">
          <h2 className="section-title">You might like</h2>
          
          <div className="recommendations-grid">
            {recommendations.map((item) => (
              <div 
                key={item.id} 
                className="recommendation-card"
                onClick={() => navigate(`/product/${item.id}`)}
              >
                <div className="rec-image">
                  <img src={item.image} alt={item.title} />
                </div>
              </div>
            ))}
          </div>
        </section>

        {/* Best with */}
        <section className="recommendations-section">
          <h2 className="section-title">Best with</h2>
          
          <div className="recommendations-grid">
            {gifts.map((gift) => (
              <div 
                key={gift.id} 
                className="recommendation-card"
                onClick={() => handleGiftClick(gift)}
              >
                <div className="rec-image">
                  <img src={gift.image} alt={gift.title} />
                </div>

              </div>
            ))}
          </div>
        </section>

        {/* Reviews */}
        <section className="reviews-section">
          <h2 className="section-title">Reviews</h2>

          <div className="reviews-container">
            
            <div className="review-card">
              <div className="review-header">
                <div className="review-avatar"></div>
                <div className="review-info">
                  <p className="review-name">Anna Kovalenko</p>
                  <p className="review-stars">★★★★★</p>
                </div>
              </div>
              <p className="review-text">
                I really like the bouquet and recommend this store! Will buy from here again.
              </p>
            </div>

            <div className="review-card">
              <div className="review-header">
                <div className="review-avatar"></div>
                <div className="review-info">
                  <p className="review-name">Maria Shevchenko</p>
                  <p className="review-stars">★★★★★</p>
                </div>
              </div>
              <p className="review-text">
                Bought the bouquet for my mom, she liked it very much! Definitely buying again.
              </p>
            </div>

            <div className="review-card">
              <div className="review-header">
                <div className="review-avatar"></div>
                <div className="review-info">
                  <p className="review-name">Olena Petrenko</p>
                  <p className="review-stars">★★★★★</p>
                </div>
              </div>
              <p className="review-text">
                Beautiful flowers, fast delivery. Very satisfied with the service!
              </p>
            </div>

          </div>

          <button className="show-more-btn">Show more reviews</button>
        </section>

        {/* Back Button */}
        <div className="back-button-container">
          <button 
            className="back-to-catalog-btn" 
            onClick={() => navigate("/catalog")}
          >
            BACK TO THE CATALOG
          </button>
        </div>

      </div>

      {/* Gift Modal */}
      {giftModalOpen && selectedGift && (
        <div className="gift-modal-overlay" onClick={() => setGiftModalOpen(false)}>
          <div className="gift-modal" onClick={(e) => e.stopPropagation()}>
            <button className="modal-close" onClick={() => setGiftModalOpen(false)}>×</button>
            <div className="modal-image">
              <img src={selectedGift.image} alt={selectedGift.title} />
            </div>
            <h3>{selectedGift.title}</h3>
            <p className="modal-price">{selectedGift.price}</p>
            <p className="modal-question">Add to cart?</p>
            <div className="modal-actions">
              <button className="modal-btn cancel-btn" onClick={() => setGiftModalOpen(false)}>
                Cancel
              </button>
              <button className="modal-btn add-btn" onClick={handleAddGiftToCart}>
                Add to Cart
              </button>
            </div>
          </div>
        </div>
      )}

      <Footer />
    </div>
  );
};

export default ProductCard;