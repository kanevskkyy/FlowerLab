import React from "react";
import bouquetsIco from "../../../assets/icons/flowerr.svg";
import ordersIco from "../../../assets/icons/orders.svg";
import reviewsIco from "../../../assets/icons/review.svg";

function AdminSidebar({ active, setActive, isCatalogOpen, setIsCatalogOpen }) {
  const NAV = [
    { key: "orders", label: "Orders", icon: ordersIco },
    { key: "reviews", label: "Reviews", icon: reviewsIco },
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
            <span style={{ flex: 1 }}>CATALOG</span>
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
                Bouquets
              </button>
              <button
                className={`admin-sub-item ${
                  active === "gifts" ? "active" : ""
                }`}
                onClick={() => {
                  setActive("gifts");
                  setIsCatalogOpen(false);
                }}>
                Gifts
              </button>
              <button
                className={`admin-sub-item ${
                  active === "catalog" ? "active" : ""
                }`}
                onClick={() => {
                  setActive("catalog");
                  setIsCatalogOpen(false);
                }}>
                Catalog Settings
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
