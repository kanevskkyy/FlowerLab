import React from "react";
import trashIco from "../../../assets/icons/trash.svg";

const Stars = ({ value }) => {
  return (
    <div className="review-stars">
      {Array.from({ length: 5 }).map((_, i) => (
        <span key={i} className={`star ${i < value ? "on" : "off"}`}>
          ★
        </span>
      ))}
    </div>
  );
};

function AdminReviewsList({ reviews, onPost, onDelete }) {
  return (
    <section className="admin-section admin-reviews">
      <h2 className="admin-section-title">Pending reviews</h2>
      <div className="admin-reviews-list">
        {reviews.map((r) => (
          <div key={r.id} className="review-card">
            <div className="review-left">
              <div className="review-avatar">
                <img src={r.avatar} alt="" />
              </div>
              <div className="review-body">
                <div className="review-name">{r.name}</div>
                <Stars value={r.stars} />
                <div className="review-text">{r.text}</div>
              </div>
            </div>
            <div className="review-actions">
              <div className="review-actions-head">
                <div className="review-action-label">Post</div>
                <div className="review-action-label">Delete</div>
              </div>
              <div className="review-actions-btns">
                <button
                  type="button"
                  className="review-icon-btn ok"
                  onClick={() => onPost(r.id)}
                >
                  ✓
                </button>
                <button
                  type="button"
                  className="review-icon-btn del"
                  onClick={() => onDelete(r.id)}
                >
                  <img src={trashIco} alt="" />
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}

export default AdminReviewsList;
