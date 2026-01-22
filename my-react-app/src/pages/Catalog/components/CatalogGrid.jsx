import { useNavigate } from "react-router-dom";
import ProductSkeleton from "../../../components/ProductSkeleton/ProductSkeleton";

function CatalogGrid({ products, onOrder, loading }) {
  const navigate = useNavigate();

  if (loading) {
    return (
      <div className="catalog-grid">
        <ProductSkeleton count={6} />
      </div>
    );
  }

  if (!products || products.length === 0) {
    return (
      <div className="catalog-grid">
        <div className="no-results">
          <p>No products found matching your criteria</p>
        </div>
      </div>
    );
  }

  return (
    <div className="catalog-grid">
      {products.map((p) => (
        <div className="catalog-item" key={p.id}>
          <div className="item-img">
            <img src={p.img} alt={p.title} />
          </div>

          <div className="item-bottom">
            <div className="item-text">
              <p>{p.title}</p>
              <p>{p.price}</p>
            </div>

            <button
              className="order-btn"
              onClick={() =>
                onOrder ? onOrder(p) : navigate(`/product/${p.id}`)
              }>
              ORDER
            </button>
          </div>
        </div>
      ))}
    </div>
  );
}

export default CatalogGrid;
