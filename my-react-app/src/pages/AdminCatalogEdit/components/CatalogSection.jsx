import React from "react";
import { useTranslation } from "react-i18next";

const CatalogSection = ({
  title,
  items,
  onInputChange,
  onAdd,
  onRemove,
  inputs,
  category,
}) => {
  const { t, i18n } = useTranslation();
  return (
    <div className="ace-card">
      <h3 className="ace-card-title">{title}</h3>

      {/* Tags List */}
      <div className="ace-tags-list">
        {items.map((item, index) => {
          const key = item.id || item.Id || `category-${index}`;
          const nameData = item.name || item.Name;

          const currentLang = i18n.language === "UA" ? "ua" : "en";
          const label =
            typeof nameData === "object"
              ? nameData[currentLang] || nameData.ua || nameData.en
              : typeof nameData === "string"
                ? nameData
                : typeof item === "string"
                  ? item
                  : nameData || "Unknown";

          return (
            <div key={key} className="ace-tag">
              <span>{label}</span>
              <button
                type="button"
                className="ace-tag-remove"
                onClick={() => onRemove(item)}>
                ✕
              </button>
            </div>
          );
        })}
        {items.length === 0 && (
          <div className="ace-empty">{t("admin.catalog.no_items")}</div>
        )}
      </div>

      {/* Add Row - MULTI LANG */}
      <div className="ace-add-row multilang">
        <div className="ace-input-group">
          <input
            type="text"
            placeholder={t("admin.catalog.ua_placeholder")}
            value={inputs[`${category}_ua`] || ""}
            onChange={(e) => onInputChange(`${category}_ua`, e.target.value)}
          />
          <input
            type="text"
            placeholder={t("admin.catalog.en_placeholder")}
            value={inputs[`${category}_en`] || ""}
            onChange={(e) => onInputChange(`${category}_en`, e.target.value)}
          />
        </div>
        <button type="button" className="ace-add-btn" onClick={onAdd}>
          {t("admin.add")}
        </button>
      </div>
    </div>
  );
};

export default CatalogSection;
