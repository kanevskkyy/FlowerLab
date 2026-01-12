import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCart } from "../../context/CartContext"; // Виправив імпорт на правильний
import toast from "react-hot-toast"; // Додав для сповіщень
import "./HomePage.css";

import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";
import HeroSection from "../../components/HeroSection/HeroSection";

import bouquet1L from "../../assets/images/bouquet1L.jpg";
import bouquet2 from "../../assets/images/bouquet2L.jpg";
import bouquet3 from "../../assets/images/bouquet3L.jpg";
import bouquet4 from "../../assets/images/about-image.png";

import ShoppingBagIcon from "../../assets/images/ShoppingBagIcon.svg";

export default function HomePage() {
  const navigate = useNavigate();
  const [menuOpen, setMenuOpen] = useState(false);
  const { addToCart } = useCart(); // Отримуємо функцію кошика

  // Додав ціни (price), щоб вони коректно передавалися в кошик
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

  const [currentReviewIdx, setCurrentReviewIdx] = useState(0);

  const [touchStart, setTouchStart] = useState(null);
  const [touchEnd, setTouchEnd] = useState(null);

  const minSwipeDistance = 50;

  const nextReview = () => {
    setCurrentReviewIdx((prev) => (prev + 1) % reviewsData.length);
  };

  const prevReview = () => {
    setCurrentReviewIdx((prev) =>
      prev === 0 ? reviewsData.length - 1 : prev - 1
    );
  };

  const onTouchStart = (e) => {
    setTouchEnd(null);
    setTouchStart(e.targetTouches[0].clientX);
  };

  const onTouchMove = (e) => {
    setTouchEnd(e.targetTouches[0].clientX);
  };

  const onTouchEnd = () => {
    if (!touchStart || !touchEnd) return;

    const distance = touchStart - touchEnd;
    const isLeftSwipe = distance > minSwipeDistance;
    const isRightSwipe = distance < -minSwipeDistance;

    if (isLeftSwipe) {
      nextReview();
    }
    if (isRightSwipe) {
      prevReview();
    }
  };

  const getVisibleReviews = () => {
    const visible = [];
    for (let i = 0; i < 3; i++) {
      const index = (currentReviewIdx + i) % reviewsData.length;
      visible.push(reviewsData[index]);
    }
    return visible;
  };

  return (
    <div className="home-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="home-main">
        <div className="hero-section">
          <HeroSection />
        </div>

        <section className="intro-section">
          <div className="intro-content">
            <h2 className="intro-title">
              We are FlowerLab Vlada, a floral studio.
            </h2>
            <p className="intro-desc">
              Our team creates premium bouquets and signature floral
              arrangements focused on quality, style, and emotion. With daily
              deliveries of fresh flowers, a wide selection, and attentive
              service, we&apos;ve become a studio people trust in Chernivtsi.
            </p>
          </div>
        </section>

        <section className="popular-section">
          <h2 className="section-title">POPULAR BOUQUETS / SALES</h2>
          <div className="popular-cards">
            {popularItems.map((item) => (
              <div
                key={item.id}
                className="popular-card"
                onClick={() => navigate(`/product/${item.id}`)}>
                <div className="popular-image">
                  <img
                    src={item.img}
                    alt={item.title}
                    className="popular-img"
                  />
                </div>

                <div className="popular-bottom">
                  <span className="popular-name">{item.title}</span>

                  {/* 👇 ЗМІНИ ТУТ: Залишили span, додали onClick з stopPropagation */}
                  <span
                    className="shopping-bag-icon"
                    onClick={(e) => {
                      e.stopPropagation(); // Зупиняє перехід на сторінку товару
                      addToCart({
                        id: item.id,
                        title: item.title,
                        img: item.img,
                        price: item.price,
                        qty: 1,
                      });
                      toast.success(`${item.title} added to cart!`);
                    }}>
                    <img src={ShoppingBagIcon} alt="Cart" />
                  </span>
                </div>
              </div>
            ))}
          </div>
        </section>

        <section className="about-section">
          <h2 className="section-title">ABOUT US</h2>
          <div className="about-content">
            <div className="about-text">
              <p>
                Welcome to our flower shop — a place where every bouquet tells a
                story. We believe that flowers are more than just a gift; they
                are a way to express emotions, share happiness, and make every
                moment memorable.
              </p>
              <p>
                Our team carefully selects fresh flowers every day to ensure the
                highest quality and beauty in every arrangement. Whether it’s a
                romantic gesture, a celebration, or just a small token of
                appreciation, we create bouquets that speak from the heart.
              </p>
              <p>
                We take pride in our attention to detail, creative designs, and
                friendly service. Each bouquet is handcrafted with love,
                tailored to fit your style and occasion.
              </p>
            </div>
            <div className="about-image">
              <img src={bouquet4} alt="About banner" className="about-img" />
            </div>
          </div>
        </section>

        <section
          className="home-reviews-section"
          onTouchStart={onTouchStart}
          onTouchMove={onTouchMove}
          onTouchEnd={onTouchEnd}>
          <h2 className="section-title">REVIEWS</h2>
          <div className="home-reviews-wrapper">
            <button className="reviews-arrow" onClick={prevReview}>
              ‹
            </button>

            <div className="home-reviews-cards">
              {getVisibleReviews().map((review, index) => (
                <div
                  key={`${review.id}-${index}`}
                  className="home-review-card fade-in">
                  <div className="home-review-top">
                    <div className="home-review-avatar" />
                    <span className="home-review-name">{review.name}</span>
                  </div>
                  <div className="home-review-stars">
                    {"★".repeat(review.stars)}
                  </div>
                  <p className="home-review-text">"{review.text}"</p>
                </div>
              ))}
            </div>

            <button className="reviews-arrow" onClick={nextReview}>
              ›
            </button>
          </div>
        </section>
      </main>

      <Footer />
    </div>
  );
}
