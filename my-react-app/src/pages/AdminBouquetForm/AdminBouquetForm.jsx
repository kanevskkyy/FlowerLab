import { useTranslation } from "react-i18next";
import "./AdminBouquetForm.css";
import { useAdminBouquetForm } from "./hooks/useAdminBouquetForm";

export default function AdminBouquetForm() {
  const { t, i18n } = useTranslation();
  const currentLang = i18n.language.toLowerCase().includes("ua") ? "ua" : "en";

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

  const {
    isEditMode,
    isGiftMode,

    loading,
    isSubmitting,
    events,
    recipients,
    flowers,
    sizes,
    formData,
    sizeStates,
    handleChange,
    handleLocalizedChange,
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
          <div style={{ textAlign: "center", padding: "2rem" }}>
            {t("admin.loading")}
          </div>
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
            <span className="abf-back-arrow">←</span>
            <span className="abf-back-text">{t("admin.cancel")}</span>
          </button>
          <h1 className="abf-title">
            {isEditMode
              ? isGiftMode
                ? t("admin.form.edit_gift")
                : t("admin.form.edit_bouquet")
              : isGiftMode
                ? t("admin.form.new_gift")
                : t("admin.form.new_bouquet")}
          </h1>
          <button
            className="abf-save-btn"
            onClick={handleSubmit}
            disabled={isSubmitting}
            style={{ opacity: isSubmitting ? 0.7 : 1 }}>
            {isSubmitting ? t("admin.saving") : t("admin.save")}
          </button>
        </header>

        <div className="abf-content">
          {/* LEFT: PHOTO */}
          <div className="abf-left-col">
            <div className="abf-card abf-photo-card">
              <h3 className="abf-card-title">{t("admin.form.photo")}</h3>
              <div className="abf-photo-preview">
                {formData.img ? (
                  <img src={formData.img} alt="Preview" />
                ) : (
                  <div className="abf-photo-placeholder">
                    {t("admin.products.no_image")}
                  </div>
                )}
              </div>
              <label className="abf-upload-btn">
                {t("admin.products.upload_image")}
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
              <h3 className="abf-card-title">{t("admin.form.general_info")}</h3>

              <div className="abf-field">
                <label>{t("admin.form.name_ua")}</label>
                <input
                  type="text"
                  value={formData.name.ua}
                  onChange={(e) =>
                    handleLocalizedChange("name", "ua", e.target.value)
                  }
                  placeholder={t("admin.form.placeholder_ua")}
                />
              </div>
              <div className="abf-field">
                <label>{t("admin.form.name_en")}</label>
                <input
                  type="text"
                  value={formData.name.en}
                  onChange={(e) =>
                    handleLocalizedChange("name", "en", e.target.value)
                  }
                  placeholder={t("admin.form.placeholder_en")}
                />
              </div>

              {isGiftMode ? (
                /* GIFTS: Simple Price & Count */
                <>
                  <div className="abf-field">
                    <label>{t("admin.form.price")}</label>
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
                    <label>{t("admin.form.available_count")}</label>
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
                    <label>{t("admin.form.sizes_prices")}</label>
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
                                    {renderLabel(size)}
                                  </span>
                                </div>

                                {isEnabled && (
                                  <div className="abf-size-price">
                                    <input
                                      type="number"
                                      placeholder={t("admin.form.price")}
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
                                    {t("admin.form.composition")}
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
                                            {renderLabel(flower)}
                                          </span>
                                          <input
                                            type="number"
                                            min="0"
                                            placeholder={t(
                                              "admin.catalog.quantity",
                                            )}
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
                                    📷 {t("admin.form.add_photos")}
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
                                        {t("admin.form.no_photos")}
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
                          {t("admin.catalog.no_items")}
                        </div>
                      )}
                    </div>
                  </div>

                  <div className="abf-field">
                    <label>{t("admin.form.description_ua")}</label>
                    <textarea
                      rows="3"
                      value={formData.description.ua}
                      onChange={(e) =>
                        handleLocalizedChange(
                          "description",
                          "ua",
                          e.target.value,
                        )
                      }
                      placeholder={t("admin.form.desc_placeholder_ua")}
                    />
                  </div>
                  <div className="abf-field">
                    <label>{t("admin.form.description_en")}</label>
                    <textarea
                      rows="3"
                      value={formData.description.en}
                      onChange={(e) =>
                        handleLocalizedChange(
                          "description",
                          "en",
                          e.target.value,
                        )
                      }
                      placeholder={t("admin.form.desc_placeholder_en")}
                    />
                  </div>
                </>
              )}
            </div>

            {/* CATEGORIES - Only for Bouquets */}
            {!isGiftMode && (
              <div className="abf-card">
                <h3 className="abf-card-title">{t("admin.form.categories")}</h3>
                {/* Flower Type */}
                <div className="abf-cat-group">
                  <h4>{t("filter.flower_type")}</h4>
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
                        {renderLabel(item)}
                      </label>
                    ))}
                  </div>
                </div>

                {/* Events */}
                <div className="abf-cat-group">
                  <h4>{t("admin.catalog.events")}</h4>
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
                        {renderLabel(item)}
                      </label>
                    ))}
                  </div>
                </div>

                {/* For Who */}
                <div className="abf-cat-group">
                  <h4>{t("admin.catalog.for_who")}</h4>
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
                        {renderLabel(item)}
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
