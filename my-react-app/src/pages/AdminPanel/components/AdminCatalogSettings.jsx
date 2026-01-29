import React from "react";
import { useTranslation } from "react-i18next";
import editIco from "../../../assets/icons/edit.svg";

function AdminCatalogSettings({ settings, onEdit }) {
  const { t, i18n } = useTranslation();
  const currentLang = i18n.language === "UA" ? "ua" : "en";

  const renderLabel = (item) => {
    if (!item) return "";
    if (typeof item === "string") return item;

    const nameData = item.name || item.Name;
    if (typeof nameData === "object" && nameData !== null) {
      return nameData[currentLang] || nameData.ua || nameData.en || "";
    }
    if (typeof nameData === "string") return nameData;

    // Fallback if the item itself is the name object
    if (item.ua || item.en) {
      return item[currentLang] || item.ua || item.en || "";
    }

    return "";
  };

  return (
    <section className="admin-section admin-catalog">
      <div className="admin-catalog-head">
        <h2 className="admin-section-title admin-catalog-title">
          {t("admin.catalog.settings_title")}
        </h2>
        <button className="admin-catalog-edit" onClick={onEdit} type="button">
          <span>{t("admin.edit")}</span>
          <img src={editIco} alt="" />
        </button>
      </div>

      <div className="admin-catalog-tables">
        {/* Events Table */}
        <div className="admin-catalog-table-wrapper">
          <div className="admin-catalog-pill">{t("admin.catalog.events")}</div>
          <table className="admin-mini-table">
            <tbody>
              {settings.events.map((x, idx) => (
                <tr key={idx}>
                  <td>— {renderLabel(x)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Recipients Table */}
        <div className="admin-catalog-table-wrapper">
          <div className="admin-catalog-pill">{t("admin.catalog.for_who")}</div>
          <table className="admin-mini-table">
            <tbody>
              {settings.forWho.map((x, idx) => (
                <tr key={idx}>
                  <td>— {renderLabel(x)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Flower Types Table */}
        <div className="admin-catalog-table-wrapper">
          <div className="admin-catalog-pill">
            {t("admin.catalog.flower_types")}
          </div>
          <table className="admin-mini-table">
            <thead>
              <tr>
                <th
                  style={{
                    textAlign: "left",
                    padding: "12px 22px",
                    fontSize: "12px",
                    color: "#888",
                    fontWeight: "500",
                    borderBottom: "1px solid #f9f9f9",
                  }}>
                  {t("admin.catalog.name")}
                </th>
                <th
                  style={{
                    textAlign: "right",
                    padding: "12px 22px",
                    fontSize: "12px",
                    color: "#888",
                    fontWeight: "500",
                    borderBottom: "1px solid #f9f9f9",
                  }}>
                  {t("admin.catalog.quantity")}
                </th>
              </tr>
            </thead>
            <tbody>
              {settings.flowerTypes.map((x, idx) => (
                <tr key={idx}>
                  <td>— {renderLabel(x)}</td>
                  <td style={{ textAlign: "right", fontWeight: "600" }}>
                    {x.quantity ?? x.Quantity ?? 0}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </section>
  );
}

export default AdminCatalogSettings;
