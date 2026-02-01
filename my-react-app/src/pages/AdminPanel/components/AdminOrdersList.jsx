import React, { useEffect, useRef } from "react";
import { useTranslation } from "react-i18next";
import AdminOrdersSort from "./AdminOrdersSort";
import {
  getLocalizedValue,
  getLocalizedStatus,
  getStatusKey,
} from "../../../utils/localizationUtils";
import searchIcon from "../../../assets/icons/search.svg";

function AdminOrdersList({
  orders,
  statuses,
  q,
  setQ,
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

  const { t, i18n } = useTranslation();

  return (
    <section className="admin-section admin-orders">
      <h2 className="admin-section-title admin-orders-title">
        {t("admin.orders.title")}
      </h2>
      <div className="admin-orders-top">
        <div className="admin-toolbar">
          <div className="admin-search">
            <img className="admin-search-ico" src={searchIcon} alt="" />
            <input
              type="text"
              placeholder={t("admin.orders.search_placeholder")}
              value={q}
              onChange={(e) => setQ(e.target.value)}
            />
          </div>
        </div>
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
                  {o.avatar ? (
                    <img src={o.avatar} alt="" />
                  ) : (
                    <div className="avatar-placeholder">
                      {o.customer?.charAt(0) || "U"}
                    </div>
                  )}
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
                  className={`status-select status-${getStatusKey(statusName)}`}
                  value={statusId}
                  onClick={(e) => e.stopPropagation()}
                  onChange={(e) => onStatusChange(o.id, e.target.value)}>
                  {(statuses || []).map((s) => (
                    <option key={s.id} value={s.id}>
                      {getLocalizedStatus(s, i18n.language, t)}
                    </option>
                  ))}
                </select>
              </div>
              <div className="admin-order-right">
                <div className="admin-order-total">
                  <p className="admin-order-total-title">
                    {t("admin.orders.total_label")}
                  </p>
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
            {isLoadingMore ? t("admin.loading_more") : ""}
          </div>
        )}
      </div>
    </section>
  );
}

export default AdminOrdersList;
