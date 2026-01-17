import { useState } from "react";
import { useCart } from "../../context/CartContext"; 
import "./HomePage.css";

import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";
import HeroSection from "./components/HeroSection";

// Sub-components
import IntroSection from "./components/IntroSection";
import PopularSection from "./components/PopularSection";
import AboutSection from "./components/AboutSection";
import ReviewsSection from "./components/ReviewsSection";

import bouquet1L from "../../assets/images/bouquet1L.jpg";
import bouquet2 from "../../assets/images/bouquet2L.jpg";
import bouquet3 from "../../assets/images/bouquet3L.jpg";


export default function HomePage() {
  const [menuOpen, setMenuOpen] = useState(false);
  const { addToCart } = useCart(); 

  const popularItems = [
    { id: 1, title: "bouquet 1", img: bouquet1L, price: "1200 ₴" },
    { id: 2, title: "bouquet 2", img: bouquet2, price: "950 ₴" },
    { id: 3, title: "bouquet 3", img: bouquet3, price: "1500 ₴" },
    { id: 4, title: "bouquet 4", img: bouquet1L, price: "1100 ₴" },
    { id: 5, title: "bouquet 5", img: bouquet2, price: "1300 ₴" },
    { id: 6, title: "bouquet 6", img: bouquet3, price: "1400 ₴" },
  ];

  const reviewsData = [
    {
      id: 1,
      name: "Maria S.",
      text: "I really like the bouquet and recommend this store. Everything was perfect!",
      stars: 5,
    },
    {
      id: 2,
      name: "Alex D.",
      text: "Fresh flowers and fast delivery. Will order again for sure!",
      stars: 5,
    },
    {
      id: 3,
      name: "Elena K.",
      text: "Beautiful packaging and very polite courier. Thank you!",
      stars: 5,
    },
    {
      id: 4,
      name: "Ivan P.",
      text: "The roses stood for 2 weeks! Amazing quality.",
      stars: 5,
    },
    {
      id: 5,
      name: "Oksana M.",
      text: "Best flower shop in Chernivtsi. Highly recommended!",
      stars: 5,
    },
  ];

  const handleAddToCart = (item) => {
    addToCart({
      id: item.id,
      title: item.title,
      img: item.img,
      price: item.price,
      qty: 1,
    });
  };

  return (
    <div className="home-page">
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
        />

        <AboutSection />

        <ReviewsSection reviews={reviewsData} />
      </main>

      <Footer />
    </div>
  );
}
