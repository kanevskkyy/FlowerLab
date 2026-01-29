import React from "react";
import { useTranslation } from "react-i18next";
import "./OrderCard.css";
import CardIcon from "../../../assets/icons/message.svg";
import { getLocalizedValue } from "../../../utils/localizationUtils";

const OrderCard = ({ order }) => {
  const { t, i18n } = useTranslation();

  return (
    <div className="history-card">
      {/* HEADER: ID and Date */}
      <div className="history-header">
        <span className="history-id">{order.id}</span>
        <span className="history-date">
          {i18n.language === "ua" ? "о" : "at"} {order.date}
        </span>
      </div>

      {/* CONTENT: Grid of items */}
      <div className="history-items-grid">
        {order.items.map((item, idx) => (
          <div key={`${item.id}-${idx}`} className="history-grid-item">
            <div className="grid-img-wrapper">
              {item.img && (
                <img src={item.img} alt={item.title} loading="lazy" />
              )}
            </div>
            <div className="grid-item-info">
              <span className="grid-title">{item.title}</span>
              {item.size && (
                <span className="grid-size">
                  {t("product.size")}: {item.size}
                </span>
              )}
              <div className="grid-price-row">
                <span className="grid-price">
                  {item.price} {order.currency}
                </span>
                <span className="grid-qty">{item.qty}</span>
              </div>
            </div>
          </div>
        ))}

        {/* Postcard Item */}
        {order.cardText && (
          <div className="history-grid-item card-item">
            <div className="grid-card-wrapper">
              <img src={CardIcon} alt="Card" className="card-icon-svg" />
              <span className="card-label">
                {t("admin.orders.postcard").toUpperCase()}
              </span>
            </div>
            <div className="grid-card-text">"{order.cardText}"</div>
          </div>
        )}
      </div>

      {/* FOOTER: Total and Status */}
      <div className="history-footer">
        <div className="history-total">
          {t("admin.orders.total_label")} {order.total} {order.currency}
        </div>
        <div className="history-status">
          {t("admin.orders.status")}{" "}
          <span
            className={`status-badge ${(() => {
              const statusObj = order.status;
              const name =
                typeof statusObj === "object" ? statusObj.name : statusObj;
              return (name || "").replace(/\s/g, "").toLowerCase();
            })()}`}>
            {(() => {
              const statusObj = order.status;
              const name =
                typeof statusObj === "string" ? statusObj : statusObj?.name;
              const translations =
                statusObj?.translations || statusObj?.Translations;

              const localized = getLocalizedValue(translations, i18n.language);

              // Priority 1: Backend localized value (if truly different)
              if (localized && localized !== name) return localized;

              // Priority 2: Local JSON translation
              const statusKey = (localized || name)
                ?.replace(/\s/g, "")
                .toLowerCase();
              const fromJson = t(`order_status.${statusKey}`);

              if (fromJson && fromJson !== `order_status.${statusKey}`) {
                return fromJson;
              }

              // Priority 3: Backend raw value
              return localized || name;
            })()}
          </span>
        </div>
      </div>
    </div>
  );
};

export default OrderCard;
