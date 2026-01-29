import React, { useRef, useEffect } from "react";
import { useTranslation } from "react-i18next";

function CatalogSort({ isOpen, setIsOpen, onSortChange }) {
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

  return (
    <div
      className="catalog-sort"
      onClick={() => setIsOpen((prev) => !prev)}
      ref={sortButtonRef}>
      <span>{t("catalog.sort_by")}</span>

      {isOpen && (
        <div className="sort-popup" ref={sortRef}>
          <p onClick={() => onSortChange("date_desc")}>
            {t("catalog.sort_date_desc")}
          </p>
          <p onClick={() => onSortChange("date_asc")}>
            {t("catalog.sort_date_asc")}
          </p>
          <p onClick={() => onSortChange("price_desc")}>
            {t("catalog.sort_price_desc")}
          </p>
          <p onClick={() => onSortChange("price_asc")}>
            {t("catalog.sort_price_asc")}
          </p>
          <p onClick={() => onSortChange("popularity")}>
            {t("catalog.sort_popularity")}
          </p>
        </div>
      )}
    </div>
  );
}

export default CatalogSort;
