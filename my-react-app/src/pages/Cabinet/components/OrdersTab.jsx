import React from "react";
import { useOrders } from "../hooks/useOrders";
import OrderIcon from "../../../assets/icons/orders.svg";

export default function OrdersTab({ activeTab, TABS }) {
  const { orders, ordersLoading } = useOrders(activeTab, TABS);

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
          <div key={order.id} className="order-card">
            <div className="order-meta">
              <span className="order-meta-id">{order.id}</span>{" "}
              <span className="order-meta-date">{order.date}</span>
            </div>

            {order.type === "single" && order.items?.[0] && (
              <div className="order-single">
                <div className="order-single-img">
                  <img src={order.items[0].img} alt="" />
                </div>

                <div>
                  <div className="order-single-title">
                    {order.items[0].title}
                  </div>
                  <div className="order-single-qty">{order.items[0].qty}</div>
                </div>

                <div className="order-single-right">
                  <div className="order-status">
                    Status:{" "}
                    <span className="order-status-value">{order.status}</span>
                  </div>
                  <div className="order-total">
                    Order Total: {order.total} {order.currency}
                  </div>
                </div>
              </div>
            )}

            {order.type === "multi" && (
              <>
                <div className="order-multi-grid">
                  {order.items?.map((item, i) => (
                    <div key={i}>
                      <div className="order-item-img">
                        {item?.img && <img src={item.img} alt="" />}
                      </div>
                      <div className="order-item-title">{item?.title}</div>
                      <div className="order-item-bottom">
                        <span>{item?.price} ₴</span>
                        <span>{item?.qty}</span>
                      </div>
                    </div>
                  ))}
                </div>

                <div className="order-multi-footer">
                  <div className="order-total">
                    Order Total: {order.total} {order.currency}
                  </div>
                  <div className="order-status">
                    Status:{" "}
                    <span className="order-status-value">{order.status}</span>
                  </div>
                </div>
              </>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
