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
import { getLocalizedValue } from "../../utils/localizationUtils";
import "./OrderTrackingPage.css";

const OrderTrackingPage = () => {
  const { t, i18n } = useTranslation();
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

  // Helper to map order data for display
  const mapOrderData = (order) => {
    const bouquets = (order.items || []).map((item) => ({
      ...item,
      localizedName: getLocalizedValue(item.bouquetName, i18n.language),
      localizedSize: getLocalizedValue(item.sizeName, i18n.language),
      qtyLabel: `${item.count} ${t("tracking.pc")}`,
    }));

    const gifts = (order.orderGifts || order.gifts || []).map((g) => {
      const gift = g.gift;
      return {
        ...g,
        localizedName:
          getLocalizedValue(gift?.name, i18n.language) || t("tracking.gift"),
        qtyLabel: `${g.orderedCount || g.count} ${t("tracking.pc")}`,
        imageUrl: gift?.imageUrls?.[0] || gift?.imageUrl,
      };
    });

    const statusObj = order.status;
    const rawName = typeof statusObj === "object" ? statusObj.name : statusObj;
    const translations = statusObj?.translations || statusObj?.Translations;
    const backendLocalized = getLocalizedValue(translations, i18n.language);

    let displayStatus = backendLocalized || rawName;

    // Priority check for local JSON translation if backend didn't provide a specialized one
    if (!backendLocalized || backendLocalized === rawName) {
      const statusKey = (rawName || "").replace(/\s/g, "").toLowerCase();
      const fromJson = t(`order_status.${statusKey}`);
      if (fromJson && fromJson !== `order_status.${statusKey}`) {
        displayStatus = fromJson;
      }
    }

    const statusClass = (rawName || "").replace(/\s/g, "").toLowerCase();

    return {
      ...order,
      displayId: `№${order.id.substring(0, 8).toUpperCase()}`,
      displayDate: new Date(order.createdAt).toLocaleString(
        (i18n.language || "ua").toLowerCase().startsWith("u")
          ? "uk-UA"
          : "en-US",
        {
          hour: "2-digit",
          minute: "2-digit",
          day: "numeric",
          month: "numeric",
          year: "numeric",
        },
      ),
      displayStatus,
      statusClass,
      items: bouquets,
      giftsList: gifts,
    };
  };

  useEffect(() => {
    const fetchHistory = async () => {
      setLoading(true);
      setError(null);

      try {
        const stored = JSON.parse(localStorage.getItem("guestOrders") || "[]");
        const pendingId = localStorage.getItem("pendingOrder");
        const pendingToken = localStorage.getItem("pendingGuestToken");

        if (
          pendingId &&
          pendingToken &&
          !stored.some((o) => o.orderId === pendingId)
        ) {
          stored.push({ orderId: pendingId, guestToken: pendingToken });
        }

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

        const results = await Promise.all(
          stored.map(async (o) => {
            try {
              const res = await orderService.getById(o.orderId, o.guestToken);
              return res;
            } catch (err) {
              console.error(`Failed to fetch order ${o.orderId}:`, err);
              return null;
            }
          }),
        );

        const validOrders = results
          .filter(Boolean)
          .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));

        // Map data for display immediately
        setOrders(validOrders.map(mapOrderData));

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
  }, [urlOrderId, urlToken, i18n.language]); // Dependency on i18n.language ensures re-mapping

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
                <span className="history-id">{order.displayId}</span>
                <span className="history-date">
                  {i18n.language === "ua" ? "о" : "at"} {order.displayDate}
                </span>
              </div>

              {/* CONTENT: Grid of items */}
              <div className="history-items-grid">
                {/* Bouquets */}
                {order.items.map((item, idx) => (
                  <div key={`b-${idx}`} className="history-grid-item">
                    <div className="grid-img-wrapper">
                      {item.bouquetImage && (
                        <img
                          src={item.bouquetImage}
                          alt={item.localizedName}
                          loading="lazy"
                        />
                      )}
                    </div>
                    <div className="grid-item-info">
                      <span className="grid-title">{item.localizedName}</span>
                      {item.localizedSize && (
                        <span className="grid-size">
                          {t("product.size")}: {item.localizedSize}
                        </span>
                      )}
                      <div className="grid-price-row">
                        <span className="grid-price">{item.price} ₴</span>
                        <span className="grid-qty">{item.qtyLabel}</span>
                      </div>
                    </div>
                  </div>
                ))}

                {/* Gifts */}
                {order.giftsList.map((g, idx) => (
                  <div key={`g-${idx}`} className="history-grid-item">
                    <div className="grid-img-wrapper">
                      {g.imageUrl && (
                        <img
                          src={g.imageUrl}
                          alt={g.localizedName}
                          loading="lazy"
                        />
                      )}
                    </div>
                    <div className="grid-item-info">
                      <span className="grid-title">{g.localizedName}</span>
                      <div className="grid-price-row">
                        <span className="grid-price">
                          {g.gift?.price || 0} ₴
                        </span>
                        <span className="grid-qty">{g.qtyLabel}</span>
                      </div>
                    </div>
                  </div>
                ))}

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
                        {t("admin.orders.postcard").toUpperCase()}
                      </span>
                    </div>
                    <div className="grid-card-text">"{order.giftMessage}"</div>
                  </div>
                )}
              </div>

              {/* FOOTER: Total and Status */}
              <div className="history-footer">
                <div className="history-total">
                  {t("admin.orders.total_label")} {order.totalPrice} ₴
                </div>
                <div className="history-status">
                  {t("admin.orders.status")}{" "}
                  <span
                    className={`status-badge ${(() => {
                      const statusObj = order.status;
                      const name =
                        typeof statusObj === "object"
                          ? statusObj.name
                          : statusObj;
                      return (name || "").replace(/\s/g, "").toLowerCase();
                    })()}`}>
                    {(() => {
                      const statusObj = order.status;
                      const name =
                        typeof statusObj === "string"
                          ? statusObj
                          : statusObj?.name;
                      const translations =
                        statusObj?.translations || statusObj?.Translations;

                      const localized = getLocalizedValue(
                        translations,
                        i18n.language,
                      );

                      // Priority 1: Backend localized value (if truly different)
                      if (localized && localized !== name) return localized;

                      // Priority 2: Local JSON translation
                      const statusKey = (localized || name)
                        ?.replace(/\s/g, "")
                        .toLowerCase();
                      const fromJson = t(`order_status.${statusKey}`);

                      if (
                        fromJson &&
                        fromJson !== `order_status.${statusKey}`
                      ) {
                        return fromJson;
                      }

                      // Priority 3: Backend raw value
                      return localized || name;
                    })()}
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
