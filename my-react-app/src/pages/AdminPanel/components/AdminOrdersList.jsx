import React, { useEffect, useRef } from "react";
import { useTranslation } from "react-i18next";
import AdminOrdersSort from "./AdminOrdersSort";
import { getLocalizedValue } from "../../../utils/localizationUtils";

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

  const { t, i18n } = useTranslation();

  const getStatusKey = (name) => {
    if (!name) return "";
    return name.replace(/\s/g, "").toLowerCase();
  };

  const getLocalizedStatus = (statusObj) => {
    if (!statusObj) return "";
    const name = typeof statusObj === "string" ? statusObj : statusObj.name;
    const translations = statusObj.translations;

    const localized = getLocalizedValue(translations, i18n.language);

    // Priority 1: Backend localized value (if truly different)
    if (localized && localized !== name) return localized;

    // Priority 2: Local JSON translation
    const statusKey = getStatusKey(localized || name);
    const fromJson = t(`order_status.${statusKey}`);

    if (fromJson && fromJson !== `order_status.${statusKey}`) {
      return fromJson;
    }

    // Priority 3: Backend raw value
    return localized || name;
  };

  return (
    <section className="admin-section admin-orders">
      <h2 className="admin-section-title admin-orders-title">
        {t("admin.orders.title")}
      </h2>
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
                      {getLocalizedStatus(s)}
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
