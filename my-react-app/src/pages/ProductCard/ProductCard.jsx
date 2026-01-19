import React, { useState, useEffect, useMemo } from "react";
import { useParams, useNavigate } from "react-router-dom";
import axiosClient from "../../api/axiosClient";
import toast from "react-hot-toast";
import { useCart } from "../../context/CartContext";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";
import "./ProductCard.css";
import ShoppingBagIcon from "../../assets/images/ShoppingBagIcon.svg";

// Gifts
import gift1 from "../../assets/images/gift1.jpg";
import gift2 from "../../assets/images/gift2.jpg";
import gift3 from "../../assets/images/gift3.png";

// Map image names to imports (Removed - images now come from API)

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

  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [selectedImageIndex, setSelectedImageIndex] = useState(0);
  const [selectedSize, setSelectedSize] = useState("");
  const [menuOpen, setMenuOpen] = useState(false);
  const [giftModalOpen, setGiftModalOpen] = useState(false);
  const [selectedGift, setSelectedGift] = useState(null);
  const [recommendations, setRecommendations] = useState([]);

  // Reset image index when size changes
  useEffect(() => {
    setSelectedImageIndex(0);
  }, [selectedSize]);

  useEffect(() => {
    window.scrollTo(0, 0);

    const fetchProductData = async () => {
      setLoading(true);
      try {
        const response = await axiosClient.get(`/api/catalog/bouquets/${id}`, {
          headers: { Accept: "application/json" },
        });
        const data = response.data;

        // Map backend DTO to frontend structure
        const mappedProduct = {
          id: data.id,
          title: data.name,
          description: data.description,
          composition:
            data.sizes[0]?.flowers
              .map((f) => `${f.name} (${f.quantity})`)
              .join(", ") || "Diverse floral mix",
          // Store Array of images for each size
          images: data.sizes.reduce((acc, size) => {
            // Get all images for this size, sorted by position?
            // Assuming API returns them or we just map them.
            // If size has no images, fallback to main bouquet photo
            const sizeImgs =
              size.images && size.images.length > 0
                ? size.images.map((i) => i.imageUrl)
                : [data.mainPhotoUrl];

            acc[size.sizeName] = sizeImgs;
            return acc;
          }, {}),
          prices: data.sizes.reduce((acc, size) => {
            acc[size.sizeName] = size.price;
            return acc;
          }, {}),
          availableSizes: data.sizes.map((s) => s.sizeName),
        };

        setProduct(mappedProduct);
        // Default to M if available, else first size
        const defaultSize = mappedProduct.availableSizes.includes("M")
          ? "M"
          : mappedProduct.availableSizes[0];
        setSelectedSize(defaultSize);
      } catch (error) {
        console.error("Failed to fetch product details:", error);
        toast.error("Product not found");
      } finally {
        setLoading(false);
      }
    };

    // ... (recommendations fetch kept same)
    const fetchRecommendations = async () => {
      try {
        const response = await axiosClient.get("/api/catalog/bouquets", {
          params: { PageSize: 4 },
          headers: { Accept: "application/json" },
        });
        const items = response.data.items || response.data;
        setRecommendations(
          items
            .filter((p) => p.id !== id)
            .slice(0, 3)
            .map((p) => ({
              id: p.id,
              image: p.mainPhotoUrl,
              title: p.name,
              price: p.price,
            })),
        );
      } catch (error) {
        console.error("Failed to fetch recommendations:", error);
      }
    };

    fetchProductData();
    fetchRecommendations();
  }, [id]);

  // ... (Gifts array kept same) ...
  const gifts = [
    { id: "gift1", image: gift1, title: "Teddy Bear", price: "350 ₴" },
    { id: "gift2", image: gift2, title: "Chocolates", price: "250 ₴" },
    { id: "gift3", image: gift3, title: "Balloon", price: "150 ₴" },
  ];

  if (loading) {
    return <div className="loading-screen">Loading bouquet details...</div>;
  }

  if (!product) {
    return <div className="error-screen">Product not found</div>;
  }

  // Safe access to current images
  const currentImages = product.images[selectedSize] || [];
  const mainImageToShow = currentImages[selectedImageIndex] || currentImages[0];

  const handleAddToCart = () => {
    const cartProduct = {
      id: `${id}-${selectedSize}`,
      productId: id,
      title: product.title,
      price: `${product.prices[selectedSize]} ₴`,
      size: selectedSize,
      img: mainImageToShow, // Use displayed image
      qty: 1,
    };
    addToCart(cartProduct);
    toast.success(`${product.title} added to cart!`);
  };

  const handleBuyNow = () => {
    handleAddToCart();
    navigate("/order-registered");
  };

  // ... (handlers kept same)

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
          {/* Product Image Section */}
          <div className="product-image-section">
            <div className="product-image">
              <img
                src={mainImageToShow}
                alt={`${product.title} - Size ${selectedSize}`}
              />
            </div>

            {/* THUMBNAILS GALLERY */}
            {currentImages.length > 1 && (
              <div className="product-thumbnails">
                {currentImages.map((img, idx) => (
                  <div
                    key={idx}
                    className={`product-thumb ${idx === selectedImageIndex ? "active" : ""}`}
                    onClick={() => setSelectedImageIndex(idx)}>
                    <img src={img} alt={`Thumb ${idx}`} />
                  </div>
                ))}
              </div>
            )}
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
                {product.availableSizes.map((size) => (
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
