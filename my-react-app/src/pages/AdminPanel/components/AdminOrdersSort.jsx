import React, { useRef, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

function AdminOrdersSort({ sort, onSortChange }) {
  const [isOpen, setIsOpen] = useState(false);
  const sortRef = useRef(null);
  const sortButtonRef = useRef(null);

  // Close sort on outside click
  useEffect(() => {
    function handleClickOutside(event) {
      if (
        sortRef.current &&
        !sortRef.current.contains(event.target) &&
        !sortButtonRef.current.contains(event.target)
      ) {
        setIsOpen(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [setIsOpen]);

  const { t } = useTranslation();

  const handleSelect = (value) => {
    onSortChange(value);
    setIsOpen(false);
  };

  return (
    <div
      className="admin-orders-sort-custom"
      onClick={() => setIsOpen((prev) => !prev)}
      ref={sortButtonRef}>
      <span>{t("admin.orders.sort_by")}</span>

      {isOpen && (
        <div className="admin-sort-popup" ref={sortRef}>
          <p
            onClick={() => handleSelect("date-desc")}
            className={sort === "date-desc" || sort === "new" ? "active" : ""}>
            {t("admin.orders.sort_date_new")}
          </p>
          <p
            onClick={() => handleSelect("date-asc")}
            className={sort === "date-asc" || sort === "old" ? "active" : ""}>
            {t("admin.orders.sort_date_old")}
          </p>
          <p
            onClick={() => handleSelect("qty-desc")}
            className={sort === "qty-desc" ? "active" : ""}>
            {t("admin.orders.sort_qty_high")}
          </p>
          <p
            onClick={() => handleSelect("qty-asc")}
            className={sort === "qty-asc" ? "active" : ""}>
            {t("admin.orders.sort_qty_low")}
          </p>
          <p
            onClick={() => handleSelect("name-asc")}
            className={sort === "name-asc" ? "active" : ""}>
            {t("admin.orders.sort_name_az")}
          </p>
          <p
            onClick={() => handleSelect("name-desc")}
            className={sort === "name-desc" ? "active" : ""}>
            {t("admin.orders.sort_name_za")}
          </p>
        </div>
      )}
    </div>
  );
}

export default AdminOrdersSort;
