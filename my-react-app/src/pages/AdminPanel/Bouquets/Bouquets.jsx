// src/pages/AdminPanel/Bouquets/Bouquets.jsx
import './Bouquets.css';

export default function Bouquets() {
  return (
    <div className="bouquets-page">
      {/* TITLE */}
      <h2 className="bouquets-title">Bouquets management</h2>

      {/* TOP BAR */}
      <div className="bouquets-top">
        <div className="bouquets-search">
          <span className="search-icon">🔍</span>
          <span className="search-label">Search by name</span>
        </div>

        <button className="add-bouquet-btn">
          Add a bouquet <span className="plus">＋</span>
        </button>
      </div>

      {/* GRID */}
      <div className="bouquets-grid">
        {Array.from({ length: 6 }).map((_, i) => (
          <BouquetCard key={i} />
        ))}
      </div>
    </div>
  );
}

function BouquetCard() {
  return (
    <div className="bouquet-card">
      <div className="bouquet-image" />

      <div className="bouquet-info">
        <span className="bouquet-name">Bouquet 1</span>

        <div className="bouquet-actions">
          <button className="icon-btn" title="Edit">
            ✏️
          </button>
          <button className="icon-btn" title="Delete">
            🗑
          </button>
        </div>
      </div>
    </div>
  );
}
