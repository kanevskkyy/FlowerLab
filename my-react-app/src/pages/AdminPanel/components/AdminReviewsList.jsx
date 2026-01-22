import React, { useEffect, useRef } from "react";
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

function AdminReviewsList({
  reviews,
  onPost,
  onDelete,
  loadMore,
  hasNextPage,
  isLoadingMore,
}) {
  const sentinelRef = useRef(null);

  useEffect(() => {
    if (!hasNextPage) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          loadMore();
        }
      },
      { rootMargin: "100px" },
    );

    if (sentinelRef.current) {
      observer.observe(sentinelRef.current);
    }

    return () => {
      if (sentinelRef.current) {
        observer.unobserve(sentinelRef.current);
      }
    };
  }, [loadMore, hasNextPage]);

  return (
    <section className="admin-section admin-reviews">
      <h2 className="admin-section-title">Pending reviews</h2>
      <div className="admin-reviews-list">
        {reviews.length === 0 && !isLoadingMore ? (
          <div style={{ padding: "2rem", textAlign: "center", color: "#888" }}>
            No pending reviews
          </div>
        ) : (
          reviews.map((r) => (
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
                    onClick={() => onPost(r.id)}>
                    ✓
                  </button>
                  <button
                    type="button"
                    className="review-icon-btn del"
                    onClick={() => onDelete(r.id)}>
                    <img src={trashIco} alt="" />
                  </button>
                </div>
              </div>
            </div>
          ))
        )}

        {/* Sentinel for Infinite Scroll */}
        {hasNextPage && (
          <div
            ref={sentinelRef}
            style={{ height: "20px", textAlign: "center", color: "#666" }}>
            {isLoadingMore ? "Loading more..." : ""}
          </div>
        )}
      </div>
    </section>
  );
}

export default AdminReviewsList;
