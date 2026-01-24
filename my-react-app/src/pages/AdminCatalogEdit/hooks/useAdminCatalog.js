import { useState, useEffect, useCallback } from "react";
import catalogService from "../../../services/catalogService";
import toast from "react-hot-toast";

export const useAdminCatalog = (confirm) => {
  const [loading, setLoading] = useState(true);

  // Data State
  const [data, setData] = useState({
    events: [],
    forWho: [],
    flowerTypes: [],
  });

  // Input State
  const [inputs, setInputs] = useState({
    events: "",
    forWho: "",
    flowerTypesName: "",
    flowerTypesQuantity: "",
  });

  // Editing State
  const [editingFlower, setEditingFlower] = useState(null);

  // Fetch Data
  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      const [events, recipients, flowers] = await Promise.all([
        catalogService.getEvents(),
        catalogService.getRecipients(),
        catalogService.getFlowers(),
      ]);

      const normalize = (res) => res.items || res || [];

      setData({
        events: normalize(events),
        forWho: normalize(recipients),
        flowerTypes: normalize(flowers).sort((a, b) =>
          a.name.localeCompare(b.name),
        ),
      });
    } catch (error) {
      console.error("Failed to fetch catalog settings:", error);
      toast.error("Failed to load settings");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  // Handlers
  const handleInputChange = (field, value) => {
    setInputs((prev) => ({ ...prev, [field]: value }));
  };

  const handleAdd = async (category) => {
    if (category === "flowerTypes") {
      const name = inputs.flowerTypesName.trim();
      const qty = parseInt(inputs.flowerTypesQuantity) || 0;

      if (!name) return;

      const exists = data.flowerTypes.some(
        (item) => item.name.toLowerCase() === name.toLowerCase(),
      );
      if (exists) {
        toast.error("This flower type already exists!");
        return;
      }

      try {
        const newItem = await catalogService.createFlower(name, qty);
        setData((prev) => ({
          ...prev,
          flowerTypes: [...prev.flowerTypes, newItem].sort((a, b) =>
            a.name.localeCompare(b.name),
          ),
        }));
        setInputs((prev) => ({
          ...prev,
          flowerTypesName: "",
          flowerTypesQuantity: "",
        }));
        toast.success("Flower added successfully");
      } catch (error) {
        console.error("Failed to add flower:", error);
        toast.error("Failed to add flower");
      }
      return;
    }

    // Generic logic
    const val = inputs[category]?.trim();
    if (!val) return;

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
        default:
          return;
      }

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

  const handleUpdateFlower = async () => {
    if (!editingFlower || !editingFlower.name.trim()) return;

    try {
      const updated = await catalogService.updateFlower(
        editingFlower.id,
        editingFlower.name,
        editingFlower.quantity,
      );

      setData((prev) => ({
        ...prev,
        flowerTypes: prev.flowerTypes
          .map((f) => (f.id === editingFlower.id ? updated : f))
          .sort((a, b) => a.name.localeCompare(b.name)),
      }));
      setEditingFlower(null);
      toast.success("Flower updated");
    } catch (error) {
      console.error("Failed to update flower:", error);
      toast.error("Failed to update flower");
    }
  };

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

  return {
    loading,
    data,
    inputs,
    editingFlower,
    setEditingFlower,
    handleInputChange,
    handleAdd,
    handleUpdateFlower,
    handleRemoveClick,
  };
};
