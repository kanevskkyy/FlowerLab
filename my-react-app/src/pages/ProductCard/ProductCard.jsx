import React, { useState, useEffect, useMemo } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useCart } from "../../context/CartContext";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";
import "./ProductCard.css";
import ShoppingBagIcon from "../../assets/images/ShoppingBagIcon.svg";

// Import all images
import Bouquet1S from "../../assets/images/bouquet1S.jpg";
import Bouquet1M from "../../assets/images/bouquet1M.jpg";
import Bouquet1L from "../../assets/images/bouquet1L.jpg";
import Bouquet1XL from "../../assets/images/bouquet1XL.jpg";
import Bouquet2S from "../../assets/images/bouquet2S.jpg";
import Bouquet2M from "../../assets/images/bouquet2M.jpg";
import Bouquet2L from "../../assets/images/bouquet2L.jpg";
import Bouquet2XL from "../../assets/images/bouquet2XL.jpg";
import Bouquet3S from "../../assets/images/bouquet3S.jpg";
import Bouquet3M from "../../assets/images/bouquet3M.JPG";
import Bouquet3L from "../../assets/images/bouquet3L.jpg";
import Bouquet3XL from "../../assets/images/bouquet3XL.png";
import bouquet4S from "../../assets/images/bouquet4S.jpg";
import bouquet4M from "../../assets/images/bouquet4M.JPG";
import bouquet4L from "../../assets/images/bouquet4L.jpg";
import bouquet4XL from "../../assets/images/bouquet4XL.jpg";
import bouquet5S from "../../assets/images/bouquet5S.jpg";
import bouquet5M from "../../assets/images/bouquet5M.jpg";
import bouquet5L from "../../assets/images/bouquet5L.jpg";
import bouquet5XL from "../../assets/images/bouquet5XL.jpg";
import bouquet6S from "../../assets/images/bouquet6S.jpg";
import bouquet6M from "../../assets/images/bouquet6M.png";
import bouquet6L from "../../assets/images/bouquet6L.png";
import bouquet6XL from "../../assets/images/bouquet6XL.png";
import bouquet7S from "../../assets/images/bouquet7S.jpg";
import bouquet7M from "../../assets/images/bouquet7M.jpg";
import bouquet7L from "../../assets/images/bouquet7L.png";
import bouquet7XL from "../../assets/images/bouquet7XL.png";
import bouquet8S from "../../assets/images/bouquet8S.jpg";
import bouquet8M from "../../assets/images/bouquet8M.jpg";
import bouquet8L from "../../assets/images/bouquet8L.jpg";
import bouquet8XL from "../../assets/images/bouquet8XL.jpg";
import bouquet9S from "../../assets/images/bouquet9S.jpg";
import bouquet9M from "../../assets/images/bouquet9M.jpg";
import bouquet9L from "../../assets/images/bouquet9L.png";
import bouquet9XL from "../../assets/images/bouquet9XL.jpg";
import bouquet10S from "../../assets/images/bouquet10S.jpg";
import bouquet10M from "../../assets/images/bouquet10M.jpg";
import bouquet10L from "../../assets/images/bouquet10L.jpg";
import bouquet10XL from "../../assets/images/bouquet10XL.jpg";
import bouquet11S from "../../assets/images/bouquet11S.jpg";
import bouquet11M from "../../assets/images/bouquet11M.jpg";
import bouquet11L from "../../assets/images/bouquet11L.jpg";
import bouquet11XL from "../../assets/images/bouquet11XL.jpg";
import bouquet12S from "../../assets/images/bouquet12S.jpg";
import bouquet12M from "../../assets/images/bouquet12M.png";
import bouquet12L from "../../assets/images/bouquet12L.png";
import bouquet12XL from "../../assets/images/bouquet12XL.png";
import bouquet13S from "../../assets/images/bouquet13S.jpg";
import bouquet13M from "../../assets/images/bouquet13M.jpg";
import bouquet13L from "../../assets/images/bouquet13L.jpg";
import bouquet13XL from "../../assets/images/bouquet13XL.jpg";
import bouquet14S from "../../assets/images/bouquet14S.jpg";
import bouquet14M from "../../assets/images/bouquet14M.jpg";
import bouquet14L from "../../assets/images/bouquet14L.jpg";
import bouquet14XL from "../../assets/images/bouquet14XL.jpg";
import bouquet15S from "../../assets/images/bouquet15S.jpg";
import bouquet15M from "../../assets/images/bouquet15M.jpg";
import bouquet15L from "../../assets/images/bouquet15L.jpg";
import bouquet15XL from "../../assets/images/bouquet15XL.jpg";

// Gifts
import gift1 from "../../assets/images/gift1.jpg";
import gift2 from "../../assets/images/gift2.jpg";
import gift3 from "../../assets/images/gift3.png";

// Import products data
import { productsData } from "../../data/productsData";
import toast from "react-hot-toast";

// Map image names to imports
const imageMap = {
  bouquet1S: Bouquet1S,
  bouquet1M: Bouquet1M,
  bouquet1L: Bouquet1L,
  bouquet1XL: Bouquet1XL,
  bouquet2S: Bouquet2S,
  bouquet2M: Bouquet2M,
  bouquet2L: Bouquet2L,
  bouquet2XL: Bouquet2XL,
  bouquet3S: Bouquet3S,
  bouquet3M: Bouquet3M,
  bouquet3L: Bouquet3L,
  bouquet3XL: Bouquet3XL,
  bouquet4S: bouquet4S,
  bouquet4M: bouquet4M,
  bouquet4L: bouquet4L,
  bouquet4XL: bouquet4XL,
  bouquet5S: bouquet5S,
  bouquet5M: bouquet5M,
  bouquet5L: bouquet5L,
  bouquet5XL: bouquet5XL,
  bouquet6S: bouquet6S,
  bouquet6M: bouquet6M,
  bouquet6L: bouquet6L,
  bouquet6XL: bouquet6XL,
  bouquet7S: bouquet7S,
  bouquet7M: bouquet7M,
  bouquet7L: bouquet7L,
  bouquet7XL: bouquet7XL,
  bouquet8S: bouquet8S,
  bouquet8M: bouquet8M,
  bouquet8L: bouquet8L,
  bouquet8XL: bouquet8XL,
  bouquet9S: bouquet9S,
  bouquet9M: bouquet9M,
  bouquet9L: bouquet9L,
  bouquet9XL: bouquet9XL,
  bouquet10S: bouquet10S,
  bouquet10M: bouquet10M,
  bouquet10L: bouquet10L,
  bouquet10XL: bouquet10XL,
  bouquet11S: bouquet11S,
  bouquet11M: bouquet11M,
  bouquet11L: bouquet11L,
  bouquet11XL: bouquet11XL,
  bouquet12S: bouquet12S,
  bouquet12M: bouquet12M,
  bouquet12L: bouquet12L,
  bouquet12XL: bouquet12XL,
  bouquet13S: bouquet13S,
  bouquet13M: bouquet13M,
  bouquet13L: bouquet13L,
  bouquet13XL: bouquet13XL,
  bouquet14S: bouquet14S,
  bouquet14M: bouquet14M,
  bouquet14L: bouquet14L,
  bouquet14XL: bouquet14XL,
  bouquet15S: bouquet15S,
  bouquet15M: bouquet15M,
  bouquet15L: bouquet15L,
  bouquet15XL: bouquet15XL,
};

// =================================================================
// MAIN COMPONENT WRAPPER
// =================================================================
const ProductCard = () => {
  const { id } = useParams();
  return <ProductCardContent key={id} />;
};

// =================================================================
// INNER CONTENT COMPONENT
// =================================================================
const ProductCardContent = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToCart } = useCart();

  const [selectedSize, setSelectedSize] = useState("M");
  const [menuOpen, setMenuOpen] = useState(false);
  const [giftModalOpen, setGiftModalOpen] = useState(false);
  const [selectedGift, setSelectedGift] = useState(null);

  // Знайти продукт за ID
  const productData =
    productsData.find((p) => p.id === parseInt(id)) || productsData[0];

  // Створити об'єкт продукту з зображеннями
  const product = {
    ...productData,
    images: {
      S: imageMap[productData.images.S],
      M: imageMap[productData.images.M],
      L: imageMap[productData.images.L],
      XL: imageMap[productData.images.XL],
    },
  };

  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  const recommendations = useMemo(() => {
    const currentId = parseInt(id);
    const others = productsData.filter((p) => p.id !== currentId);

    const shift = currentId % others.length;
    const rotated = [...others.slice(shift), ...others.slice(0, shift)];

    return rotated.slice(0, 3).map((p) => ({
      id: p.id,
      image: imageMap[p.mainImage],
      title: p.title,
      price: p.price,
    }));
  }, [id]);

  const gifts = [
    { id: "gift1", image: gift1, title: "Teddy Bear", price: "350 ₴" },
    { id: "gift2", image: gift2, title: "Chocolates", price: "250 ₴" },
    { id: "gift3", image: gift3, title: "Balloon", price: "150 ₴" },
  ];

  const handleAddToCart = () => {
    const cartProduct = {
      id: `${id}-${selectedSize}`,
      productId: id,
      title: product.title,
      price: `${product.prices[selectedSize]} ₴`,
      size: selectedSize,
      img: product.images[selectedSize],
      qty: 1,
    };
    addToCart(cartProduct);
    toast.success(`${product.title} added to cart!`);
  };

  const handleBuyNow = () => {
    handleAddToCart();
    navigate("/order-registered");
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
      img: selectedGift.image,
      qty: 1,
      isGift: true,
    };
    addToCart(giftProduct);
    setGiftModalOpen(false);
    setSelectedGift(null);
  };

  // ✅ ФУНКЦІЯ ДЛЯ ПЕРЕХОДУ В КАТАЛОГ (зі скролом)
  const handleBackToCatalog = () => {
    window.scrollTo(0, 0);
    navigate("/catalog");
  };

  return (
    <div className="product-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <div className="product-card">
        {/* Breadcrumbs */}
        <div className="breadcrumbs">
          <span onClick={handleBackToCatalog} className="breadcrumb-link">
            Catalog
          </span>
          <span className="separator">›</span>
          <span className="current">{product.title}</span>
        </div>

        {/* Main Product Content */}
        <div className="product-content">
          {/* Product Image */}
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

            <p className="product-price">{product.prices[selectedSize]} ₴</p>

            <div className="info-block">
              <h3>Description</h3>
              <p>{product.description}</p>
            </div>

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
                    className={`size-btn ${
                      selectedSize === size ? "active" : ""
                    }`}
                    onClick={() => setSelectedSize(size)}>
                    {size}
                  </button>
                ))}
              </div>
            </div>

            {/* Actions */}
            <div className="product-actions">
              <button className="buy-now-btn" onClick={handleBuyNow}>
                BUY NOW
              </button>
              <button className="add-cart-btn" onClick={handleAddToCart}>
                ADD TO CART
                <span className="cart-icon">
                  <img src={ShoppingBagIcon} alt="Cart" />
                </span>
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
                onClick={() => navigate(`/product/${item.id}`)}>
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

        {/* Best with */}
        <section className="recommendations-section">
          <h2 className="section-title">Best with</h2>

          <div className="recommendations-grid">
            {gifts.map((gift) => (
              <div
                key={gift.id}
                className="recommendation-card"
                onClick={() => handleGiftClick(gift)}>
                <div className="rec-image">
                  <img src={gift.image} alt={gift.title} />
                </div>
                <div className="rec-details">
                  <p className="rec-title">{gift.title}</p>
                  <p className="rec-price">{gift.price}</p>
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
                I really like the bouquet and recommend this store! Will buy
                from here again.
              </p>
            </div>
            {/* ... other reviews ... */}
          </div>

          <button className="show-more-btn">Show more reviews</button>
        </section>

        {/* Back Button */}
        <div className="back-button-container">
          <button className="back-to-catalog-btn" onClick={handleBackToCatalog}>
            BACK TO THE CATALOG
          </button>
        </div>
      </div>

      {/* Gift Modal */}
      {giftModalOpen && selectedGift && (
        <div
          className="gift-modal-overlay"
          onClick={() => setGiftModalOpen(false)}>
          <div className="gift-modal" onClick={(e) => e.stopPropagation()}>
            <button
              className="modal-close"
              onClick={() => setGiftModalOpen(false)}>
              ×
            </button>
            <div className="modal-image">
              <img src={selectedGift.image} alt={selectedGift.title} />
            </div>
            <h3>{selectedGift.title}</h3>
            <p className="modal-price">{selectedGift.price}</p>
            <p className="modal-question">Add to cart?</p>
            <div className="modal-actions">
              <button
                className="modal-btn cancel-btn"
                onClick={() => setGiftModalOpen(false)}>
                Cancel
              </button>
              <button
                className="modal-btn add-btn"
                onClick={handleAddGiftToCart}>
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
