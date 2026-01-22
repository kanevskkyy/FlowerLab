import React, { useState, useEffect } from "react";
import axiosClient from "../../api/axiosClient";
import "./PopupFilterMenu.css";

const PopupFilterMenu = ({ isOpen, onClose, onApply, currentFilters }) => {
  const [metadata, setMetadata] = useState({
    events: [],
    recipients: [],
    flowers: [],
    sizes: [],
  });

  const [price, setPrice] = useState(50000);
  const [selectedSizeId, setSelectedSizeId] = useState("");
  const [quantity, setQuantity] = useState("");
  const [selectedEventIds, setSelectedEventIds] = useState([]);
  const [selectedRecipientIds, setSelectedRecipientIds] = useState([]);
  const [selectedFlowerIds, setSelectedFlowerIds] = useState([]);

  // Initialize from props when opened
  useEffect(() => {
    if (isOpen && currentFilters) {
      if (currentFilters.maxPrice) setPrice(currentFilters.maxPrice);
      else setPrice(50000); // Default if not set

      if (currentFilters.sizeIds && currentFilters.sizeIds.length > 0)
        setSelectedSizeId(currentFilters.sizeIds[0]);
      else setSelectedSizeId("");

      // Handle Quantity (stored as array in URL/hook but single string in UI for now?)
      // Front UI uses radio for single qty, but hook supports array. Let's assume single for now.
      if (currentFilters.quantities && currentFilters.quantities.length > 0)
        setQuantity(currentFilters.quantities[0].toString());
      else setQuantity("");

      setSelectedEventIds(currentFilters.eventIds || []);
      setSelectedRecipientIds(currentFilters.recipientIds || []);
      setSelectedFlowerIds(currentFilters.flowerIds || []);
    } else if (isOpen && !currentFilters) {
      // Reset if no filters exist
      resetFilters();
    }
  }, [isOpen, currentFilters]);

  useEffect(() => {
    const fetchMetadata = async () => {
      try {
        const [eventsRes, recipientsRes, flowersRes, sizesRes] =
          await Promise.all([
            axiosClient.get("/api/catalog/events"),
            axiosClient.get("/api/catalog/recipients"),
            axiosClient.get("/api/catalog/flowers"),
            axiosClient.get("/api/catalog/sizes"),
          ]);

        setMetadata({
          events: eventsRes.data,
          recipients: recipientsRes.data,
          flowers: flowersRes.data,
          sizes: sizesRes.data,
        });
      } catch (error) {
        console.error("Failed to fetch filter metadata:", error);
      }
    };

    if (isOpen) {
      fetchMetadata();
    }
  }, [isOpen]);

  const toggleArrayValue = (value, setter, array) => {
    setter(
      array.includes(value)
        ? array.filter((v) => v !== value)
        : [...array, value],
    );
  };

  const resetFilters = () => {
    setPrice(50000); // Reset to default max
    setSelectedSizeId("");
    setQuantity("");
    setSelectedEventIds([]);
    setSelectedRecipientIds([]);
    setSelectedFlowerIds([]);
  };

  const applyFilters = () => {
    onApply({
      maxPrice: price === 50000 || price === 0 ? null : price,
      sizeIds: selectedSizeId ? [selectedSizeId] : [],
      quantities: quantity ? [parseInt(quantity)] : [],
      eventIds: selectedEventIds,
      recipientIds: selectedRecipientIds,
      flowerIds: selectedFlowerIds,
    });
    onClose();
  };

  return (
    <div className={`filter-overlay ${isOpen ? "open" : ""}`} onClick={onClose}>
      <div className="filter-menu" onClick={(e) => e.stopPropagation()}>
        <button className="close-btn" onClick={onClose}>
          ✕
        </button>

        <h3 className="filter-title">Filter by:</h3>

        {/* PRICE */}
        <p className="filter-label">Price</p>

        <div className="price-inputs">
          <input
            type="number"
            value={price}
            onChange={(e) => setPrice(Number(e.target.value))}
          />
          <span>—</span>
          <input type="number" value={50000} disabled className="max-price" />
        </div>

        <div className="range-wrapper">
          <input
            type="range"
            min="0"
            max="50000"
            step="100"
            value={price}
            onChange={(e) => setPrice(Number(e.target.value))}
          />
        </div>

        {/* SIZE + QUANTITY */}
        <div className="two-columns">
          <div>
            <p className="filter-label">Size</p>
            {metadata.sizes.map((s) => (
              <label key={s.id} className="radio-item">
                <input
                  type="radio"
                  name="size"
                  checked={selectedSizeId === s.id}
                  onChange={() => setSelectedSizeId(s.id)}
                />
                {s.name}
              </label>
            ))}
          </div>

          <div>
            <p className="filter-label">Quantity</p>
            {["51", "101", "201", "501"].map((q) => (
              <label key={q} className="radio-item">
                <input
                  type="radio"
                  name="quantity"
                  checked={quantity === q}
                  onChange={() => setQuantity(q)}
                />
                {q}
              </label>
            ))}
          </div>
        </div>

        <div className="filter-scroll-area">
          {/* EVENT */}
          <p className="filter-label">Event</p>
          {metadata.events.map((e) => (
            <label key={e.id} className="checkbox-item">
              <input
                type="checkbox"
                checked={selectedEventIds.includes(e.id)}
                onChange={() =>
                  toggleArrayValue(e.id, setSelectedEventIds, selectedEventIds)
                }
              />
              {e.name}
            </label>
          ))}

          {/* FOR WHO */}
          <p className="filter-label">For who</p>
          {metadata.recipients.map((r) => (
            <label key={r.id} className="checkbox-item">
              <input
                type="checkbox"
                checked={selectedRecipientIds.includes(r.id)}
                onChange={() =>
                  toggleArrayValue(
                    r.id,
                    setSelectedRecipientIds,
                    selectedRecipientIds,
                  )
                }
              />
              {r.name}
            </label>
          ))}

          {/* FLOWER TYPE */}
          <p className="filter-label">Flower type</p>
          {metadata.flowers.map((f) => (
            <label key={f.id} className="checkbox-item">
              <input
                type="checkbox"
                checked={selectedFlowerIds.includes(f.id)}
                onChange={() =>
                  toggleArrayValue(
                    f.id,
                    setSelectedFlowerIds,
                    selectedFlowerIds,
                  )
                }
              />
              {f.name}
            </label>
          ))}
        </div>

        <button className="reset-btn" onClick={resetFilters}>
          RESET FILTERS
        </button>
        <button className="apply-btn" onClick={applyFilters}>
          APPLY
        </button>
      </div>
    </div>
  );
};

export default PopupFilterMenu;
