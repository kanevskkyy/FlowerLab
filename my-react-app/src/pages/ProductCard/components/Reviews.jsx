import React, { useState } from "react";
import toast from "react-hot-toast";
import { useAuth } from "../../../context/useAuth";

import starIcon from "../../../assets/icons/star.svg";
import starUnfilledIcon from "../../../assets/icons/star-unfilled.svg";

const Reviews = ({ reviews, onAddReview }) => {
  const [visibleCount, setVisibleCount] = useState(3);
  const { user } = useAuth();

  const handleWriteReview = () => {
    if (!user) {
      toast.error(
        "Будь ласка, увійдіть або зареєструйтесь, щоб залишити відгук.",
      );
      return;
    }
    onAddReview();
  };

  const renderStars = (rating) => {
    return (
      <div className="pc-stars-container">
        {[1, 2, 3, 4, 5].map((star) => (
          <img
            key={star}
            src={star <= rating ? starIcon : starUnfilledIcon}
            alt={star <= rating ? "filled star" : "unfilled star"}
            className="pc-review-star-icon"
          />
        ))}
      </div>
    );
  };

  if (!reviews || reviews.length === 0) {
    return (
      <section className="pc-reviews-section">
        <h2 className="section-title">Reviews</h2>
        <div className="pc-review-button-wrapper">
          <button className="pc-write-review-btn" onClick={handleWriteReview}>
            Write a review
          </button>
        </div>
        <p className="no-reviews">
          No reviews yet. Be the first to share your thoughts!
        </p>
      </section>
    );
  }

  const handleShowMore = () => {
    if (visibleCount >= reviews.length) {
      setVisibleCount(3);
    } else {
      setVisibleCount(reviews.length);
    }
  };

  const visibleReviews = reviews.slice(0, visibleCount);
  const isExpanded = visibleCount >= reviews.length;

  return (
    <section className="pc-reviews-section">
      <h2 className="section-title">Reviews</h2>
      <div className="pc-review-button-wrapper">
        <button className="pc-write-review-btn" onClick={handleWriteReview}>
          Write a review
        </button>
      </div>

      <div className="pc-reviews-list">
        {visibleReviews.map((review) => (
          <div key={review.id} className="pc-review-card">
            <div className="pc-review-header">
              <div className="pc-review-avatar">
                {review.avatar ? (
                  <img src={review.avatar} alt={review.name} />
                ) : (
                  <div className="pc-avatar-placeholder">
                    {review.name.charAt(0).toUpperCase()}
                  </div>
                )}
              </div>
              <div className="pc-review-info">
                <p className="pc-review-name">{review.name}</p>
                <div className="pc-review-stars">
                  {renderStars(review.rating)}
                </div>
              </div>
            </div>
            <p className="pc-review-text">{review.text}</p>
          </div>
        ))}
      </div>

      <div className="pc-reviews-actions">
        {reviews.length > 3 && (
          <button className="pc-show-more-btn" onClick={handleShowMore}>
            {isExpanded ? "Show less" : "Show more reviews"}
          </button>
        )}
      </div>
    </section>
  );
};

export default Reviews;
