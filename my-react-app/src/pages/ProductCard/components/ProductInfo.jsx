import ShoppingBagIcon from "../../../assets/icons/ShoppingBagIcon.svg";
import { useTranslation } from "react-i18next";

const ProductInfo = ({
  product,
  selectedSize,
  onSizeSelect,
  onBuyNow,
  onAddToCart,
}) => {
  const { t } = useTranslation();
  return (
    <div className="product-info">
      <h1 className="product-title">{product.title}</h1>

      <p className="product-price">{product.prices[selectedSize]} ₴</p>

      <div className="info-block">
        <h3>{t("product.description_label")}</h3>
        <p>{product.description}</p>
      </div>

      <div className="info-block">
        <h3>{t("product.composition_label")}</h3>
        <p>{product.compositions?.[selectedSize] || product.composition}</p>
      </div>

      {/* Events Tags */}
      {product.events && product.events.length > 0 && (
        <div className="info-block">
          <h3>{t("product.events_label")}</h3>
          <div className="info-tags">
            {product.events.map((tag) => (
              <span key={tag} className="info-tag">
                {tag}
              </span>
            ))}
          </div>
        </div>
      )}

      {/* Recipients Tags */}
      {product.recipients && product.recipients.length > 0 && (
        <div className="info-block">
          <h3>{t("product.for_who_label")}</h3>
          <div className="info-tags">
            {product.recipients.map((tag) => (
              <span key={tag} className="info-tag">
                {tag}
              </span>
            ))}
          </div>
        </div>
      )}

      {/* Size Selection */}
      <div className="size-section">
        <h3>{t("product.size")}</h3>
        <div className="size-buttons">
          {product.availableSizes.map((size) => {
            const stock = product.stock?.[size];
            const isOOS = stock?.max === 0;
            const isDisabled = isOOS || stock?.available === false;

            return (
              <button
                key={size}
                disabled={isDisabled}
                className={`size-btn ${selectedSize === size ? "active" : ""} ${isDisabled ? "disabled" : ""}`}
                onClick={() => !isDisabled && onSizeSelect(size)}
                title={isOOS ? t("product.out_of_stock") : ""}>
                {size}
              </button>
            );
          })}
        </div>

        {/* Stock Warning */}
        {product.stock?.[selectedSize]?.max > 0 &&
          product.stock?.[selectedSize]?.max < 5 && (
            <div
              style={{
                color: "#d9534f",
                fontSize: "12px",
                marginTop: "8px",
                fontWeight: "500",
              }}>
              🔥{" "}
              {t("product.only_left", {
                count: product.stock[selectedSize].max,
              })}
            </div>
          )}
        {product.stock?.[selectedSize]?.max === 0 && (
          <div
            style={{
              color: "#999",
              fontSize: "12px",
              marginTop: "8px",
              fontStyle: "italic",
            }}>
            {t("product.out_of_stock")}
          </div>
        )}
      </div>

      {/* Actions */}
      <div className="product-actions">
        <button
          className="buy-now-btn"
          onClick={onBuyNow}
          disabled={!product.stock?.[selectedSize]?.max}
          style={
            !product.stock?.[selectedSize]?.max
              ? { opacity: 0.5, cursor: "not-allowed" }
              : {}
          }>
          {product.stock?.[selectedSize]?.max === 0
            ? t("product.sold_out")
            : t("product.buy_now")}
        </button>
        <button
          className="add-cart-btn"
          onClick={() => onAddToCart(true)}
          disabled={!product.stock?.[selectedSize]?.max}
          style={
            !product.stock?.[selectedSize]?.max
              ? { opacity: 0.5, cursor: "not-allowed" }
              : {}
          }>
          ADD TO CART
          <span className="cart-icon">
            <img src={ShoppingBagIcon} alt="Cart" />
          </span>
        </button>
      </div>
    </div>
  );
};

export default ProductInfo;
