import React from "react";
import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";
import "./ProductSkeleton.css";

const ProductSkeleton = ({ count = 1 }) => {
  return Array(count)
    .fill(0)
    .map((_, index) => (
      <div className="product-card-skeleton" key={index}>
        <div className="skeleton-image">
          <Skeleton height="100%" />
        </div>
        <div className="skeleton-content">
          <Skeleton width="60%" height={24} style={{ marginBottom: "8px" }} />
          <Skeleton width="40%" height={20} style={{ marginBottom: "16px" }} />
          <Skeleton height={40} width="100%" />
        </div>
      </div>
    ));
};

export default ProductSkeleton;
