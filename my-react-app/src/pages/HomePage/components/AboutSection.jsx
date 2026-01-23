import bouquet4 from "../../../assets/images/about-image.webp";

function AboutSection() {
  return (
    <section className="about-section">
      <h2 className="section-title">ABOUT US</h2>
      <div className="about-content">
        <div className="about-text">
          <p>
            Welcome to our flower shop — a place where every bouquet tells a
            story. We believe that flowers are more than just a gift; they are a
            way to express emotions, share happiness, and make every moment
            memorable.
          </p>
          <p>
            Our team carefully selects fresh flowers every day to ensure the
            highest quality and beauty in every arrangement. Whether it’s a
            romantic gesture, a celebration, or just a small token of
            appreciation, we create bouquets that speak from the heart.
          </p>
          <p>
            We take pride in our attention to detail, creative designs, and
            friendly service. Each bouquet is handcrafted with love, tailored to
            fit your style and occasion.
          </p>
        </div>
        <div className="about-image">
          <img
            src={bouquet4}
            alt="About banner"
            className="about-img"
            loading="lazy"
          />
        </div>
      </div>
    </section>
  );
}

export default AboutSection;
