import React from "react";
// Assuming ORDER_STATUSES is constant or passed as prop. 
// It was a constant in the parent file. I will define it here or accept it.
// To keep it simple, I'll define it here as it represents business logic for this view.

const ORDER_STATUSES = [
  "New",
  "Processing",
  "Shipped",
  "Delivered",
  "Cancelled",
];

function AdminOrdersList({ orders, sort, setSort, onStatusChange, onOrderClick }) {
  return (
    <section className="admin-section admin-orders">
      <h2 className="admin-section-title admin-orders-title">Orders</h2>
      <div className="admin-orders-top">
        <div />
        <div className="admin-orders-sort">
          <div className="admin-orders-sort-label">SORT BY</div>
          <select
            className="admin-orders-sort-select"
            value={sort}
            onChange={(e) => setSort(e.target.value)}
          >
            <option value="new">Date: New to old</option>
            <option value="old">Date: Old to new</option>
          </select>
        </div>
      </div>
      <div className="admin-orders-list">
        {orders.map((o) => (
          <div
            key={o.id}
            className="admin-order-card"
            onClick={() => onOrderClick(o.id)}
          >
            <div className="admin-order-left">
              <div className="admin-order-avatar">
                <img src={o.avatar} alt="" />
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
                className={`status-select status-${o.status.toLowerCase()}`}
                value={o.status}
                onClick={(e) => e.stopPropagation()}
                onChange={(e) => onStatusChange(o.id, e.target.value)}
              >
                {ORDER_STATUSES.map((s) => (
                  <option key={s} value={s}>
                    {s}
                  </option>
                ))}
              </select>
            </div>
            <div className="admin-order-right">
              <div className="admin-order-total-value">{o.total}</div>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}

export default AdminOrdersList;
