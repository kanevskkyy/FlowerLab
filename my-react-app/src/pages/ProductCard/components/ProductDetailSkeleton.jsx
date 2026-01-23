import React from "react";
import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";
import "../ProductCard.css"; // Reuse layout styles

const ProductDetailSkeleton = () => {
  return (
    <div className="product-card">
      {/* Breadcrumbs Skeleton */}
      <div className="breadcrumbs" style={{ width: "300px" }}>
        <Skeleton width={300} height={20} />
      </div>

      <div className="product-content">
        {/* Left: Image Section */}
        <div className="product-image-section">
          {/* Main Image */}
          <div className="product-image" style={{ background: "transparent" }}>
            <Skeleton height="100%" borderRadius={20} />
          </div>
          {/* Thumbnails */}
          <div className="product-thumbnails">
            {Array(4)
              .fill(0)
              .map((_, i) => (
                <Skeleton
                  key={i}
                  width={70}
                  height={70}
                  borderRadius={12}
                  style={{ marginRight: 12 }}
                />
              ))}
          </div>
        </div>

        {/* Right: Info Section */}
        <div className="product-info">
          {/* Title */}
          <div className="product-title">
            <Skeleton width="80%" height={40} />
          </div>

          {/* Price */}
          <div className="product-price">
            <Skeleton width={120} height={35} />
          </div>

          {/* Size Section */}
          <div className="size-section">
            <Skeleton width={100} height={24} style={{ marginBottom: 12 }} />
            <div className="size-buttons">
              <Skeleton width={80} height={45} borderRadius={10} />
              <Skeleton width={80} height={45} borderRadius={10} />
              <Skeleton width={80} height={45} borderRadius={10} />
            </div>
          </div>

          {/* Description Blocks */}
          <div className="info-block">
            <Skeleton width={150} height={24} style={{ marginBottom: 8 }} />
            <Skeleton count={3} />
          </div>

          <div className="info-block">
            <Skeleton width={150} height={24} style={{ marginBottom: 8 }} />
            <Skeleton count={2} />
          </div>

          {/* Action Buttons */}
          <div className="product-actions" style={{ marginTop: 30 }}>
            <Skeleton width={180} height={50} borderRadius={40} />
            <Skeleton width={180} height={50} borderRadius={40} />
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductDetailSkeleton;
