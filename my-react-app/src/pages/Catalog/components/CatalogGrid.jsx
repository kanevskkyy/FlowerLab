import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import ProductSkeleton from "../../../components/ProductSkeleton/ProductSkeleton";

function CatalogGrid({ products, onOrder, loading }) {
  const navigate = useNavigate();
  const { t } = useTranslation();

  if (loading) {
    return (
      <div className="catalog-grid">
        <ProductSkeleton count={6} />
      </div>
    );
  }

  if (!products || products.length === 0) {
    return (
      <div className="catalog-grid empty">
        <div className="no-results">
          <p>{t("catalog.no_results")}</p>
          <span>{t("catalog.no_results_sub")}</span>
        </div>
      </div>
    );
  }

  const optimizeCloudinaryUrl = (url) => {
    if (!url || !url.includes("cloudinary.com")) return url;
    if (url.includes("/w_")) return url;
    const parts = url.split("upload/");
    if (parts.length < 2) return url;
    return `${parts[0]}upload/w_400,h_400,c_fill,q_auto,f_auto/${parts[1]}`;
  };

  return (
    <div className="catalog-grid">
      {products.map((p) => (
        <div className="catalog-item" key={p.id}>
          <div className="item-img">
            <img
              src={optimizeCloudinaryUrl(p.img)}
              alt={p.title}
              loading="lazy"
            />
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
              {t("catalog.order_btn")}
            </button>
          </div>
        </div>
      ))}
    </div>
  );
}

export default CatalogGrid;
