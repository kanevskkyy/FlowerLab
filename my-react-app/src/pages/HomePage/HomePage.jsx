import { useState } from 'react';
import './HomePage.css';
import SideMenu from '../SideMenu/SideMenu'; // шлях підкоригуй, якщо інший

export default function HomePage() {
  const [language, setLanguage] = useState('en'); // 'en' або 'ua'
  const [isMenuOpen, setIsMenuOpen] = useState(false);

 

  return (
    <div className="home-page">
      {/* HEADER */}
      <header className="home-header">
       <div className="home-header-left">
  <button
    className="burger"
    onClick={() => setIsMenuOpen(true)}
    aria-label="Open menu"
  >
    <span></span>
    <span></span>
    <span></span>
  </button>

  {/* ENG/UA перемикач */}
  <div className="lang-switch">
    <button
      type="button"
      className={language === 'en' ? 'active' : ''}
      onClick={() => setLanguage('en')}
    >
      ENG
    </button>
    <span>/</span>
    <button
      type="button"
      className={language === 'ua' ? 'active' : ''}
      onClick={() => setLanguage('ua')}
    >
      UA
    </button>
  </div>
</div>

        <div className="home-header-center">[LOGO]</div>

        <div className="home-header-right">
          <span className="header-text">UAH/USD</span>
          <span className="icon">🛍</span>
          <span className="icon">👤 sign up/in</span>
        </div>
      </header>

      {/* MAIN */}
      <main className="home-main">
        {/* HERO / SLIDER */}
        <section className="hero-section">
          <button className="hero-arrow hero-arrow-left">‹</button>
          <div className="hero-banner" />
          <button className="hero-arrow hero-arrow-right">›</button>
        </section>

        {/* ORDER BUTTON */}
        <div className="order-wrapper">
          <button className="order-button">ORDER</button>
        </div>

        {/* POPULAR BOUQUETS / SALES */}
        <section className="popular-section">
          <h2 className="section-title">POPULAR BOUQUETS / SALES</h2>

          <div className="popular-cards">
            {['bouquet 1', 'bouquet 2', 'bouquet 3'].map((name) => (
              <div key={name} className="popular-card">
                <div className="popular-image" />
                <div className="popular-bottom">
                  <span className="popular-name">{name}</span>
                  <span className="lock-icon">🔒</span>
                </div>
              </div>
            ))}
          </div>
        </section>

        {/* ABOUT US */}
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
                highest quality and beauty in every arrangement. Whether it&apos;s a
                romantic gesture, a celebration, or just a small token of
                appreciation, we create bouquets that speak from the heart.
              </p>
              <p>
                We take pride in our attention to detail, creative designs, and
                friendly service. Each bouquet is handcrafted with love, tailored
                to fit your style and occasion.
              </p>
            </div>

            <div className="about-image" />
          </div>
        </section>

        {/* REVIEWS */}
        <section className="home-reviews-section">
          <h2 className="section-title">REVIEWS</h2>

          <div className="home-reviews-wrapper">
            <button className="reviews-arrow">‹</button>

            <div className="home-reviews-cards">
              {Array.from({ length: 3 }).map((_, i) => (
                <div key={i} className="home-review-card">
                  <div className="home-review-top">
                    <div className="home-review-avatar" />
                    <span className="home-review-name">[Name Surname]</span>
                  </div>

                  <div className="home-review-stars">
                    {'★★★★★'.split('').map((star, j) => (
                      <span key={j}>★</span>
                    ))}
                  </div>

                  <p className="home-review-text">
                    i really like the bouquet and recommend this store
                  </p>
                </div>
              ))}
            </div>

            <button className="reviews-arrow">›</button>
          </div>
        </section>
      </main>

      {/* FOOTER */}
      <footer className="home-footer">
        <div className="footer-item">
          <span className="icon">📍</span>
          <span>
            м. Чернівці, вул. Герцена 2а,
            <br />
            вул. Васіле Александрі, 1
          </span>
        </div>
        <div className="footer-item">
          <span className="icon">📞</span>
          <span>+38 050 159 19 12</span>
        </div>
        <div className="footer-item">
          <span className="icon">📷</span>
          <span>@flowerlab_vlada</span>
        </div>
      </footer>

      {/* SIDE MENU OVERLAY */}
      {isMenuOpen && (
        <div className="side-menu-overlay">
          <SideMenu
            language={language}
            onClose={() => setIsMenuOpen(false)}
          />
        </div>
      )}
    </div>
  );
}
