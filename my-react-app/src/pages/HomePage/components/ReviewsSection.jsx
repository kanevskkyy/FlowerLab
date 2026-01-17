import React, { useState } from "react";

function ReviewsSection({ reviews }) {
  const [currentReviewIdx, setCurrentReviewIdx] = useState(0);
  const [touchStart, setTouchStart] = useState(null);
  const [touchEnd, setTouchEnd] = useState(null);

  const minSwipeDistance = 50;

  const nextReview = () => {
    setCurrentReviewIdx((prev) => (prev + 1) % reviews.length);
  };

  const prevReview = () => {
    setCurrentReviewIdx((prev) =>
      prev === 0 ? reviews.length - 1 : prev - 1
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

    if (isLeftSwipe) nextReview();
    if (isRightSwipe) prevReview();
  };

  const getVisibleReviews = () => {
    const visible = [];
    for (let i = 0; i < 3; i++) {
      const index = (currentReviewIdx + i) % reviews.length;
      visible.push(reviews[index]);
    }
    return visible;
  };

  return (
    <section
      className="home-reviews-section"
      onTouchStart={onTouchStart}
      onTouchMove={onTouchMove}
      onTouchEnd={onTouchEnd}
    >
      <h2 className="section-title">REVIEWS</h2>
      <div className="home-reviews-wrapper">
        <button className="reviews-arrow" onClick={prevReview}>
          ‹
        </button>

        <div className="home-reviews-cards">
          {getVisibleReviews().map((review, index) => (
            <div
              key={`${review.id}-${index}`}
              className="home-review-card fade-in"
            >
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
  );
}

export default ReviewsSection;
