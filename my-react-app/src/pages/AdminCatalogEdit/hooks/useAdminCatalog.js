import { useState, useEffect, useCallback } from "react";
import catalogService from "../../../services/catalogService";
import toast from "react-hot-toast";
import { useTranslation } from "react-i18next";
import { getLocalizedValue } from "../../../utils/localizationUtils";

export const useAdminCatalog = (confirm) => {
  const { t, i18n } = useTranslation();
  const [loading, setLoading] = useState(true);

  // Data State
  const [data, setData] = useState({
    events: [],
    forWho: [],
    flowerTypes: [],
  });

  // Input State
  const [inputs, setInputs] = useState({
    events_ua: "",
    events_en: "",
    forWho_ua: "",
    forWho_en: "",
    flowerTypesName_ua: "",
    flowerTypesName_en: "",
    flowerTypesQuantity: "",
  });

  // Editing State
  const [editingFlower, setEditingFlower] = useState(null);

  // Helper to normalize any catalog item
  const normalizeItem = useCallback((item) => {
    if (!item || typeof item !== "object") return null;

    // Aggressive ID search
    const id =
      item.id ||
      item.Id ||
      item.ID ||
      item.itemId ||
      item.flowerId ||
      item.eventId ||
      item.recipientId;

    // Aggressive Name search
    const name = item.name || item.Name || item.title || item.Title || {};

    // Aggressive Quantity search
    const quantity =
      item.quantity ?? item.Quantity ?? item.qty ?? item.Qty ?? 0;

    return { id, name, quantity };
  }, []);

  // Helper to sort flowers by name (UA first)
  const sortFlowers = useCallback(
    (list) => {
      return [...list].sort((a, b) => {
        const nameA = getLocalizedValue(a.name, i18n.language) || "";
        const nameB = getLocalizedValue(b.name, i18n.language) || "";
        return nameA.localeCompare(nameB);
      });
    },
    [i18n.language],
  );

  // Fetch Data
  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      const [events, recipients, flowers] = await Promise.all([
        catalogService.getEvents(),
        catalogService.getRecipients(),
        catalogService.getFlowers(),
      ]);

      const normalize = (res) => {
        if (!res) return [];
        // Handle both camelCase and PascalCase
        const items = res.items || res.Items || res;
        return Array.isArray(items) ? items : [];
      };

      const eventsData = normalize(events).map(normalizeItem);
      const recipientsData = normalize(recipients).map(normalizeItem);
      const flowersData = normalize(flowers).map(normalizeItem);

      setData({
        events: eventsData,
        forWho: recipientsData,
        flowerTypes: sortFlowers(flowersData),
      });
    } catch (error) {
      console.error("Failed to fetch catalog settings:", error);
      toast.error(t("toasts.admin_load_error"));
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
      const nameUa = inputs.flowerTypesName_ua.trim();
      const nameEn = inputs.flowerTypesName_en.trim();
      const qty = parseInt(inputs.flowerTypesQuantity) || 0;

      if (!nameUa) {
        toast.error(t("toasts.admin_ua_name_required"));
        return;
      }

      const exists = data.flowerTypes.some((item) => {
        const iName = typeof item.name === "object" ? item.name.ua : item.name;
        return iName.toLowerCase() === nameUa.toLowerCase();
      });

      if (exists) {
        toast.error(t("toasts.admin_flower_exists"));
        return;
      }

      try {
        const nameObj = { ua: nameUa, en: nameEn || nameUa };
        const res = await catalogService.createFlower(nameObj, qty);
        const newItem = normalizeItem(res);

        setData((prev) => ({
          ...prev,
          flowerTypes: sortFlowers([...prev.flowerTypes, newItem]),
        }));
        setInputs((prev) => ({
          ...prev,
          flowerTypesName_ua: "",
          flowerTypesName_en: "",
          flowerTypesQuantity: "",
        }));
        toast.success(t("toasts.admin_flower_added"));
      } catch (error) {
        console.error("Failed to add flower:", error);
        toast.error(t("toasts.admin_flower_add_failed"));
      }
      return;
    }

    // Generic logic
    const valUa = inputs[`${category}_ua`]?.trim();
    const valEn = inputs[`${category}_en`]?.trim();

    if (!valUa) {
      toast.error(t("toasts.admin_ua_name_required"));
      return;
    }

    const exists = data[category].some((item) => {
      const iName = typeof item.name === "object" ? item.name.ua : item.name;
      return iName.toLowerCase() === valUa.toLowerCase();
    });

    if (exists) {
      toast.error(t("toasts.admin_item_exists"));
      return;
    }

    try {
      const nameObj = { ua: valUa, en: valEn || valUa };
      let res;
      switch (category) {
        case "events":
          res = await catalogService.createEvent(nameObj);
          break;
        case "forWho":
          res = await catalogService.createRecipient(nameObj);
          break;
        default:
          return;
      }

      const newItem = normalizeItem(res);
      setData((prev) => ({
        ...prev,
        [category]: [...prev[category], newItem],
      }));
      setInputs((prev) => ({
        ...prev,
        [`${category}_ua`]: "",
        [`${category}_en`]: "",
      }));
      toast.success(t("toasts.admin_added_success"));
    } catch (error) {
      console.error(`Failed to add ${category}:`, error);
      toast.error(t("toasts.admin_add_failed"));
    }
  };

  const handleUpdateFlower = async () => {
    if (!editingFlower) return;
    const id = editingFlower.id || editingFlower.Id;
    const nameData = editingFlower.name || editingFlower.Name;

    // Fallback validation
    const uaName = getLocalizedValue(nameData, "ua");
    if (!uaName?.trim()) {
      toast.error(t("toasts.admin_ua_name_required"));
      return;
    }

    const updatePromise = catalogService.updateFlower(
      id,
      nameData,
      editingFlower.quantity ?? editingFlower.Quantity,
    );

    toast.promise(updatePromise, {
      loading: t("admin.saving"),
      success: (res) => {
        let updated = normalizeItem(res);

        // Safety Merge: if backend returns hollow data, preserve existing state
        if (!updated || !updated.id || !getLocalizedValue(updated.name, "ua")) {
          console.warn("Partial data from backend, merging with state", res);
          updated = {
            ...editingFlower,
            ...(updated || {}),
            id: id,
            name:
              updated && getLocalizedValue(updated.name, "ua")
                ? updated.name
                : nameData,
          };
        }

        setData((prev) => ({
          ...prev,
          flowerTypes: sortFlowers(
            prev.flowerTypes.map((f) => ((f.id || f.Id) === id ? updated : f)),
          ),
        }));
        setEditingFlower(null);
        return t("toasts.admin_flower_updated");
      },
      error: (err) => {
        console.error("Update Failed:", err);
        return t("toasts.admin_flower_update_failed");
      },
    });
  };

  const handleRemoveClick = (category, item) => {
    const nameData = item.name || item.Name;
    const itemName = getLocalizedValue(nameData, i18n.language) || "Item";
    const itemId = item.id || item.Id;

    confirm({
      title: t("admin.catalog.confirm_delete_title", { name: itemName }),
      message: t("admin.catalog.confirm_delete_msg"),
      confirmText: t("admin.delete"),
      confirmType: "danger",
      onConfirm: async () => {
        try {
          switch (category) {
            case "events":
              await catalogService.deleteEvent(itemId);
              break;
            case "forWho":
              await catalogService.deleteRecipient(itemId);
              break;
            case "flowerTypes":
              await catalogService.deleteFlower(itemId);
              break;
            default:
              return;
          }

          setData((prev) => ({
            ...prev,
            [category]: prev[category].filter((i) => (i.id || i.Id) !== itemId),
          }));
          toast.success(t("toasts.admin_deleted_success"));
        } catch (error) {
          console.error(`Failed to delete ${category}:`, error);
          toast.error(t("toasts.admin_delete_failed"));
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
