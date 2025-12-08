import React, { useState } from "react";
import "./PopupFilterMenu.css";

const PopupFilterMenu = ({ isOpen, onClose, onApply }) => {
  const [price, setPrice] = useState(1000);
  const [size, setSize] = useState("S");
  const [quantity, setQuantity] = useState("51");
  const [events, setEvents] = useState([]);
  const [forWho, setForWho] = useState([]);
  const [flowerType, setFlowerType] = useState([]);

  const toggleArrayValue = (value, setter, array) => {
    setter(
      array.includes(value)
        ? array.filter((v) => v !== value)
        : [...array, value]
    );
  };

  const resetFilters = () => {
    setPrice([1000, 10000]);
    setSize("S");
    setQuantity("51");
    setEvents([]);
    setForWho([]);
    setFlowerType([]);
  };

  const applyFilters = () => {
    onApply({
      priceMin: price,
      priceMax: 50000,
      price,
      size,
      quantity,
      events,
      forWho,
      flowerType,
    });
    onClose();
  };

  return (
    <div className={`filter-overlay ${isOpen ? "open" : ""}`} onClick={onClose}>
      <div className="filter-menu" onClick={(e) => e.stopPropagation()}>
        <button className="close-btn" onClick={onClose}>✕</button>

        <h3 className="filter-title">Filter by:</h3>

        {/* PRICE */}
        <p className="filter-label">Price</p>

        <div className="price-inputs">
        <input
            type="number"
            value={price}
            onChange={(e) => setPrice(+e.target.value)}
        />
        <span>—</span>
        <input
            type="number"
            value={50000}
            disabled
            className="max-price"
        />
        </div>

        <div className="range-wrapper">
        <input
            type="range"
            min="0"
            max="50000"
            value={price}
            onChange={(e) => setPrice(+e.target.value)}
        />
        </div>

        {/* SIZE + QUANTITY */}
        <div className="two-columns">
          <div>
            <p className="filter-label">Size</p>
            {["S", "M", "L", "XL"].map((s) => (
              <label key={s} className="radio-item">
                <input
                  type="radio"
                  checked={size === s}
                  onChange={() => setSize(s)}
                />
                {s}
              </label>
            ))}
          </div>

          <div>
            <p className="filter-label">Quantity</p>
            {["51", "101", "201", "501"].map((q) => (
              <label key={q} className="radio-item">
                <input
                  type="radio"
                  checked={quantity === q}
                  onChange={() => setQuantity(q)}
                />
                {q}
              </label>
            ))}
          </div>
        </div>

        {/* EVENT */}
        <p className="filter-label">Event</p>
        {["Birthday", "Wedding", "Engagement", "Anniversary"].map((e) => (
          <label key={e} className="checkbox-item">
            <input
              type="checkbox"
              checked={events.includes(e)}
              onChange={() => toggleArrayValue(e, setEvents, events)}
            />
            {e}
          </label>
        ))}

        {/* FOR WHO */}
        <p className="filter-label">For who</p>
        {["Mom", "Wife", "Husband", "Kid", "Teacher", "Co-worker"].map((p) => (
          <label key={p} className="checkbox-item">
            <input
              type="checkbox"
              checked={forWho.includes(p)}
              onChange={() => toggleArrayValue(p, setForWho, forWho)}
            />
            {p}
          </label>
        ))}

        {/* FLOWER TYPE */}
        <p className="filter-label">Flower type</p>
        {[
          "Peony",
          "Rose",
          "Lily",
          "Tulip",
          "Orchid",
          "Hydrangea",
          "Daffodil",
          "Chrysanthemum",
        ].map((f) => (
          <label key={f} className="checkbox-item">
            <input
              type="checkbox"
              checked={flowerType.includes(f)}
              onChange={() => toggleArrayValue(f, setFlowerType, flowerType)}
            />
            {f}
          </label>
        ))}

        <button className="reset-btn" onClick={resetFilters}>RESET FILTERS</button>
        <button className="apply-btn" onClick={applyFilters}>APPLY</button>
      </div>
    </div>
  );
};

export default PopupFilterMenu;
