import './Catalog.css';

export default function Catalog() {
  return (
    <div className="catalog-page">
      <div className="catalog-header-row">
        <h2 className="catalog-title">Catalog settings</h2>

        <button className="catalog-edit-btn">
          Edit ✏️
        </button>
      </div>

      {/* BLOCK: EVENTS */}
      <CatalogBlock
        title="Events"
        items={[
          'Birthday',
          'Wedding',
          'Engagement',
        ]}
      />

      {/* BLOCK: FOR WHO */}
      <CatalogBlock
        title="For who"
        items={[
          'Mom',
          'Wife',
          'Husband',
          'Kid',
          'Teacher',
          'Co-worker',
        ]}
      />

      {/* BLOCK: FLOWER TYPES */}
      <CatalogBlock
        title="Flower types"
        items={[
          'Peony',
          'Rose',
          'Lily',
          'Tulip',
          'Orchid',
          'Hydrangea',
          'Daffodil',
          'Chrysanthemum',
        ]}
      />
    </div>
  );
}

function CatalogBlock({ title, items }) {
  return (
    <div className="catalog-block">
      <div className="catalog-block-title">{title}</div>

      <ul className="catalog-list">
        {items.map((item, i) => (
          <li key={i} className="catalog-list-item">
            – {item}
          </li>
        ))}
      </ul>
    </div>
  );
}
