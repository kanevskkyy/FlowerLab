import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./HomePage.css";

import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";

// ===== HERO IMAGES =====
import hero1 from "../../assets/images/bouquet1.jpg";
import hero2 from "../../assets/images/bouquet2L.jpg";
import hero3 from "../../assets/images/bouquet3L.jpg";
import ShoppingBagIcon from "../../assets/icons/shopping-bag.svg";


// ===== POPULAR IMAGES (як у Catalog) =====
import bouquet1L from "../../assets/images/bouquet1L.jpg";
import bouquet2 from "../../assets/images/bouquet2L.jpg";
import bouquet3 from "../../assets/images/bouquet3L.jpg";

export default function HomePage() {
  const navigate = useNavigate();

  const [menuOpen, setMenuOpen] = useState(false);
  const [slide, setSlide] = useState(0);

  // HERO SLIDER
  const heroImages = [hero1, hero2, hero3];

  const prevSlide = () => {
    setSlide((prev) => (prev - 1 + heroImages.length) % heroImages.length);
  };

  const nextSlide = () => {
    setSlide((prev) => (prev + 1) % heroImages.length);
  };

  // POPULAR (IDs беруться з твого Catalog: 1,2,3)
  const popularItems = [
    { id: 1, title: "bouquet 1", img: bouquet1L },
    { id: 2, title: "bouquet 2", img: bouquet2 },
    { id: 3, title: "bouquet 3", img: bouquet3 },
  ];

  return (
    <div className="home-page">
      {/* HEADER */}
      <Header onMenuOpen={() => setMenuOpen(true)} />

      {/* POPUP MENU */}
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      {/* MAIN */}
      <main className="home-main">
        {/* ===== HERO SLIDER ===== */}
        <section className="hero-section">
          <div className="hero-banner">
            <img
              src={heroImages[slide]}
              alt={`Hero slide ${slide + 1}`}
              className="hero-image"
              draggable="false"
            />

            <button
              className="hero-arrow hero-arrow-left"
              onClick={prevSlide}
              aria-label="Previous slide"
              type="button"
            >
              ‹
            </button>

            <button
              className="hero-arrow hero-arrow-right"
              onClick={nextSlide}
              aria-label="Next slide"
              type="button"
            >
              ›
            </button>
          </div>
        </section>

        {/* ORDER */}
        <div className="order-wrapper">
          <button className="order-button">ORDER</button>
        </div>

        {/* POPULAR */}
        <section className="popular-section">
          <h2 className="section-title">POPULAR BOUQUETS / SALES</h2>

          <div className="popular-cards">
            {popularItems.map((item) => (
              <div
                key={item.id}
                className="popular-card"
                role="button"
                tabIndex={0}
                onClick={() => navigate(`/product/${item.id}`)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") navigate(`/product/${item.id}`);
                }}
              >
                <div className="popular-image">
                  <img
                    src={item.img}
                    alt={item.title}
                    className="popular-img"
                    draggable="false"
                  />
                </div>

                <div className="popular-bottom">
                  <span className="popular-name">{item.title}</span>
                 <span className="shopping-bag-icon">
  <img src={ShoppingBagIcon} alt="Shopping bag" />
</span>
                </div>
              </div>
            ))}
          </div>
        </section>

        {/* ABOUT */}
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
                highest quality and beauty in every arrangement. Whether it&apos;s
                a romantic gesture, a celebration, or just a small token of
                appreciation, we create bouquets that speak from the heart.
              </p>
              <p>
                We take pride in our attention to detail, creative designs, and
                friendly service. Each bouquet is handcrafted with love,
                tailored to fit your style and occasion.
              </p>
            </div>

            {/* ✅ ABOUT BANNER IMAGE */}
            <div className="about-image">
              <img
                src={bouquet3}
                alt="About banner"
                className="about-img"
                draggable="false"
              />
            </div>
          </div>
        </section>

        {/* REVIEWS */}
        <section className="home-reviews-section">
          <h2 className="section-title">REVIEWS</h2>

          <div className="home-reviews-wrapper">
           <button
  className="hero-arrow reviews-arrow"
  aria-label="Next review"
  type="button"
>
  ‹
</button>

            <div className="home-reviews-cards">
              {Array.from({ length: 3 }).map((_, i) => (
                <div key={i} className="home-review-card">
                  <div className="home-review-top">
                    <div className="home-review-avatar" />
                    <span className="home-review-name">[Name Surname]</span>
                  </div>

                  <div className="home-review-stars">
                    {"★★★★★".split("").map((_, j) => (
                      <span key={j}>★</span>
                    ))}
                  </div>

                  <p className="home-review-text">
                    i really like the bouquet and recommend this store
                  </p>
                </div>
              ))}
            </div>
<button
  className="hero-arrow reviews-arrow"
  aria-label="Previous review"
  type="button"
>
  ›
</button>



          </div>
        </section>
      </main>

      {/* FOOTER */}
      <Footer />
    </div>
  );
}
