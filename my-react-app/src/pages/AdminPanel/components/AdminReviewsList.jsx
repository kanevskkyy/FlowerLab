import React, { useEffect, useRef } from "react";
import { useTranslation } from "react-i18next";
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
  onApprove,
  onDelete,
  loadMore,
  hasNextPage,
  isLoadingMore,
}) {
  const { t } = useTranslation();
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
      <h2 className="admin-section-title">
        {t("admin.reviews.pending_title")}
      </h2>
      <div className="admin-reviews-list">
        {reviews.length === 0 && !isLoadingMore ? (
          <div style={{ padding: "2rem", textAlign: "center", color: "#888" }}>
            {t("admin.reviews.no_pending")}
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
                  <div className="review-action-label">
                    {t("admin.reviews.post")}
                  </div>
                  <div className="review-action-label">{t("admin.delete")}</div>
                </div>
                <div className="review-actions-btns">
                  <button
                    type="button"
                    className="review-icon-btn ok"
                    onClick={() => onApprove(r.id)}>
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
            {isLoadingMore ? t("admin.loading") : ""}
          </div>
        )}
      </div>
    </section>
  );
}

export default AdminReviewsList;
