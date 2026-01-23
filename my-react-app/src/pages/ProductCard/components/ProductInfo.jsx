import ShoppingBagIcon from "../../../assets/icons/ShoppingBagIcon.svg";

const ProductInfo = ({
  product,
  selectedSize,
  onSizeSelect,
  onBuyNow,
  onAddToCart,
}) => {
  return (
    <div className="product-info">
      <h1 className="product-title">{product.title}</h1>

      <p className="product-price">{product.prices[selectedSize]} ₴</p>

      <div className="info-block">
        <h3>Description</h3>
        <p>{product.description}</p>
      </div>

      <div className="info-block">
        <h3>Composition:</h3>
        <p>{product.compositions?.[selectedSize] || product.composition}</p>
      </div>

      {/* Events Tags */}
      {product.events && product.events.length > 0 && (
        <div className="info-block">
          <h3>Events:</h3>
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
          <h3>For Who:</h3>
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
        <h3>Size</h3>
        <div className="size-buttons">
          {product.availableSizes.map((size) => (
            <button
              key={size}
              className={`size-btn ${selectedSize === size ? "active" : ""}`}
              onClick={() => onSizeSelect(size)}>
              {size}
            </button>
          ))}
        </div>
      </div>

      {/* Actions */}
      <div className="product-actions">
        <button className="buy-now-btn" onClick={onBuyNow}>
          BUY NOW
        </button>
        <button className="add-cart-btn" onClick={() => onAddToCart(true)}>
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
