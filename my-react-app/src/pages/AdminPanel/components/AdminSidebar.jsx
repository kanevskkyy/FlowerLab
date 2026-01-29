import React from "react";
import { useTranslation } from "react-i18next";
import bouquetsIco from "../../../assets/icons/flowerr.svg";
import ordersIco from "../../../assets/icons/orders.svg";
import reviewsIco from "../../../assets/icons/review.svg";

function AdminSidebar({ active, setActive, isCatalogOpen, setIsCatalogOpen }) {
  const { t } = useTranslation();
  const NAV = [
    { key: "orders", label: t("admin.sidebar.orders"), icon: ordersIco },
    { key: "reviews", label: t("admin.sidebar.reviews"), icon: reviewsIco },
  ];

  return (
    <aside className="admin-side">
      <nav className="admin-nav">
        {/* CATALOG DROPDOWN */}
        <div className="nav-group">
          <button
            className={`admin-nav-item ${
              active === "bouquets" ||
              active === "gifts" ||
              active === "catalog"
                ? "active-parent"
                : ""
            }`}
            onClick={() => setIsCatalogOpen(!isCatalogOpen)}>
            <img className="admin-nav-ico" src={bouquetsIco} alt="" />
            <span style={{ flex: 1 }}>{t("admin.sidebar.catalog")}</span>
            {/* Стрілочка (клас у CSS) */}
            <span className={`nav-arrow ${isCatalogOpen ? "open" : ""}`}>
              ›
            </span>
          </button>

          {isCatalogOpen && (
            <div className="admin-submenu">
              <button
                className={`admin-sub-item ${
                  active === "bouquets" ? "active" : ""
                }`}
                onClick={() => {
                  setActive("bouquets");
                  setIsCatalogOpen(false);
                }}>
                {t("admin.sidebar.bouquets")}
              </button>
              <button
                className={`admin-sub-item ${
                  active === "gifts" ? "active" : ""
                }`}
                onClick={() => {
                  setActive("gifts");
                  setIsCatalogOpen(false);
                }}>
                {t("admin.sidebar.gifts")}
              </button>
              <button
                className={`admin-sub-item ${
                  active === "catalog" ? "active" : ""
                }`}
                onClick={() => {
                  setActive("catalog");
                  setIsCatalogOpen(false);
                }}>
                {t("admin.sidebar.catalog_settings")}
              </button>
            </div>
          )}
        </div>

        {/* OTHER ITEMS */}
        {NAV.map((item) => (
          <button
            key={item.key}
            type="button"
            className={`admin-nav-item ${active === item.key ? "active" : ""}`}
            onClick={() => setActive(item.key)}>
            <img className="admin-nav-ico" src={item.icon} alt="" />
            <span>{item.label}</span>
          </button>
        ))}
      </nav>
    </aside>
  );
}

export default AdminSidebar;
