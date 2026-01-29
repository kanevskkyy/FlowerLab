import React, { useState } from "react";
import toast from "react-hot-toast";
import { useTranslation } from "react-i18next";
import reviewService from "../../../services/reviewService";
import starIcon from "../../../assets/icons/star.svg";
import starUnfilledIcon from "../../../assets/icons/star-unfilled.svg";
import "./AddReviewModal.css";

const AddReviewModal = ({ isOpen, onClose, bouquetId, onReviewSuccess }) => {
  const { t } = useTranslation();
  const [rating, setRating] = useState(0);
  const [comment, setComment] = useState("");
  const [hoverRating, setHoverRating] = useState(0);
  const [submitting, setSubmitting] = useState(false);

  if (!isOpen) return null;

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (rating === 0) {
      toast.error(t("product.review_rate_error"));
      return;
    }

    if (!comment.trim()) {
      toast.error(t("product.review_comment_error"));
      return;
    }

    setSubmitting(true);
    try {
      const reviewData = {
        bouquetId: bouquetId,
        rating: rating,
        comment: comment,
      };

      await reviewService.createReview(reviewData);
      toast.success(t("product.review_success"));
      onReviewSuccess();
      onClose();
      // Reset form
      setRating(0);
      setComment("");
    } catch (error) {
      console.error("Failed to submit review:", error);

      let errorMessage = t("product.review_generic_error");
      const errorResponse = error.response?.data;
      const errorString =
        typeof errorResponse === "string"
          ? errorResponse
          : JSON.stringify(errorResponse || {}).toLowerCase();

      if (
        errorString.includes("not ordered") ||
        errorString.includes("forbidden")
      ) {
        errorMessage = t("product.review_not_ordered");
      } else if (
        errorString.includes("already left a review") ||
        errorString.includes("already exists")
      ) {
        errorMessage = t("product.review_already_left");
      }

      toast.error(errorMessage);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="review-modal-overlay" onClick={onClose}>
      <div className="review-modal" onClick={(e) => e.stopPropagation()}>
        <button className="review-modal-close" onClick={onClose}>
          ×
        </button>

        <h3>{t("product.leave_review")}</h3>
        <p className="review-modal-subtitle">{t("product.review_subtitle")}</p>

        <form onSubmit={handleSubmit} className="review-form">
          <div className="star-rating-container">
            {[1, 2, 3, 4, 5].map((star) => {
              const isFilled = (hoverRating || rating) >= star;
              return (
                <img
                  key={star}
                  src={isFilled ? starIcon : starUnfilledIcon}
                  alt={`${star} Stars`}
                  className={`star-icon ${isFilled ? "filled" : "unfilled"}`}
                  onMouseEnter={() => setHoverRating(star)}
                  onMouseLeave={() => setHoverRating(0)}
                  onClick={() => setRating(star)}
                />
              );
            })}
          </div>

          <textarea
            className="review-textarea"
            placeholder={t("product.review_placeholder")}
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            rows={5}
          />

          <div className="review-modal-actions">
            <button
              type="button"
              className="modal-btn cancel-btn"
              onClick={onClose}
              disabled={submitting}>
              {t("product.cancel")}
            </button>
            <button
              type="submit"
              className="modal-btn submit-btn"
              disabled={submitting}>
              {submitting
                ? t("product.submitting")
                : t("product.submit_review")}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddReviewModal;
