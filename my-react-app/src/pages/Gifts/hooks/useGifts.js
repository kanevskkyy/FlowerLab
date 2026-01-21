import { useState, useEffect } from "react";
import catalogService from "../../../services/catalogService";
import toast from "react-hot-toast";

export function useGifts() {
  const [gifts, setGifts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchGifts = async () => {
      try {
        setLoading(true);
        const data = await catalogService.getGifts();
        // data.items if paged, or data if list. Usually getGifts returns PagedList or List.
        // Based on AdminProducts, it likely returns { items: [...] } or [...]
        const items = data.items || data || [];
        setGifts(items);
      } catch (error) {
        console.error("Failed to fetch gifts:", error);
        toast.error("Failed to load gifts");
      } finally {
        setLoading(false);
      }
    };

    fetchGifts();
  }, []);

  return {
    gifts,
    loading,
  };
}
