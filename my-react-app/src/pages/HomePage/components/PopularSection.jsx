import React from "react";
import { useNavigate } from "react-router-dom";
import ShoppingBagIcon from "../../../assets/images/ShoppingBagIcon.svg";
import toast from "react-hot-toast";

function PopularSection({ items, onAddToCart }) {
  const navigate = useNavigate();

  return (
    <section className="popular-section">
      <h2 className="section-title">POPULAR BOUQUETS / SALES</h2>
      <div className="popular-cards">
        {items.map((item) => (
          <div
            key={item.id}
            className="popular-card"
            onClick={() => navigate(`/product/${item.id}`)}
          >
            <div className="popular-image">
              <img src={item.img} alt={item.title} className="popular-img" />
            </div>

            <div className="popular-bottom">
              <span className="popular-name">{item.title}</span>

              <span
                className="shopping-bag-icon"
                onClick={(e) => {
                  e.stopPropagation();
                  onAddToCart(item);
                  toast.success(`${item.title} added to cart!`);
                }}
              >
                <img src={ShoppingBagIcon} alt="Cart" />
              </span>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}

export default PopularSection;
