import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import toast from "react-hot-toast";
import orderService from "../../services/orderService";
import { extractErrorMessage } from "../../utils/errorUtils";
import {
  getLocalizedValue,
  getLocalizedStatus,
  getStatusKey,
} from "../../utils/localizationUtils";
import "./AdminOrderDetails.css";

export default function AdminOrderDetails() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { t, i18n } = useTranslation();
  const [order, setOrder] = useState(null);
  const [statuses, setStatuses] = useState([]); // List of available statuses
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [orderData, statusesData] = await Promise.all([
          orderService.getById(id),
          orderService.getStatuses(),
        ]);
        setOrder(orderData);
        setStatuses(statusesData);
      } catch (error) {
        console.error("Failed to load order:", error);
        toast.error(t(extractErrorMessage(error, "admin.orders.failed_load")));
        navigate("/admin");
      } finally {
        setLoading(false);
      }
    };
    if (id) fetchData();
  }, [id, navigate]);

  const handleStatusChange = async (newStatusId) => {
    const originalStatus = order.status;
    const targetStatusObj = statuses.find((s) => s.id === newStatusId);

    // If not found or same, ignore
    if (
      !targetStatusObj ||
      (originalStatus && originalStatus.id === newStatusId)
    )
      return;

    // Optimistic
    setOrder((prev) => ({ ...prev, status: targetStatusObj }));

    try {
      await orderService.updateStatus(id, newStatusId);

      if (targetStatusObj.name === "Cancelled") {
        toast.error(t("toasts.admin_order_cancelled"));
      } else if (targetStatusObj.name === "Delivered") {
        toast.success(t("toasts.admin_order_delivered"));
      } else {
        const localizedStatusName = getLocalizedStatus(
          targetStatusObj,
          i18n.language,
          t,
        );
        toast.success(
          t("toasts.admin_status_updated", { status: localizedStatusName }),
        );
      }
    } catch (error) {
      console.error("Failed to update status:", error);
      toast.error(
        t(extractErrorMessage(error, "toasts.admin_status_update_failed")),
      );
      setOrder((prev) => ({ ...prev, status: originalStatus }));
    }
  };

  if (loading)
    return (
      <div className="aod-loading">{t("admin.orders.loading_details")}</div>
    );
  if (!order)
    return <div className="aod-loading">{t("admin.orders.not_found")}</div>;

  // order.status is likely an object { id, name } now from backend
  const currentStatusId = order.status?.id || "";
  const currentStatusName = order.status?.name || "Unknown";

  return (
    <div className="aod-page">
      <div className="aod-container">
        {/* HEADER */}
        <header className="aod-header">
          <div className="aod-header-left">
            <button className="aod-back-btn" onClick={() => navigate("/admin")}>
              ← {t("admin.orders.back_to_orders")}
            </button>
            <h1 className="aod-title">
              {t("admin.orders.id")}
              {order.id.substring(0, 8).toUpperCase()}
            </h1>
            <span className="aod-date">
              {new Date(
                order.status?.createdAt || order.createdAt || Date.now(),
              ).toLocaleString("uk-UA", {
                year: "numeric",
                month: "numeric",
                day: "numeric",
                hour: "2-digit",
                minute: "2-digit",
              })}
            </span>
          </div>

          <div className="aod-status-wrapper">
            <label>{t("admin.orders.status")}</label>
            <select
              className={`aod-status-select status-${getStatusKey(currentStatusName)}`}
              value={currentStatusId}
              onChange={(e) => handleStatusChange(e.target.value)}>
              {statuses.map((s) => (
                <option key={s.id} value={s.id}>
                  {getLocalizedStatus(s, i18n.language, t)}
                </option>
              ))}
            </select>
          </div>
        </header>

        <div className="aod-content">
          {/* LEFT COLUMN: ITEMS */}
          <div className="aod-main">
            {/* Products */}
            <section className="aod-card">
              <h2 className="aod-section-title">{t("admin.orders.items")}</h2>
              <div className="aod-items-list">
                {order.items?.map((item, idx) => (
                  <div key={`item-${idx}`} className="aod-item">
                    <div className="aod-item-img">
                      {item.bouquetImage && (
                        <img
                          src={item.bouquetImage}
                          alt=""
                          className="aod-item-img-tag"
                        />
                      )}
                    </div>
                    <div className="aod-item-info">
                      <div className="aod-item-name">
                        {getLocalizedValue(item.bouquetName, i18n.language) ||
                          "Unknown Bouquet"}
                      </div>
                      <div className="aod-item-meta">
                        {item.sizeName && (
                          <span>
                            {t("admin.orders.size", {
                              size: getLocalizedValue(
                                item.sizeName,
                                i18n.language,
                              ),
                            })}{" "}
                            |{" "}
                          </span>
                        )}
                        {t("admin.orders.qty", { count: item.count })}
                      </div>
                    </div>
                    <div className="aod-item-total-row">
                      {(item.price * item.count).toFixed(2)} ₴
                    </div>
                  </div>
                ))}

                {/* GIFTS */}
                {order.gifts?.map((giftItem, idx) => (
                  <div key={`gift-${idx}`} className="aod-item">
                    <div className="aod-item-img">
                      <img
                        src={giftItem.gift?.imageUrl || "/placeholder.png"}
                        alt=""
                        className="aod-item-img-tag"
                      />
                    </div>
                    <div className="aod-item-info">
                      <div className="aod-item-name">
                        {getLocalizedValue(
                          giftItem.gift?.name,
                          i18n.language,
                        ) || "Unknown Gift"}
                      </div>
                      <div className="aod-item-meta">
                        {t("admin.orders.qty", {
                          count: giftItem.orderedCount,
                        })}
                      </div>
                    </div>
                    <div className="aod-item-total-row">
                      {(
                        (giftItem.gift?.price || 0) *
                        (giftItem.orderedCount || 1)
                      ).toFixed(2)}{" "}
                      ₴
                    </div>
                  </div>
                ))}

                {/* POSTCARD (If gift message exists, we assume postcard was bought) */}
                {order.giftMessage && (
                  <div className="aod-item">
                    <div className="aod-item-img aod-item-placeholder">💌</div>
                    <div className="aod-item-info">
                      <div className="aod-item-name">
                        {t("admin.orders.postcard")}
                      </div>
                      <div className="aod-item-meta">
                        {t("admin.orders.postcard_msg")}
                      </div>
                    </div>
                    <div className="aod-item-total-row">50.00 ₴</div>
                  </div>
                )}
              </div>
            </section>

            {/* Comment */}
            {order.notes && (
              <section className="aod-card">
                <h2 className="aod-section-title">
                  {t("admin.orders.customer_comment")}
                </h2>
                <p className="aod-comment">"{order.notes}"</p>
              </section>
            )}

            {/* Postcard */}
            {order.postcardText && (
              <section className="aod-card">
                <h2 className="aod-section-title">
                  {t("admin.orders.postcard_text")}
                </h2>
                <p className="aod-comment">"{order.postcardText}"</p>
              </section>
            )}

            {/* Gift Message */}
            {order.giftMessage && (
              <section className="aod-card">
                <h2 className="aod-section-title">
                  {t("admin.orders.gift_message")}
                </h2>
                <p className="aod-comment">"{order.giftMessage}"</p>
              </section>
            )}
          </div>

          {/* RIGHT COLUMN: INFO */}
          <div className="aod-sidebar">
            {/* Customer Info */}
            <section className="aod-card">
              <h2 className="aod-section-title">
                {t("admin.orders.customer")}
              </h2>
              <div className="aod-info-row">
                <span className="label">{t("admin.orders.name")}</span>
                <span className="value">
                  {order.userFirstName || order.firstName}{" "}
                  {order.userLastName || order.lastName}
                </span>
              </div>
              <div className="aod-info-row">
                <span className="label">{t("admin.orders.phone")}</span>
                <span className="value">{order.phoneNumber}</span>
              </div>
            </section>

            {/* Recipient Info */}
            {order.receiverName && (
              <section className="aod-card">
                <h2 className="aod-section-title">
                  {t("admin.orders.recipient")}
                </h2>
                <div className="aod-info-row">
                  <span className="label">{t("admin.orders.name")}</span>
                  <span className="value">{order.receiverName}</span>
                </div>
                {order.receiverPhone && (
                  <div className="aod-info-row">
                    <span className="label">{t("admin.orders.phone")}</span>
                    <span className="value">{order.receiverPhone}</span>
                  </div>
                )}
              </section>
            )}

            {/* Delivery Info */}
            <section className="aod-card">
              <h2 className="aod-section-title">
                {t("admin.orders.delivery_details")}
              </h2>
              <div className="aod-info-block">
                <div className="label">{t("admin.orders.address")}</div>
                <div className="value">
                  {order.deliveryInformation?.address ||
                    order.pickupStoreAddress ||
                    "N/A"}
                </div>
              </div>
            </section>

            <section className="aod-card aod-summary">
              <h2 className="aod-section-title">
                {t("admin.orders.payment_summary")}
              </h2>

              <div className="aod-total-row">
                <span>{t("admin.orders.total")}</span>
                <span>{order.totalPrice} ₴</span>
              </div>
            </section>
          </div>
        </div>
      </div>
    </div>
  );
}
