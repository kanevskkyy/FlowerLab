import React from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useConfirm } from "../../context/ModalProvider";
import { useAdminCatalog } from "./hooks/useAdminCatalog";
import CatalogSection from "./components/CatalogSection";
import FlowerStockTable from "./components/FlowerStockTable";
import "./AdminCatalogEdit.css";

export default function AdminCatalogEdit() {
  const navigate = useNavigate();
  const confirm = useConfirm();
  const { t } = useTranslation();

  const {
    loading,
    data,
    inputs,
    editingFlower,
    setEditingFlower,
    handleInputChange,
    handleAdd,
    handleUpdateFlower,
    handleRemoveClick,
  } = useAdminCatalog(confirm);

  if (loading) {
    return <div className="ace-page loading">{t("admin.loading")}</div>;
  }

  return (
    <div className="ace-page">
      <div className="ace-container">
        {/* HEADER */}
        <header className="ace-header">
          <button className="ace-back-btn" onClick={() => navigate("/admin")}>
            ← {t("admin.catalog.back_to_admin")}
          </button>
          <h1 className="ace-title">{t("admin.catalog.edit_title")}</h1>
          <div style={{ width: 100 }}></div> {/* Spacer to center title */}
        </header>

        {/* CONTENT */}
        <div className="ace-content">
          <div className="ace-grid-2">
            <CatalogSection
              title={t("admin.catalog.events")}
              items={data.events}
              inputs={inputs}
              category="events"
              onInputChange={handleInputChange}
              onAdd={() => handleAdd("events")}
              onRemove={(item) => handleRemoveClick("events", item)}
            />
            <CatalogSection
              title={t("admin.catalog.for_who")}
              items={data.forWho}
              inputs={inputs}
              category="forWho"
              onInputChange={handleInputChange}
              onAdd={() => handleAdd("forWho")}
              onRemove={(item) => handleRemoveClick("forWho", item)}
            />
          </div>

          {/* Flowers Full Width */}
          <FlowerStockTable
            flowers={data.flowerTypes}
            inputs={inputs}
            onInputChange={handleInputChange}
            onAdd={handleAdd}
            onRemove={handleRemoveClick}
            editingFlower={editingFlower}
            setEditingFlower={setEditingFlower}
            onUpdateFlower={handleUpdateFlower}
          />
        </div>
      </div>
    </div>
  );
}
