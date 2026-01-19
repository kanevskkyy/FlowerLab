import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import catalogService from "../../services/catalogService";
import toast from "react-hot-toast";
import { useConfirm } from "../../context/ModalProvider";
import "./AdminCatalogEdit.css";

export default function AdminCatalogEdit() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);

  // Data State (items are objects { id, name })
  const [data, setData] = useState({
    events: [],
    forWho: [],
    flowerTypes: [],
  });

  // Input State
  const [inputs, setInputs] = useState({
    events: "",
    forWho: "",
    flowerTypes: "",
  });

  const confirm = useConfirm();

  // Fetch Data
  const fetchData = async () => {
    try {
      setLoading(true);
      const [events, recipients, flowers] = await Promise.all([
        catalogService.getEvents(),
        catalogService.getRecipients(),
        catalogService.getFlowers(),
      ]);

      // Helper to normalize data (ensure we have objects or consistently handle strings if API returns strings)
      // Assuming API returns objects { id, name } or lists of such objects
      const normalize = (res) => res.items || res || [];

      setData({
        events: normalize(events),
        forWho: normalize(recipients),
        flowerTypes: normalize(flowers),
      });
    } catch (error) {
      console.error("Failed to fetch catalog settings:", error);
      toast.error("Failed to load settings");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  // ADD Handlers
  const handleAdd = async (category) => {
    const val = inputs[category].trim();
    if (!val) return;

    // Optimistic duplicate check (by name)
    const exists = data[category].some(
      (item) => item.name.toLowerCase() === val.toLowerCase(),
    );
    if (exists) {
      toast.error("This item already exists!");
      return;
    }

    try {
      let newItem;
      switch (category) {
        case "events":
          newItem = await catalogService.createEvent(val);
          break;
        case "forWho":
          newItem = await catalogService.createRecipient(val);
          break;
        case "flowerTypes":
          newItem = await catalogService.createFlower(val);
          break;
        default:
          return;
      }

      // Update State
      setData((prev) => ({
        ...prev,
        [category]: [...prev[category], newItem],
      }));
      setInputs((prev) => ({ ...prev, [category]: "" }));
      toast.success("Added successfully");
    } catch (error) {
      console.error(`Failed to add ${category}:`, error);
      toast.error("Failed to add item");
    }
  };

  // REMOVE Handlers (Trigger Modal)
  const handleRemoveClick = (category, item) => {
    confirm({
      title: `Delete "${item.name}"?`,
      message:
        "Are you sure you want to delete this item? This creates potential issues for products using it.",
      confirmText: "Delete",
      confirmType: "danger",
      onConfirm: async () => {
        try {
          switch (category) {
            case "events":
              await catalogService.deleteEvent(item.id);
              break;
            case "forWho":
              await catalogService.deleteRecipient(item.id);
              break;
            case "flowerTypes":
              await catalogService.deleteFlower(item.id);
              break;
            default:
              return;
          }

          // Update State
          setData((prev) => ({
            ...prev,
            [category]: prev[category].filter((i) => i.id !== item.id),
          }));
          toast.success("Deleted successfully");
        } catch (error) {
          console.error(`Failed to delete ${category}:`, error);
          toast.error("Failed to delete item");
        }
      },
    });
  };

  const handleKeyDown = (e, category) => {
    if (e.key === "Enter") {
      e.preventDefault();
      handleAdd(category);
    }
  };

  // Render Section
  const renderSection = (title, categoryKey) => (
    <div className="ace-card">
      <h3 className="ace-card-title">{title}</h3>

      {/* Tags List */}
      <div className="ace-tags-list">
        {data[categoryKey].map((item) => (
          <div key={item.id} className="ace-tag">
            <span>{item.name}</span>
            <button
              type="button"
              className="ace-tag-remove"
              onClick={() => handleRemoveClick(categoryKey, item)}>
              ✕
            </button>
          </div>
        ))}
        {data[categoryKey].length === 0 && (
          <div className="ace-empty">No items yet</div>
        )}
      </div>

      {/* Add Row */}
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

  if (loading) {
    return <div className="ace-page loading">Loading...</div>;
  }

  return (
    <div className="ace-page">
      <div className="ace-container">
        {/* HEADER */}
        <header className="ace-header">
          <button className="ace-back-btn" onClick={() => navigate("/admin")}>
            ← Back to Admin
          </button>
          <h1 className="ace-title">Edit Catalog Settings</h1>
          <div style={{ width: 100 }}></div> {/* Spacer to center title */}
        </header>

        {/* CONTENT */}
        <div className="ace-content">
          {renderSection("Events", "events")}
          {renderSection("For Who (Recipients)", "forWho")}
          {renderSection("Flower Types", "flowerTypes")}
        </div>
      </div>
    </div>
  );
}
