import { useNavigate } from "react-router-dom";
import ShoppingBagIcon from "../../../assets/icons/ShoppingBagIcon.svg";
import toast from "react-hot-toast";

import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";

function PopularSection({ items, onAddToCart, isLoading, error }) {
  const navigate = useNavigate();

  return (
    <section className="popular-section">
      <h2 className="section-title">POPULAR BOUQUETS / SALES</h2>

      {error ? (
        <div style={{ textAlign: "center", padding: "40px", color: "#666" }}>
          Failed to load popular bouquets.
        </div>
      ) : (
        <div className="popular-cards">
          {isLoading
            ? [...Array(6)].map((_, i) => (
                <div
                  key={i}
                  className="popular-card"
                  style={{ boxShadow: "none" }}>
                  <Skeleton
                    style={{
                      height: "100%",
                      width: "100%",
                      borderRadius: "20px",
                      display: "block",
                      lineHeight: 0,
                    }}
                  />
                </div>
              ))
            : items.map((item) => (
                <div
                  key={item.id}
                  className="popular-card"
                  onClick={() => navigate(`/product/${item.id}`)}>
                  <div className="popular-image">
                    <img
                      src={item.img}
                      alt={item.title}
                      className="popular-img"
                      loading="lazy"
                    />
                  </div>

                  <div className="popular-bottom">
                    <span className="popular-name">{item.title}</span>

                    <span
                      className="shopping-bag-icon"
                      onClick={(e) => {
                        e.stopPropagation();
                        onAddToCart(item);
                        toast.success(`${item.title} added to cart!`);
                      }}>
                      <img src={ShoppingBagIcon} alt="Cart" />
                    </span>
                  </div>
                </div>
              ))}
        </div>
      )}
    </section>
  );
}

export default PopularSection;
