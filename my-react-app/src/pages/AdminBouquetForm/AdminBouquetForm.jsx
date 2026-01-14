import React, { useState, useEffect } from "react";
import { useNavigate, useParams, useLocation } from "react-router-dom";
import axiosClient from "../../api/axiosClient";
import toast from "react-hot-toast";
import "./AdminBouquetForm.css";

export default function AdminBouquetForm() {
  const navigate = useNavigate();
  const { id } = useParams();
  const location = useLocation();

  const isEditMode = Boolean(id);
  const isGiftMode = location.pathname.includes("gifts");

  // Metadata from API
  const [events, setEvents] = useState([]);
  const [recipients, setRecipients] = useState([]);
  const [flowers, setFlowers] = useState([]);
  const [sizes, setSizes] = useState([]);
  const [loading, setLoading] = useState(true);

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

  // Fetch metadata on mount
  useEffect(() => {
    const fetchMetadata = async () => {
      try {
        setLoading(true);
        const [eventsRes, recipientsRes, flowersRes, sizesRes] =
          await Promise.all([
            axiosClient.get("/api/catalog/sizes"),
            axiosClient.get("/api/catalog/flowers"),
            axiosClient.get("/api/catalog/recipients"),
            axiosClient.get("/api/catalog/events"),
          ]);

        setEvents(eventsRes.data);
        setRecipients(recipientsRes.data);
        setFlowers(flowersRes.data);
        setSizes(sizesRes.data);
      } catch (error) {
        console.error("Failed to fetch metadata:", error);
        toast.error("Failed to load form data");
      } finally {
        setLoading(false);
      }
    };

    fetchMetadata();
  }, []);

  // Load bouquet data in edit mode
  useEffect(() => {
    if (isEditMode && !loading) {
      const fetchBouquet = async () => {
        try {
          const response = await axiosClient.get(`/api/catalog/bouquets/${id}`);
          const data = response.data;

          setFormData({
            title: data.name,
            price: data.sizes[0]?.price || "",
            description: data.description || "",
            category: isGiftMode ? "Gifts" : "Bouquets",
            events: data.events.map((e) => e.id),
            forWho: data.recipients.map((r) => r.id),
            flowerTypes: data.sizes[0]?.flowers.map((f) => f.id) || [],
            img: data.mainPhotoUrl,
            imgFile: null,
          });
        } catch (error) {
          console.error("Failed to fetch bouquet:", error);
          toast.error("Failed to load bouquet data");
        }
      };

      fetchBouquet();
    }
  }, [isEditMode, id, isGiftMode, loading]);

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
      setFormData((prev) => ({ ...prev, img: previewUrl, imgFile: file }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const data = new FormData();
      data.append("Name", formData.title);
      data.append("Description", formData.description || "");

      // Add event IDs
      formData.events.forEach((eventId) => {
        data.append("EventIds", eventId);
      });

      // Add recipient IDs
      formData.forWho.forEach((recipientId) => {
        data.append("RecipientIds", recipientId);
      });

      // Create single size (M) with price
      console.log("Available sizes:", sizes);
      const sizeM = sizes.find((s) => s.name === "M" || s.sizeName === "M");
      if (!sizeM) {
        toast.error("Size M not found");
        return;
      }

      data.append("Sizes[0].SizeId", sizeM.id);
      data.append("Sizes[0].Price", formData.price);

      // Add selected flowers with quantity 1
      formData.flowerTypes.forEach((flowerId, index) => {
        data.append(`Sizes[0].FlowerIds[${index}]`, flowerId);
        data.append(`Sizes[0].FlowerQuantities[${index}]`, 1);
      });

      // Add images
      if (formData.imgFile) {
        data.append("MainPhoto", formData.imgFile);
        data.append("Sizes[0].MainImage", formData.imgFile);
      } else if (!isEditMode) {
        toast.error("Please upload an image");
        return;
      }

      if (isEditMode) {
        await axiosClient.put(`/api/catalog/bouquets/${id}`, data, {
          headers: { "Content-Type": "multipart/form-data" },
        });
        toast.success("Bouquet updated successfully!");
      } else {
        await axiosClient.post("/api/catalog/bouquets", data, {
          headers: { "Content-Type": "multipart/form-data" },
        });
        toast.success("Bouquet created successfully!");
      }

      localStorage.setItem("adminActiveTab", isGiftMode ? "gifts" : "bouquets");
      navigate("/admin");
    } catch (error) {
      console.error("Failed to save bouquet:", error);
      toast.error(error.response?.data?.message || "Failed to save bouquet");
    }
  };

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
                  {events.map((item) => (
                    <label
                      key={item.id}
                      className={`abf-tag ${
                        formData.events.includes(item.id) ? "active" : ""
                      }`}>
                      <input
                        type="checkbox"
                        checked={formData.events.includes(item.id)}
                        onChange={() => handleCheckboxChange("events", item.id)}
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
                        onChange={() => handleCheckboxChange("forWho", item.id)}
                      />
                      {item.name}
                    </label>
                  ))}
                </div>
              </div>

              {/* Flower Types (Показуємо тільки для букетів) */}
              {!isGiftMode && (
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
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
