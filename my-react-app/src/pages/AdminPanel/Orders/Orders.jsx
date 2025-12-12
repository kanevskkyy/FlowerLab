// src/pages/AdminPanel/Orders/Orders.jsx
import './Orders.css';

const mockOrders = Array.from({ length: 5 }).map((_, i) => ({
  id: i + 1,
  customerName: 'Name Surname',
  bouquetName: 'Bouquet name',
  quantity: '1 pc',
  dateTime: 'at 10:06:10 25.10.25',
  total: '1000 ₴',
}));

export default function Orders() {
  return (
    <div className="orders-page">
      {/* Заголовок + сортировка */}
      <div className="orders-header-row">
        <h2 className="orders-title">Orders</h2>

        <button className="orders-sort-btn">
          SORT BY <span className="arrow">↑</span>
        </button>
      </div>

      {/* Список замовлень */}
      <div className="orders-list">
        {mockOrders.map((order) => (
          <OrderCard key={order.id} order={order} />
        ))}
      </div>
    </div>
  );
}

function OrderCard({ order }) {
  return (
    <div className="order-card">
      {/* аватар зліва */}
      <div className="order-avatar" />

      {/* центр */}
      <div className="order-main">
        <div className="order-row-top">
          <span className="order-name">{order.customerName}</span>
          <span className="order-datetime">{order.dateTime}</span>
        </div>

        <div className="order-row-bottom">
          <div className="order-left-info">
            <div className="order-bouquet">{order.bouquetName}</div>
            <div className="order-qty">{order.quantity}</div>
          </div>

          <div className="order-total">
            <span className="order-total-label">Order Total:</span>
            <span className="order-total-value">{order.total}</span>
          </div>
        </div>
      </div>
    </div>
  );
}
