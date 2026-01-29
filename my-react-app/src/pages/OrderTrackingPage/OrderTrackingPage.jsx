import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useSearchParams, useNavigate } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../../components/PopupMenu/PopupMenu";
import orderService from "../../services/orderService";
import { useAuth } from "../../context/useAuth";
import toast from "react-hot-toast";
import CardIcon from "../../assets/icons/message.svg";
import "./OrderTrackingPage.css";

const OrderTrackingPage = () => {
  const { t } = useTranslation();
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();
  const { user } = useAuth();

  // State for menu
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  // State for order data
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // URL params
  const urlOrderId = searchParams.get("orderId");
  const urlToken = searchParams.get("token") || searchParams.get("guestToken");

  useEffect(() => {
    const fetchHistory = async () => {
      setLoading(true);
      setError(null);

      try {
        // 1. Get all guest orders from localStorage
        const stored = JSON.parse(localStorage.getItem("guestOrders") || "[]");

        // 2. Add pending order if not present (legacy/direct redirect support)
        const pendingId = localStorage.getItem("pendingOrder");
        const pendingToken = localStorage.getItem("pendingGuestToken");
        if (
          pendingId &&
          pendingToken &&
          !stored.some((o) => o.orderId === pendingId)
        ) {
          stored.push({ orderId: pendingId, guestToken: pendingToken });
        }

        // 3. Handle URL params (highest priority, ensure it's in the list)
        if (
          urlOrderId &&
          urlToken &&
          !stored.some((o) => o.orderId === urlOrderId)
        ) {
          stored.unshift({ orderId: urlOrderId, guestToken: urlToken });
        }

        if (stored.length === 0) {
          setLoading(false);
          return;
        }

        // 4. Fetch all in parallel
        const results = await Promise.all(
          stored.map(async (o) => {
            try {
              return await orderService.getById(o.orderId, o.guestToken);
            } catch (err) {
              console.error(`Failed to fetch order ${o.orderId}:`, err);
              return null;
            }
          }),
        );

        const validOrders = results
          .filter(Boolean)
          .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));

        setOrders(validOrders);
        if (validOrders.length === 0 && stored.length > 0) {
          setError(t("tracking.find_error"));
        }
      } catch (err) {
        console.error("History Fetch Error:", err);
        setError(t("tracking.fetch_error"));
      } finally {
        setLoading(false);
      }
    };

    fetchHistory();
  }, [urlOrderId, urlToken]);

  // Helper to format date
  const formatDate = (dateString) => {
    if (!dateString) return "";
    return new Date(dateString).toLocaleString("uk-UA", {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  return (
    <div className="page-wrapper tracking-page">
      <Header onMenuOpen={() => setIsMenuOpen(true)} />
      <PopupMenu isOpen={isMenuOpen} onClose={() => setIsMenuOpen(false)} />
      <main className="tracking-content">
        <h1 className="tracking-title">{t("tracking.title")}</h1>
        {user && (
          <div className="tracking-card" style={{ marginBottom: "20px" }}>
            <p>
              Welcome, <b>{user.firstName}</b>! You can view your full history
              in the cabinet.
            </p>
            <button
              className="tracking-submit-btn"
              style={{ backgroundColor: "#d46aac", marginTop: "10px" }}
              onClick={() => navigate("/cabinet")}>
              Go to My Orders
            </button>
          </div>
        )}
        {error && (
          <div className="tracking-card">
            <div className="tracking-error">{error}</div>
          </div>
        )}
        {loading && orders.length === 0 && (
          <div className="tracking-card">
            <div className="ios-spinner"></div>
            <p>{t("tracking.searching")}</p>
          </div>
        )}
        {/* LIST OF ORDERS */}
        <div
          className="orders-list"
          style={{ display: "flex", flexDirection: "column", gap: "20px" }}>
          {orders.map((order) => (
            <div key={order.id} className="history-card">
              {/* HEADER: ID and Date */}
              <div className="history-header">
                <span className="history-id">
                  №{order.id.substring(0, 8).toUpperCase()}
                </span>
                <span className="history-date">
                  {t("tracking.at")}{" "}
                  {new Date(order.createdAt).toLocaleString("uk-UA", {
                    hour: "2-digit",
                    minute: "2-digit",
                    day: "numeric",
                    month: "numeric",
                    year: "numeric",
                  })}
                </span>
              </div>

              {/* CONTENT: Grid of items */}
              <div className="history-items-grid">
                {/* Bouquets */}
                {(order.items || []).map((item, idx) => (
                  <div key={`b-${idx}`} className="history-grid-item">
                    <div className="grid-img-wrapper">
                      {item.bouquetImage && (
                        <img
                          src={item.bouquetImage}
                          alt={item.bouquetName}
                          loading="lazy"
                        />
                      )}
                    </div>
                    <div className="grid-item-info">
                      <span className="grid-title">{item.bouquetName}</span>
                      {item.sizeName && (
                        <span className="grid-size">
                          {t("tracking.size")}: {item.sizeName}
                        </span>
                      )}
                      <div className="grid-price-row">
                        <span className="grid-price">{item.price} ₴</span>
                        <span className="grid-qty">
                          {item.count} {t("tracking.pc")}
                        </span>
                      </div>
                    </div>
                  </div>
                ))}

                {/* Gifts */}
                {(order.orderGifts || order.gifts || []).map((g, idx) => {
                  const gift = g.gift;
                  return (
                    <div key={`g-${idx}`} className="history-grid-item">
                      <div className="grid-img-wrapper">
                        {(gift?.imageUrls?.[0] || gift?.imageUrl) && (
                          <img
                            src={gift.imageUrls?.[0] || gift.imageUrl}
                            alt={gift.name}
                            loading="lazy"
                          />
                        )}
                      </div>
                      <div className="grid-item-info">
                        <span className="grid-title">
                          {gift?.name || t("tracking.gift")}
                        </span>
                        <div className="grid-price-row">
                          <span className="grid-price">
                            {gift?.price || 0} ₴
                          </span>
                          <span className="grid-qty">
                            {g.orderedCount || g.count} {t("tracking.pc")}
                          </span>
                        </div>
                      </div>
                    </div>
                  );
                })}

                {/* Postcard Item */}
                {order.giftMessage && (
                  <div className="history-grid-item card-item">
                    <div className="grid-card-wrapper">
                      <img
                        src={CardIcon}
                        alt="Card"
                        className="card-icon-svg"
                      />
                      <span className="card-label">
                        {t("tracking.postcard")}
                      </span>
                    </div>
                    <div className="grid-card-text">"{order.giftMessage}"</div>
                  </div>
                )}
              </div>

              {/* FOOTER: Total and Status */}
              <div className="history-footer">
                <div className="history-total">
                  {t("tracking.total")}: {order.totalPrice} ₴
                </div>
                <div className="history-status">
                  {t("tracking.status")}:{" "}
                  <span
                    className={`status-badge ${(
                      order.status?.name ||
                      order.status ||
                      ""
                    ).toLowerCase()}`}>
                    {order.status?.name || order.status}
                  </span>
                </div>
              </div>
            </div>
          ))}
        </div>
        {/* Empty state */}
        {!loading && orders.length === 0 && (
          <div className="tracking-card">
            <p>{t("tracking.no_orders")}</p>
            <p style={{ marginTop: "10px", fontSize: "0.9rem", color: "#666" }}>
              {t("tracking.guest_only")}
            </p>
          </div>
        )}
      </main>
      <Footer />
    </div>
  );
};

export default OrderTrackingPage;
