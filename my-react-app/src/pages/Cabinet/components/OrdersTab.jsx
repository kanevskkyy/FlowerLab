import React, { useEffect, useRef } from "react";
import { useOrders } from "../hooks/useOrders";
import OrderIcon from "../../../assets/icons/orders.svg";
import OrderCard from "./OrderCard";

export default function OrdersTab({ activeTab, TABS }) {
  const { orders, ordersLoading, hasNextPage, loadMore, isLoadingMore } =
    useOrders(activeTab, TABS);

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
        <h1 className="cabinet-title">Order history</h1>

        <div className="orders-sort">
          <span className="orders-sort-label">SORT BY</span>
          <select className="orders-sort-select">
            <option>Date</option>
            <option>Status</option>
            <option>Total</option>
          </select>
        </div>
      </div>

      <div className="orders-list">
        {!ordersLoading && orders.length === 0 && (
          <div className="orders-empty">
            <img src={OrderIcon} className="orders-empty-icon" alt="" />
            <span>You haven't placed any orders yet.</span>
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
                Loading more...
              </span>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
