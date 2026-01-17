import React from "react";
import editIco from "../../../assets/icons/edit.svg";

function AdminCatalogSettings({ settings, onEdit }) {
  return (
    <section className="admin-section admin-catalog">
      <div className="admin-catalog-head">
        <h2 className="admin-section-title admin-catalog-title">
          Catalog settings
        </h2>
        <button
          className="admin-catalog-edit"
          onClick={onEdit}
          type="button"
        >
          <span>Edit</span>
          <img src={editIco} alt="" />
        </button>
      </div>
      <div className="admin-catalog-groups">
        <div className="admin-catalog-group">
          <div className="admin-catalog-pill">Events</div>
          <ul className="admin-catalog-list">
            {settings.events.map((x) => (
              <li key={x}>{x}</li>
            ))}
          </ul>
        </div>
        <div className="admin-catalog-group">
          <div className="admin-catalog-pill">For who</div>
          <ul className="admin-catalog-list">
            {settings.forWho.map((x) => (
              <li key={x}>{x}</li>
            ))}
          </ul>
        </div>
        <div className="admin-catalog-group">
          <div className="admin-catalog-pill">Flower types</div>
          <ul className="admin-catalog-list">
            {settings.flowerTypes.map((x) => (
              <li key={x}>{x}</li>
            ))}
          </ul>
        </div>
      </div>
    </section>
  );
}

export default AdminCatalogSettings;
