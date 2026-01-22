import React, { useEffect, useRef } from "react";
import AdminOrdersSort from "./AdminOrdersSort";

function AdminOrdersList({
  orders,
  statuses,
  sort,
  setSort,
  onStatusChange,
  onOrderClick,
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
    <section className="admin-section admin-orders">
      <h2 className="admin-section-title admin-orders-title">Orders</h2>
      <div className="admin-orders-top">
        <div />
        <div className="admin-orders-sort">
          <AdminOrdersSort sort={sort} onSortChange={setSort} />
        </div>
      </div>
      <div className="admin-orders-list">
        {orders.map((o) => {
          const statusName = o.status?.name || o.status || "Unknown";
          const statusId = o.status?.id || "";
          return (
            <div
              key={o.id}
              className="admin-order-card"
              onClick={() => onOrderClick(o.id)}>
              <div className="admin-order-left">
                <div className="admin-order-avatar">
                  {o.avatar && <img src={o.avatar} alt="" />}
                </div>
                <div className="admin-order-mid">
                  <div className="admin-order-title">{o.title}</div>
                  <div className="admin-order-sub">
                    <span className="admin-order-name">{o.customer}</span>
                    <span className="admin-order-date">{o.date}</span>
                  </div>
                </div>
              </div>
              <div className="admin-order-status">
                <select
                  className={`status-select status-${statusName.toLowerCase()}`}
                  value={statusId}
                  onClick={(e) => e.stopPropagation()}
                  onChange={(e) => onStatusChange(o.id, e.target.value)}>
                  {(statuses || []).map((s) => (
                    <option key={s.id} value={s.id}>
                      {s.name}
                    </option>
                  ))}
                </select>
              </div>
              <div className="admin-order-right">
                <div className="admin-order-total">
                  <p className="admin-order-total-title">Order Total:</p>
                  <p className="admin-order-total-value">{o.total}</p>
                </div>
              </div>
            </div>
          );
        })}

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

export default AdminOrdersList;
