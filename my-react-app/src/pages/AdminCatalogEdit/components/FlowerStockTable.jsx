import React from "react";
import { useTranslation } from "react-i18next";
import editIcon from "../../../assets/icons/edit.svg";
import trashIcon from "../../../assets/icons/trash.svg";
import { getLocalizedValue } from "../../../utils/localizationUtils";

const FlowerStockTable = ({
  flowers,
  inputs,
  onInputChange,
  onAdd,
  onRemove,
  editingFlower,
  setEditingFlower,
  onUpdateFlower,
}) => {
  const { t, i18n } = useTranslation();
  return (
    <div className="ace-card full-width">
      <h3 className="ace-card-title">{t("admin.catalog.flower_types")}</h3>

      <div className="ace-table-wrapper">
        <table className="ace-table">
          <thead>
            <tr>
              <th>{t("admin.catalog.name")}</th>
              <th style={{ width: "120px" }}>{t("admin.catalog.quantity")}</th>
              <th style={{ width: "140px", textAlign: "right" }}>
                {t("admin.catalog.actions")}
              </th>
            </tr>
          </thead>
          <tbody>
            {/* Add Row */}
            <tr className="ace-table-add-row">
              <td>
                <div className="ace-input-group">
                  <input
                    type="text"
                    placeholder={t("admin.catalog.ua_placeholder")}
                    value={inputs.flowerTypesName_ua}
                    onChange={(e) =>
                      onInputChange("flowerTypesName_ua", e.target.value)
                    }
                  />
                  <input
                    type="text"
                    placeholder={t("admin.catalog.en_placeholder")}
                    value={inputs.flowerTypesName_en}
                    onChange={(e) =>
                      onInputChange("flowerTypesName_en", e.target.value)
                    }
                  />
                </div>
              </td>
              <td>
                <input
                  type="number"
                  placeholder="0"
                  value={inputs.flowerTypesQuantity}
                  onChange={(e) =>
                    onInputChange("flowerTypesQuantity", e.target.value)
                  }
                  onKeyDown={(e) => e.key === "Enter" && onAdd("flowerTypes")}
                />
              </td>
              <td style={{ textAlign: "right" }}>
                <button
                  className="ace-add-btn small"
                  onClick={() => onAdd("flowerTypes")}>
                  {t("admin.add")}
                </button>
              </td>
            </tr>

            {/* List Rows */}
            {flowers.map((item, index) => {
              const itemId = item.id || item.Id || `flower-${index}`;
              const editingId = editingFlower?.id || editingFlower?.Id;
              const isEditing = editingFlower && editingId === itemId;

              if (isEditing) {
                let nameData = editingFlower.name || editingFlower.Name || {};
                // If name is just a string, convert to object
                if (typeof nameData === "string") {
                  nameData = { ua: nameData, en: nameData };
                }
                return (
                  <tr key={itemId} className="ace-editing-row">
                    <td>
                      <div className="ace-input-group">
                        <input
                          type="text"
                          placeholder={t("admin.catalog.ua_placeholder")}
                          value={nameData.ua || ""}
                          onChange={(e) =>
                            setEditingFlower({
                              ...editingFlower,
                              name: { ...nameData, ua: e.target.value },
                            })
                          }
                        />
                        <input
                          type="text"
                          placeholder={t("admin.catalog.en_placeholder")}
                          value={nameData.en || ""}
                          onChange={(e) =>
                            setEditingFlower({
                              ...editingFlower,
                              name: { ...nameData, en: e.target.value },
                            })
                          }
                        />
                      </div>
                    </td>
                    <td>
                      <input
                        type="number"
                        value={
                          editingFlower.quantity ?? editingFlower.Quantity ?? ""
                        }
                        onChange={(e) =>
                          setEditingFlower({
                            ...editingFlower,
                            quantity: parseInt(e.target.value) || 0,
                          })
                        }
                      />
                    </td>
                    <td style={{ textAlign: "right" }}>
                      <button
                        className="ace-icon-btn save"
                        onClick={onUpdateFlower}
                        title={t("admin.save")}>
                        <svg
                          width="24"
                          height="24"
                          viewBox="0 0 24 24"
                          fill="none"
                          stroke="currentColor"
                          strokeWidth="2.5"
                          strokeLinecap="round"
                          strokeLinejoin="round">
                          <polyline points="20 6 9 17 4 12"></polyline>
                        </svg>
                      </button>
                      <button
                        className="ace-icon-btn cancel"
                        onClick={() => setEditingFlower(null)}
                        title={t("admin.cancel")}>
                        <svg
                          width="24"
                          height="24"
                          viewBox="0 0 24 24"
                          fill="none"
                          stroke="currentColor"
                          strokeWidth="2.5"
                          strokeLinecap="round"
                          strokeLinejoin="round">
                          <line x1="18" y1="6" x2="6" y2="18"></line>
                          <line x1="6" y1="6" x2="18" y2="18"></line>
                        </svg>
                      </button>
                    </td>
                  </tr>
                );
              }

              const nameData = item.name || item.Name;
              const displayName =
                getLocalizedValue(nameData, i18n.language) ||
                (typeof nameData === "string" ? nameData : "") ||
                t("admin.catalog.unnamed_item");
              const quantity = item.quantity ?? item.Quantity ?? 0;

              return (
                <tr key={itemId}>
                  <td>{displayName}</td>
                  <td>{quantity}</td>
                  <td style={{ textAlign: "right" }}>
                    <button
                      className="ace-icon-btn edit"
                      onClick={() => setEditingFlower(item)}>
                      <img src={editIcon} alt="edit" />
                    </button>
                    <button
                      className="ace-icon-btn delete"
                      onClick={() => onRemove("flowerTypes", item)}>
                      <img src={trashIcon} alt="trash" />
                    </button>
                  </td>
                </tr>
              );
            })}
            {flowers.length === 0 && (
              <tr>
                <td colSpan="3" className="ace-empty">
                  {t("admin.catalog.no_items")}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default FlowerStockTable;
