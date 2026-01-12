import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "./AdminCatalogEdit.css";

// Початкові дані (імітація бази даних)
const INITIAL_DATA = {
  events: ["Birthday", "Wedding", "Engagement", "Anniversary"],
  forWho: ["Mom", "Wife", "Husband", "Kid", "Teacher", "Co-worker"],
  flowerTypes: ["Peony", "Rose", "Lily", "Tulip", "Orchid", "Hydrangea"],
};

export default function AdminCatalogEdit() {
  const navigate = useNavigate();

  // Стан для списків
  const [data, setData] = useState({
    events: [],
    forWho: [],
    flowerTypes: [],
  });

  // Стан для полів вводу (окремо для кожної категорії)
  const [inputs, setInputs] = useState({
    events: "",
    forWho: "",
    flowerTypes: "",
  });

  // Завантаження даних
  useEffect(() => {
    // Імітуємо запит на сервер
    setTimeout(() => {
      setData(INITIAL_DATA);
    }, 100);
  }, []);

  // Додавання нового тегу
  const handleAdd = (category) => {
    const val = inputs[category].trim();
    if (!val) return;

    if (data[category].includes(val)) {
      alert("This item already exists!");
      return;
    }

    setData((prev) => ({
      ...prev,
      [category]: [...prev[category], val],
    }));

    setInputs((prev) => ({ ...prev, [category]: "" })); // Очищаємо інпут
  };

  // Видалення тегу
  const handleRemove = (category, item) => {
    setData((prev) => ({
      ...prev,
      [category]: prev[category].filter((i) => i !== item),
    }));
  };

  // Обробка введення тексту (Enter також додає)
  const handleKeyDown = (e, category) => {
    if (e.key === "Enter") {
      e.preventDefault();
      handleAdd(category);
    }
  };

  const handleSave = () => {
    console.log("Saving catalog settings:", data);
    // Тут логіка відправки на сервер
    navigate("/admin");
  };

  // Компонент для однієї секції
  const renderSection = (title, categoryKey) => (
    <div className="ace-card">
      <h3 className="ace-card-title">{title}</h3>

      {/* Список тегів */}
      <div className="ace-tags-list">
        {data[categoryKey].map((item) => (
          <div key={item} className="ace-tag">
            <span>{item}</span>
            <button
              type="button"
              className="ace-tag-remove"
              onClick={() => handleRemove(categoryKey, item)}>
              ✕
            </button>
          </div>
        ))}
        {data[categoryKey].length === 0 && (
          <div className="ace-empty">No items yet</div>
        )}
      </div>

      {/* Поле додавання */}
      <div className="ace-add-row">
        <input
          type="text"
          placeholder={`Add new ${title.toLowerCase()}...`}
          value={inputs[categoryKey]}
          onChange={(e) =>
            setInputs({ ...inputs, [categoryKey]: e.target.value })
          }
          onKeyDown={(e) => handleKeyDown(e, categoryKey)}
        />
        <button
          type="button"
          className="ace-add-btn"
          onClick={() => handleAdd(categoryKey)}>
          Add
        </button>
      </div>
    </div>
  );

  return (
    <div className="ace-page">
      <div className="ace-container">
        {/* HEADER */}
        <header className="ace-header">
          <button className="ace-back-btn" onClick={() => navigate("/admin")}>
            ← Cancel
          </button>
          <h1 className="ace-title">Edit Catalog Settings</h1>
          <button className="ace-save-btn" onClick={handleSave}>
            Save Changes
          </button>
        </header>

        {/* CONTENT */}
        <div className="ace-content">
          {renderSection("Events", "events")}
          {renderSection("For Who", "forWho")}
          {renderSection("Flower Types", "flowerTypes")}
        </div>
      </div>
    </div>
  );
}
