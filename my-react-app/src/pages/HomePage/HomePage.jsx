import { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useCart } from "../../context/CartContext";
import "./HomePage.css";
import catalogService from "../../services/catalogService";

import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../../components/PopupMenu/PopupMenu";
import HeroSection from "./components/HeroSection";

// Sub-components
import IntroSection from "./components/IntroSection";
import PopularSection from "./components/PopularSection";
import AboutSection from "./components/AboutSection";
import ReviewsSection from "./components/ReviewsSection";
import SEO from "../../components/SEO/SEO";

export default function HomePage() {
  const { t } = useTranslation();
  const [menuOpen, setMenuOpen] = useState(false);
  const { addToCart } = useCart();

  const [popularItems, setPopularItems] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(false);

  useEffect(() => {
    const fetchPopular = async () => {
      try {
        const data = await catalogService.getBouquets();
        const items = data.items || data || [];
        // Take top 4 or 6 items
        const mapped = items.slice(0, 6).map((b) => {
          // Default Size Logic (Minimum Price)
          let minPriceSize = null;
          if (b.sizes && b.sizes.length > 0) {
            const sortedSizes = [...b.sizes].sort((a, b) => a.price - b.price);
            minPriceSize = sortedSizes[0];
          }

          return {
            id: b.id, // This is productId
            bouquetId: b.id, // Distinct for clarity
            sizeId: minPriceSize ? minPriceSize.sizeId : null,
            sizeName: minPriceSize ? minPriceSize.sizeName : "Standard",
            title: b.name,
            img: b.mainPhotoUrl,
            // Fallback Price Strategy
            price: minPriceSize ? `${minPriceSize.price} ₴` : `${b.price} ₴`,
          };
        });
        setPopularItems(mapped);
      } catch (err) {
        console.error("Failed to fetch popular bouquets:", err);
        setError(true);
      } finally {
        setIsLoading(false);
      }
    };
    fetchPopular();
  }, []);
  const reviewsData = t("home.reviews_data", { returnObjects: true }) || [];

  const handleAddToCart = (item) => {
    // Safeguard: If sizeId is missing (e.g. backend cache stale), redirect to product page
    if (!item.sizeId) {
      window.location.href = `/product/${item.id}`;
      return;
    }

    addToCart({
      id: `${item.bouquetId}-${item.sizeName}`, // Composite ID for cart
      bouquetId: item.bouquetId,
      sizeId: item.sizeId,
      title: item.title,
      img: item.img,
      price: item.price,
      qty: 1,
    });
  };

  return (
    <div className="home-page">
      <SEO
        title={t("seo.home_title")}
        description={t("seo.home_desc")}
        image="/og-home.jpg"
      />
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="home-main">
        <div className="hero-section">
          <HeroSection />
        </div>

        <IntroSection />
        <PopularSection
          items={popularItems}
          onAddToCart={handleAddToCart}
          isLoading={isLoading}
          error={error}
        />

        <AboutSection />

        <ReviewsSection reviews={reviewsData} />
      </main>

      <Footer />
    </div>
  );
}
