import React, { useEffect, useRef } from "react";
import { useTranslation } from "react-i18next";
import { useOrders } from "../hooks/useOrders";
import OrderIcon from "../../../assets/icons/orders.svg";
import OrderCard from "./OrderCard";

export default function OrdersTab({ activeTab, TABS }) {
  const { t } = useTranslation();
  const {
    orders,
    ordersLoading,
    hasNextPage,
    loadMore,
    isLoadingMore,
    sort,
    setSort,
  } = useOrders(activeTab, TABS);

  const sentinelRef = useRef(null);

  useEffect(() => {
    if (!hasNextPage) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && !ordersLoading && !isLoadingMore) {
          loadMore();
        }
      },
      { rootMargin: "100px" }, // Project standard rootMargin
    );

    if (sentinelRef.current) {
      observer.observe(sentinelRef.current);
    }

    return () => {
      if (sentinelRef.current) {
        observer.unobserve(sentinelRef.current);
      }
    };
  }, [hasNextPage, ordersLoading, isLoadingMore, loadMore]);

  return (
    <div className="cabinet-panel-inner cabinet-orders">
      <div className="orders-top">
        <h1 className="cabinet-title">{t("cabinet.history_title")}</h1>

        <div className="orders-sort">
          <span className="orders-sort-label">{t("cabinet.sort_by")}</span>
          <select
            className="orders-sort-select"
            value={sort}
            onChange={(e) => setSort(e.target.value)}>
            <option value="DateDesc">{t("cabinet.date_desc")}</option>
            <option value="DateAsc">{t("cabinet.date_asc")}</option>
            <option value="TotalDesc">{t("cabinet.total_desc")}</option>
            <option value="TotalAsc">{t("cabinet.total_asc")}</option>
            <option value="StatusAsc">{t("cabinet.status")}</option>
          </select>
        </div>
      </div>

      <div className="orders-list">
        {!ordersLoading && orders.length === 0 && (
          <div className="orders-empty">
            <img src={OrderIcon} className="orders-empty-icon" alt="" />
            <span>{t("cabinet.empty_orders")}</span>
          </div>
        )}

        {orders.map((order) => (
          <OrderCard key={order.rawId} order={order} />
        ))}

        {/* Sentinel for Infinite Scroll (Standard Pattern) */}
        {hasNextPage && (
          <div ref={sentinelRef} className="orders-loading-indicator">
            <div className="ios-spinner"></div>
            {isLoadingMore && (
              <span
                style={{ marginLeft: "10px", fontSize: "12px", color: "#666" }}>
                {t("cabinet.loading_more")}
              </span>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
