import React from "react";
import "./AdminBouquetForm.css";
import { useAdminBouquetForm } from "./hooks/useAdminBouquetForm";

export default function AdminBouquetForm() {
  const {
    isEditMode,
    isGiftMode,
    loading,
    events,
    recipients,
    flowers,
    sizes,
    formData,
    sizeStates,
    handleChange,
    handleCheckboxChange,
    handleImageUpload,
    handleSizeCheckbox,
    handleSizePriceChange,
    handleSizeFlowerQuantityChange,
    handleSizeImagesUpload,
    handleRemoveSizeImage,
    handleSubmit,
    navigate,
  } = useAdminBouquetForm();

  if (loading) {
    return (
      <div className="abf-page">
        <div className="abf-container">
          <div style={{ textAlign: "center", padding: "2rem" }}>Loading...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="abf-page">
      <div className="abf-container">
        {/* HEADER */}
        <header className="abf-header">
          <button
            className="abf-back-btn"
            type="button"
            onClick={() => navigate("/admin")}>
            ← Cancel
          </button>
          <h1 className="abf-title">
            {isEditMode
              ? `Edit ${isGiftMode ? "Gift" : "Bouquet"}`
              : `New ${isGiftMode ? "Gift" : "Bouquet"}`}
          </h1>
          <button className="abf-save-btn" onClick={handleSubmit}>
            Save
          </button>
        </header>

        <div className="abf-content">
          {/* LEFT: PHOTO */}
          <div className="abf-left-col">
            <div className="abf-card abf-photo-card">
              <h3 className="abf-card-title">Photo</h3>
              <div className="abf-photo-preview">
                {formData.img ? (
                  <img src={formData.img} alt="Preview" />
                ) : (
                  <div className="abf-photo-placeholder">No Image</div>
                )}
              </div>
              <label className="abf-upload-btn">
                Upload image
                <input
                  type="file"
                  accept="image/*"
                  hidden
                  onChange={handleImageUpload}
                />
              </label>
            </div>
          </div>

          {/* RIGHT: INFO */}
          <div className="abf-right-col">
            <div className="abf-card">
              <h3 className="abf-card-title">General Information</h3>

              <div className="abf-field">
                <label>Product Name</label>
                <input
                  type="text"
                  name="title"
                  value={formData.title}
                  onChange={handleChange}
                  placeholder={
                    isGiftMode ? "e.g. Teddy Bear" : "e.g. Velvet Roses"
                  }
                />
              </div>

              {isGiftMode ? (
                /* GIFTS: Simple Price & Count */
                <>
                  <div className="abf-field">
                    <label>Price</label>
                    <div className="abf-size-price">
                      <input
                        type="number"
                        name="price"
                        value={formData.price}
                        onChange={handleChange}
                        placeholder="0"
                      />
                      <span>₴</span>
                    </div>
                  </div>
                  <div className="abf-field">
                    <label>Available Count</label>
                    <input
                      type="number"
                      name="availableCount"
                      value={formData.availableCount}
                      onChange={handleChange}
                      placeholder="e.g. 10"
                    />
                  </div>
                </>
              ) : (
                /* BOUQUETS: Sizes & Description */
                <>
                  <div className="abf-field">
                    <label>Sizes & Prices</label>
                    <div className="abf-sizes-list">
                      {sizes.length > 0 ? (
                        sizes.map((size) => {
                          const st = sizeStates[size.id] || {};
                          const isEnabled = !!st.enabled;

                          return (
                            <div
                              key={size.id}
                              className={`abf-size-row ${
                                isEnabled ? "active" : ""
                              }`}>
                              <div className="abf-size-top-row">
                                <div className="abf-size-check">
                                  <input
                                    type="checkbox"
                                    checked={isEnabled}
                                    onChange={() => handleSizeCheckbox(size.id)}
                                  />
                                  <span className="abf-size-name">
                                    {size.name}
                                  </span>
                                </div>

                                {isEnabled && (
                                  <div className="abf-size-price">
                                    <input
                                      type="number"
                                      placeholder="Price"
                                      value={st.price || ""}
                                      onChange={(e) =>
                                        handleSizePriceChange(
                                          size.id,
                                          e.target.value,
                                        )
                                      }
                                    />
                                    <span>₴</span>
                                  </div>
                                )}
                              </div>

                              {/* Flower Quantities - NEW */}
                              {isEnabled && formData.flowerTypes.length > 0 && (
                                <div className="abf-size-flowers">
                                  <p className="abf-mini-label">
                                    Flower Composition:
                                  </p>
                                  <div className="abf-flower-qtys">
                                    {formData.flowerTypes.map((fId) => {
                                      const flower = flowers.find(
                                        (f) => f.id === fId,
                                      );
                                      if (!flower) return null;
                                      const qty =
                                        st.flowerQuantities?.[fId] || "";
                                      return (
                                        <div
                                          key={fId}
                                          className="abf-flower-qty-item">
                                          <span className="abf-fq-name">
                                            {flower.name}
                                          </span>
                                          <input
                                            type="number"
                                            min="0"
                                            placeholder="Qty"
                                            value={qty}
                                            onChange={(e) =>
                                              handleSizeFlowerQuantityChange(
                                                size.id,
                                                fId,
                                                e.target.value,
                                              )
                                            }
                                          />
                                        </div>
                                      );
                                    })}
                                  </div>
                                </div>
                              )}

                              {isEnabled && (
                                <div className="abf-size-img-section">
                                  <label className="abf-upload-btn-mini">
                                    📷 Add Photos
                                    <input
                                      type="file"
                                      accept="image/*"
                                      multiple
                                      hidden
                                      onChange={(e) =>
                                        handleSizeImagesUpload(e, size.id)
                                      }
                                    />
                                  </label>

                                  <div className="abf-mini-gallery">
                                    {st.images && st.images.length > 0 ? (
                                      st.images.map((img, idx) => (
                                        <div
                                          key={idx}
                                          className="abf-mini-thumb">
                                          <img
                                            src={img.url}
                                            alt={`Size ${idx}`}
                                          />
                                          <button
                                            type="button"
                                            className="abf-remove-img"
                                            onClick={() =>
                                              handleRemoveSizeImage(
                                                size.id,
                                                idx,
                                              )
                                            }>
                                            ×
                                          </button>
                                        </div>
                                      ))
                                    ) : (
                                      <span className="abf-no-img-text">
                                        No photos
                                      </span>
                                    )}
                                  </div>
                                </div>
                              )}
                            </div>
                          );
                        })
                      ) : (
                        <div style={{ color: "#999", fontSize: "13px" }}>
                          No sizes found in catalog.
                        </div>
                      )}
                    </div>
                  </div>

                  <div className="abf-field">
                    <label>Description</label>
                    <textarea
                      name="description"
                      rows="4"
                      value={formData.description}
                      onChange={handleChange}
                      placeholder="Enter description..."
                    />
                  </div>
                </>
              )}
            </div>

            {/* CATEGORIES - Only for Bouquets */}
            {!isGiftMode && (
              <div className="abf-card">
                <h3 className="abf-card-title">Categories</h3>

                {/* Events */}
                <div className="abf-cat-group">
                  <h4>Events</h4>
                  <div className="abf-tags">
                    {events.map((item) => (
                      <label
                        key={item.id}
                        className={`abf-tag ${
                          formData.events.includes(item.id) ? "active" : ""
                        }`}>
                        <input
                          type="checkbox"
                          checked={formData.events.includes(item.id)}
                          onChange={() =>
                            handleCheckboxChange("events", item.id)
                          }
                        />
                        {item.name}
                      </label>
                    ))}
                  </div>
                </div>

                {/* For Who */}
                <div className="abf-cat-group">
                  <h4>For Who</h4>
                  <div className="abf-tags">
                    {recipients.map((item) => (
                      <label
                        key={item.id}
                        className={`abf-tag ${
                          formData.forWho.includes(item.id) ? "active" : ""
                        }`}>
                        <input
                          type="checkbox"
                          checked={formData.forWho.includes(item.id)}
                          onChange={() =>
                            handleCheckboxChange("forWho", item.id)
                          }
                        />
                        {item.name}
                      </label>
                    ))}
                  </div>
                </div>

                {/* Flower Type */}
                <div className="abf-cat-group">
                  <h4>Flower Type</h4>
                  <div className="abf-tags">
                    {flowers.map((item) => (
                      <label
                        key={item.id}
                        className={`abf-tag ${
                          formData.flowerTypes.includes(item.id) ? "active" : ""
                        }`}>
                        <input
                          type="checkbox"
                          checked={formData.flowerTypes.includes(item.id)}
                          onChange={() =>
                            handleCheckboxChange("flowerTypes", item.id)
                          }
                        />
                        {item.name}
                      </label>
                    ))}
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
