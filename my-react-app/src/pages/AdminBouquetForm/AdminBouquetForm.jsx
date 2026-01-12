import React, { useState, useEffect } from "react";
import { useNavigate, useParams, useLocation } from "react-router-dom";
import "./AdminBouquetForm.css";

const CATEGORIES = {
  events: ["Birthday", "Wedding", "Engagement", "Anniversary"],
  forWho: ["Mom", "Wife", "Husband", "Kid", "Teacher", "Co-worker"],
  flowerTypes: ["Peony", "Rose", "Lily", "Tulip", "Orchid", "Hydrangea"],
};

export default function AdminBouquetForm() {
  const navigate = useNavigate();
  const { id } = useParams();
  const location = useLocation();

  const isEditMode = Boolean(id);
  // Перевіряємо за URL, чи ми додаємо подарунок
  const isGiftMode = location.pathname.includes("gifts");

  // ✅ ВИПРАВЛЕНО: Категорія встановлюється правильно одразу тут.
  // useEffect для цього більше не потрібен.
  const [formData, setFormData] = useState({
    title: "",
    price: "",
    description: "",
    category: isGiftMode ? "Gifts" : "Bouquets",
    events: [],
    forWho: [],
    flowerTypes: [],
    img: null,
  });

  // useEffect для завантаження даних при редагуванні (залишаємо)
  useEffect(() => {
    if (isEditMode) {
      // Імітація завантаження даних
      const timer = setTimeout(() => {
        setFormData({
          title: isGiftMode ? "Teddy Bear" : "Bouquet Roses",
          price: isGiftMode ? "850" : "1500",
          description: "Sample description...",
          category: isGiftMode ? "Gifts" : "Bouquets",
          events: ["Birthday"],
          forWho: ["Mom"],
          flowerTypes: isGiftMode ? [] : ["Rose"],
          img: null,
        });
      }, 100);
      return () => clearTimeout(timer);
    }
  }, [isEditMode, id, isGiftMode]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleCheckboxChange = (category, item) => {
    setFormData((prev) => {
      const list = prev[category];
      if (list.includes(item)) {
        return { ...prev, [category]: list.filter((i) => i !== item) };
      } else {
        return { ...prev, [category]: [...list, item] };
      }
    });
  };

  const handleImageUpload = (e) => {
    const file = e.target.files[0];
    if (file) {
      const previewUrl = URL.createObjectURL(file);
      setFormData((prev) => ({ ...prev, img: previewUrl }));
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log("Saving Product:", formData);
    // Повертаємось на правильну вкладку
    localStorage.setItem("adminActiveTab", isGiftMode ? "gifts" : "bouquets");
    navigate("/admin");
  };

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

              <div className="abf-field">
                <label>Price (₴)</label>
                <input
                  type="number"
                  name="price"
                  value={formData.price}
                  onChange={handleChange}
                  placeholder="0"
                />
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
            </div>

            {/* CATEGORIES */}
            <div className="abf-card">
              <h3 className="abf-card-title">Categories</h3>

              {/* Events */}
              <div className="abf-cat-group">
                <h4>Events</h4>
                <div className="abf-tags">
                  {CATEGORIES.events.map((item) => (
                    <label
                      key={item}
                      className={`abf-tag ${
                        formData.events.includes(item) ? "active" : ""
                      }`}>
                      <input
                        type="checkbox"
                        checked={formData.events.includes(item)}
                        onChange={() => handleCheckboxChange("events", item)}
                      />
                      {item}
                    </label>
                  ))}
                </div>
              </div>

              {/* For Who */}
              <div className="abf-cat-group">
                <h4>For Who</h4>
                <div className="abf-tags">
                  {CATEGORIES.forWho.map((item) => (
                    <label
                      key={item}
                      className={`abf-tag ${
                        formData.forWho.includes(item) ? "active" : ""
                      }`}>
                      <input
                        type="checkbox"
                        checked={formData.forWho.includes(item)}
                        onChange={() => handleCheckboxChange("forWho", item)}
                      />
                      {item}
                    </label>
                  ))}
                </div>
              </div>

              {/* Flower Types (Показуємо тільки для букетів) */}
              {!isGiftMode && (
                <div className="abf-cat-group">
                  <h4>Flower Type</h4>
                  <div className="abf-tags">
                    {CATEGORIES.flowerTypes.map((item) => (
                      <label
                        key={item}
                        className={`abf-tag ${
                          formData.flowerTypes.includes(item) ? "active" : ""
                        }`}>
                        <input
                          type="checkbox"
                          checked={formData.flowerTypes.includes(item)}
                          onChange={() =>
                            handleCheckboxChange("flowerTypes", item)
                          }
                        />
                        {item}
                      </label>
                    ))}
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
