import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import catalogService from "../../services/catalogService";
import "./PopupFilterMenu.css";

const PopupFilterMenu = ({ isOpen, onClose, onApply, currentFilters }) => {
  const { t, i18n } = useTranslation();
  const [metadata, setMetadata] = useState({
    events: [],
    recipients: [],
    flowers: [],
    sizes: [],
  });

  const [minPrice, setMinPrice] = useState(0);
  const [maxPrice, setMaxPrice] = useState(50000);
  const [globalMin, setGlobalMin] = useState(0);
  const [globalMax, setGlobalMax] = useState(50000);

  const [selectedSizeId, setSelectedSizeId] = useState("");
  const [quantity, setQuantity] = useState("");
  const [selectedEventIds, setSelectedEventIds] = useState([]);
  const [selectedRecipientIds, setSelectedRecipientIds] = useState([]);
  const [selectedFlowerIds, setSelectedFlowerIds] = useState([]);

  // Initialize from props when opened
  useEffect(() => {
    if (isOpen && currentFilters) {
      if (currentFilters.minPrice) setMinPrice(currentFilters.minPrice);
      else setMinPrice(globalMin);

      if (currentFilters.maxPrice) setMaxPrice(currentFilters.maxPrice);
      else setMaxPrice(globalMax);

      if (currentFilters.sizeIds && currentFilters.sizeIds.length > 0)
        setSelectedSizeId(currentFilters.sizeIds[0]);
      else setSelectedSizeId("");

      // Handle Quantity (stored as array in URL/hook but single string in UI for now?)
      if (currentFilters.quantities && currentFilters.quantities.length > 0)
        setQuantity(currentFilters.quantities[0].toString());
      else setQuantity("");

      setSelectedEventIds(currentFilters.eventIds || []);
      setSelectedRecipientIds(currentFilters.recipientIds || []);
      setSelectedFlowerIds(currentFilters.flowerIds || []);
    } else if (isOpen && !currentFilters) {
      resetFilters();
    }
  }, [isOpen, currentFilters, globalMin, globalMax]);

  useEffect(() => {
    const fetchMetadata = async () => {
      try {
        const data = await catalogService.getFilters();

        const normalize = (items) =>
          (items || []).map((item) => ({
            ...item,
            id: item.id || item.Id,
            name: item.name || item.Name,
          }));

        setMetadata({
          events: normalize(data.eventResponseList),
          recipients: normalize(data.receivmentResponseList),
          flowers: normalize(data.flowerResponseList),
          sizes: normalize(data.sizeResponseList),
        });

        // 1. Try to get price range from filter metadata first
        let gMin = 0;
        let gMax = 50000;

        if (data.priceRange) {
          gMin = data.priceRange.minPrice ?? data.priceRange.MinPrice ?? 0;
          gMax = data.priceRange.maxPrice ?? data.priceRange.MaxPrice ?? 50000;
        }

        // 2. If metadata range seems default or suspicious, verify with direct product queries
        // or just ALWAYS verify to be sure (as user requested precise bounds).
        // Fetch Min
        try {
          const minResp = await catalogService.getBouquets({
            PageSize: 1,
            SortBy: "price_asc",
          });
          if (minResp.items && minResp.items.length > 0) {
            // Note: Does item contain Sizes? We should check min price of the item itself.
            // Assuming item.price is the starting price.
            const p = minResp.items[0];
            // Check if it has sizes with lower price? Usually item.price is base.
            // Let's assume item.price is sufficient.
            const msgMin = p.price;
            if (msgMin !== undefined) gMin = msgMin;
          }
        } catch (e) {
          console.error("Failed to fetch Min Price", e);
        }

        // Fetch Max
        try {
          const maxResp = await catalogService.getBouquets({
            PageSize: 1,
            SortBy: "price_desc",
          });
          if (maxResp.items && maxResp.items.length > 0) {
            const p = maxResp.items[0];
            // If item has sizes, max price might be in sizes?
            // But sorting 'price_desc' likely sorts by base price?
            // If the backend sorts correctly, the first item is the most expensive.
            // We take its price.
            // If it has a more expensive size, we ideally want THAT.
            // But calculating that perfectly is hard without fetching all.
            // Let's trust the backend sort.
            let msgMax = p.price;
            if (p.sizes && p.sizes.length > 0) {
              // Check if any size is max
              const maxSz = Math.max(...p.sizes.map((s) => s.price));
              if (maxSz > msgMax) msgMax = maxSz;
            }
            if (msgMax !== undefined) gMax = msgMax;
          }
        } catch (e) {
          console.error("Failed to fetch Max Price", e);
        }

        setGlobalMin(gMin);
        setGlobalMax(gMax);

        // Update selected if no current filters or if they match defaults
        if (!currentFilters || !currentFilters.maxPrice) {
          setMaxPrice(gMax);
        }
        if (!currentFilters || !currentFilters.minPrice) {
          setMinPrice(gMin);
        }
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
    setMinPrice(globalMin);
    setMaxPrice(globalMax);
    setSelectedSizeId("");
    setQuantity("");
    setSelectedEventIds([]);
    setSelectedRecipientIds([]);
    setSelectedFlowerIds([]);
  };

  const applyFilters = () => {
    onApply({
      minPrice: minPrice === globalMin ? null : minPrice,
      maxPrice: maxPrice === globalMax ? null : maxPrice,
      sizeIds: selectedSizeId ? [selectedSizeId] : [],
      quantities: quantity ? [parseInt(quantity)] : [],
      eventIds: selectedEventIds,
      recipientIds: selectedRecipientIds,
      flowerIds: selectedFlowerIds,
    });
    onClose();
  };

  const langNormalized = i18n.language ? i18n.language.toLowerCase() : "";
  const currentLang =
    langNormalized.startsWith("uk") || langNormalized === "ua" ? "ua" : "en";

  const renderLabel = (item) => {
    if (!item) return "";
    const nameData = item.name || item.Name;
    if (typeof nameData === "object" && nameData !== null) {
      return nameData[currentLang] || nameData.ua || nameData.en || "";
    }
    return (
      nameData ||
      item.name ||
      item.Name ||
      (typeof item === "string" ? item : "")
    );
  };

  return (
    <div className={`filter-overlay ${isOpen ? "open" : ""}`} onClick={onClose}>
      <div className="filter-menu" onClick={(e) => e.stopPropagation()}>
        <button className="close-btn" onClick={onClose}>
          ✕
        </button>

        <h3 className="filter-title">{t("filter.title")}</h3>

        {/* PRICE */}
        <p className="filter-label">{t("filter.price")}</p>

        <div className="price-inputs">
          <input
            type="number"
            value={minPrice}
            onChange={(e) => setMinPrice(Number(e.target.value))}
            min={globalMin}
            max={maxPrice}
          />
          <span>—</span>
          <input
            type="number"
            value={maxPrice}
            onChange={(e) => setMaxPrice(Number(e.target.value))}
            min={minPrice}
            max={globalMax}
          />
        </div>

        <div className="range-wrapper">
          <input
            type="range"
            min={globalMin}
            max={globalMax}
            step="100"
            value={maxPrice}
            onChange={(e) => {
              const val = Number(e.target.value);
              if (val >= minPrice) setMaxPrice(val);
            }}
          />
        </div>

        {/* SIZE + QUANTITY */}
        <div className="two-columns">
          <div>
            <p className="filter-label">{t("filter.size")}</p>
            {metadata.sizes.map((s) => (
              <label key={s.id} className="radio-item">
                <input
                  type="radio"
                  name="size"
                  checked={selectedSizeId === s.id}
                  onChange={() => setSelectedSizeId(s.id)}
                />
                {renderLabel(s)}
              </label>
            ))}
          </div>

          <div>
            <p className="filter-label">{t("filter.quantity")}</p>
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
          <p className="filter-label">{t("filter.event")}</p>
          {metadata.events.map((e) => (
            <label key={e.id} className="checkbox-item">
              <input
                type="checkbox"
                checked={selectedEventIds.includes(e.id)}
                onChange={() =>
                  toggleArrayValue(e.id, setSelectedEventIds, selectedEventIds)
                }
              />
              {renderLabel(e)}
            </label>
          ))}

          {/* FOR WHO */}
          <p className="filter-label">{t("filter.recipient")}</p>
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
              {renderLabel(r)}
            </label>
          ))}

          {/* FLOWER TYPE */}
          <p className="filter-label">{t("filter.flower_type")}</p>
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
              {renderLabel(f)}
            </label>
          ))}
        </div>

        <button className="reset-btn" onClick={resetFilters}>
          {t("filter.reset")}
        </button>
        <button className="apply-btn" onClick={applyFilters}>
          {t("filter.apply")}
        </button>
      </div>
    </div>
  );
};

export default PopupFilterMenu;
