import React from "react";
import { useNavigate } from "react-router-dom";
import { useConfirm } from "../../context/ModalProvider";
import { useAdminCatalog } from "./hooks/useAdminCatalog";
import CatalogSection from "./components/CatalogSection";
import FlowerStockTable from "./components/FlowerStockTable";
import "./AdminCatalogEdit.css";

export default function AdminCatalogEdit() {
  const navigate = useNavigate();
  const confirm = useConfirm();

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
    return <div className="ace-page loading">Loading...</div>;
  }

  return (
    <div className="ace-page">
      <div className="ace-container">
        {/* HEADER */}
        <header className="ace-header">
          <button className="ace-back-btn" onClick={() => navigate("/admin")}>
            ← Back to Admin
          </button>
          <h1 className="ace-title">Edit Catalog Settings</h1>
          <div style={{ width: 100 }}></div> {/* Spacer to center title */}
        </header>

        {/* CONTENT */}
        <div className="ace-content">
          <div className="ace-grid-2">
            <CatalogSection
              title="Events"
              items={data.events}
              inputValue={inputs.events}
              onInputChange={(val) => handleInputChange("events", val)}
              onAdd={() => handleAdd("events")}
              onRemove={(item) => handleRemoveClick("events", item)}
              onKeyDown={(e) => e.key === "Enter" && handleAdd("events")}
            />
            <CatalogSection
              title="For Who (Recipients)"
              items={data.forWho}
              inputValue={inputs.forWho}
              onInputChange={(val) => handleInputChange("forWho", val)}
              onAdd={() => handleAdd("forWho")}
              onRemove={(item) => handleRemoveClick("forWho", item)}
              onKeyDown={(e) => e.key === "Enter" && handleAdd("forWho")}
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
