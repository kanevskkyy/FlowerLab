import { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import catalogService from "../../../services/catalogService";
import toast from "react-hot-toast";
import { extractErrorMessage } from "../../../utils/errorUtils";

export function useAdminCatalog() {
  const { i18n } = useTranslation();
  const [settings, setSettings] = useState({
    events: [],
    forWho: [],
    flowerTypes: [],
  });
  const [loading, setLoading] = useState(false);

  const fetchSettings = async () => {
    setLoading(true);
    try {
      // Execute in parallel
      const [eventsData, recipientsData, flowersData] = await Promise.all([
        catalogService.getEvents(),
        catalogService.getRecipients(),
        catalogService.getFlowers(),
      ]);

      // Assuming API returns arrays directly or { items: [] }
      // Adjust mapping if needed based on real API response
      const mapNames = (data) => {
        const items = data.items || data.Items || data || [];
        return items.map((i) => ({
          ...i,
          id: i.id || i.Id,
          name: i.name || i.Name,
          quantity: i.quantity ?? i.Quantity,
        }));
      };

      setSettings({
        events: mapNames(eventsData),
        forWho: mapNames(recipientsData),
        flowerTypes: mapNames(flowersData),
      });
    } catch (error) {
      console.error("Failed to fetch catalog settings:", error);
      toast.error(
        extractErrorMessage(error, "Failed to load catalog settings"),
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSettings();
  }, [i18n.language]);

  const handleEdit = () => {
    toast("Edit feature coming soon!");
  };

  return {
    settings,
    loading,
    handleEdit,
  };
}
