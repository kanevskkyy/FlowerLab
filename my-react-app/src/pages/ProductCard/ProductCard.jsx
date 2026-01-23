import React, { useState, useMemo } from "react";
import { useParams, useNavigate } from "react-router-dom";
import toast from "react-hot-toast";
import { useCart } from "../../context/CartContext";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";
import "./ProductCard.css";

import { useProductData } from "./hooks/useProductData";
import { useGifts } from "../Gifts/hooks/useGifts";

import ProductImages from "./components/ProductImages";
import ProductInfo from "./components/ProductInfo";
import BestWithSlider from "./components/BestWithSlider";
import Recommendations from "./components/Recommendations";
import Reviews from "./components/Reviews";
import GiftModal from "./components/GiftModal";
import AddReviewModal from "./components/AddReviewModal";
import ProductDetailSkeleton from "./components/ProductDetailSkeleton";

import "./ProductCard.css";

const ProductCard = () => {
  const { id } = useParams();
  return <ProductCardContent key={id} />;
};

const ProductCardContent = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToCart } = useCart();

  const [menuOpen, setMenuOpen] = useState(false);
  const [giftModalOpen, setGiftModalOpen] = useState(false);
  const [reviewModalOpen, setReviewModalOpen] = useState(false);
  const [selectedGift, setSelectedGift] = useState(null);

  // Custom hook for product data
  const {
    product,
    loading,
    recommendations,
    reviews,
    selectedSize,
    setSelectedSize,
    selectedImageIndex,
    setSelectedImageIndex,
    refetchReviews,
  } = useProductData(id);

  const { gifts: fetchedGifts } = useGifts();

  const gifts = useMemo(() => {
    return fetchedGifts.map((g) => ({
      id: g.id,
      image: g.imageUrl || "/placeholder.png",
      title: g.name,
      price: `${g.price} ₴`,
    }));
  }, [fetchedGifts]);

  if (loading) {
    return <ProductDetailSkeleton />;
  }

  if (!product) {
    return <div className="error-screen">Product not found</div>;
  }

  // Safe access to current images
  const currentImages = product.images[selectedSize] || [];
  const mainImageToShow = currentImages[selectedImageIndex] || currentImages[0];

  const handleAddToCart = (openCart = true) => {
    const cartProduct = {
      id: `${id}-${selectedSize}`,
      bouquetId: id,
      sizeId: product.sizeIds[selectedSize],
      productId: id,
      title: product.title,
      price: `${product.prices[selectedSize]} ₴`,
      sizeName: selectedSize,
      img: mainImageToShow,
      qty: 1,
    };
    addToCart(cartProduct, openCart);
    if (openCart) {
      toast.success(`${product.title} added to cart!`);
    }
  };

  const handleBuyNow = () => {
    handleAddToCart(false);
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
          <ProductImages
            mainImage={mainImageToShow}
            thumbnails={currentImages}
            selectedIndex={selectedImageIndex}
            onSelect={setSelectedImageIndex}
            altTitle={`${product.title} - Size ${selectedSize}`}
          />

          <ProductInfo
            product={product}
            selectedSize={selectedSize}
            onSizeSelect={setSelectedSize}
            onBuyNow={handleBuyNow}
            onAddToCart={handleAddToCart}
          />
        </div>

        <Recommendations
          items={recommendations}
          onItemClick={(recId) => navigate(`/product/${recId}`)}
        />

        <BestWithSlider gifts={gifts} onGiftClick={handleGiftClick} />

        <Reviews
          reviews={reviews}
          onAddReview={() => setReviewModalOpen(true)}
        />

        <div className="back-button-container">
          <button className="back-to-catalog-btn" onClick={handleBackToCatalog}>
            BACK TO THE CATALOG
          </button>
        </div>
      </div>

      <GiftModal
        isOpen={giftModalOpen}
        gift={selectedGift}
        onClose={() => setGiftModalOpen(false)}
        onAddToCart={handleAddGiftToCart}
      />

      <AddReviewModal
        isOpen={reviewModalOpen}
        onClose={() => setReviewModalOpen(false)}
        bouquetId={id}
        onReviewSuccess={refetchReviews}
      />

      <Footer />
    </div>
  );
};

export default ProductCard;
