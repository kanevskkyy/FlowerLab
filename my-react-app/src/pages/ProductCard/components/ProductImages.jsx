import React from "react";

const ProductImages = ({
  mainImage,
  thumbnails,
  selectedIndex,
  onSelect,
  altTitle,
}) => {
  return (
    <div className="product-image-section">
      <div className="product-image">
        <img src={mainImage} alt={altTitle} />
      </div>

      {thumbnails && thumbnails.length > 1 && (
        <div className="product-thumbnails">
          {thumbnails.map((img, idx) => (
            <div
              key={idx}
              className={`product-thumb ${idx === selectedIndex ? "active" : ""}`}
              onClick={() => onSelect(idx)}>
              <img src={img} alt={`Thumb ${idx}`} />
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default ProductImages;
