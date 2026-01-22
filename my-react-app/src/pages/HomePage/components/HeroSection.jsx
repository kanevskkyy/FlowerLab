import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

// Images
import heroImg1 from "../../../assets/images/bouquet1L.webp";
import heroImg2 from "../../../assets/images/bouquet2L.webp";
import heroImg3 from "../../../assets/images/bouquet3L.webp";
import logo from "../../../assets/icons/banner-logo.svg";
import arrowLeft from "../../../assets/icons/arrow-left.svg";
import arrowRight from "../../../assets/icons/arrow-right.svg";

const HeroSection = () => {
  const navigate = useNavigate();
  const [currentSlide, setCurrentSlide] = useState(0);

  // Дані для слайдів
  const slides = [
    { id: 1, image: heroImg1 },
    { id: 2, image: heroImg2 },
    { id: 3, image: heroImg3 },
  ];

  // === ЛОГІКА СВАЙПУ ===
  const [touchStart, setTouchStart] = useState(null);
  const [touchEnd, setTouchEnd] = useState(null);
  const minSwipeDistance = 50;

  const nextSlide = () => {
    setCurrentSlide((prev) => (prev + 1) % slides.length);
  };

  const prevSlide = () => {
    setCurrentSlide((prev) => (prev === 0 ? slides.length - 1 : prev - 1));
  };

  // Автоплей (опціонально)
  useEffect(() => {
    const interval = setInterval(nextSlide, 5000);
    return () => clearInterval(interval);
  }, []);

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

    if (isLeftSwipe) nextSlide();
    if (isRightSwipe) prevSlide();
  };

  return (
    <div
      className="hero-container"
      // Додаємо події на контейнер
      onTouchStart={onTouchStart}
      onTouchMove={onTouchMove}
      onTouchEnd={onTouchEnd}>
      {slides.map((slide, index) => (
        <div
          key={slide.id}
          className={`hero-slide ${index === currentSlide ? "active" : ""}`}>
          <img src={slide.image} alt="Flowers" className="hero-img" />
        </div>
      ))}

      {/* Контент поверх слайдів */}
      <div className="hero-content">
        <div className="hero-logo-text">
          <img src={logo} alt="FLOWER LAB VLADA" className="hero-logo" />
        </div>

        <button className="hero-cta-btn" onClick={() => navigate("/catalog")}>
          ORDER
        </button>
      </div>

      {/* Стрілки навігації */}
      <button className="hero-arrow left" onClick={prevSlide}>
        <img src={arrowLeft} alt="arrow-left" />
      </button>
      <button className="hero-arrow right" onClick={nextSlide}>
        <img src={arrowRight} alt="arrow-right" />
      </button>
    </div>
  );
};

export default HeroSection;
