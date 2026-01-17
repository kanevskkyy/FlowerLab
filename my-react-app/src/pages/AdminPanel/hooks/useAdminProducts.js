import { useState, useMemo, useEffect } from "react";
import catalogService from "../../../services/catalogService";
import toast from "react-hot-toast";

export function useAdminProducts(active) {
  const [products, setProducts] = useState([]);
  const [loadingProducts, setLoadingProducts] = useState(false);
  const [q, setQ] = useState("");

  const fetchProducts = async () => {
    try {
      setLoadingProducts(true);
      const data = await catalogService.getBouquets();
      const items = data.items || data;

      setProducts(
        items.map((b) => ({
          id: b.id,
          title: b.name,
          img: b.mainPhotoUrl,
          price: `${b.price} ₴`,
          category: "Bouquets", 
        }))
      );
    } catch (error) {
      console.error("Failed to fetch bouquets:", error);
      toast.error("Failed to load bouquets");
    } finally {
      setLoadingProducts(false);
    }
  };

  useEffect(() => {
    if (active === "bouquets" || active === "gifts") {
      fetchProducts();
    }
  }, [active]);

  const handleDeleteProduct = async (id) => {
    if (window.confirm("Are you sure you want to delete this item?")) {
      try {
        await catalogService.deleteBouquet(id);
        toast.success("Item deleted successfully");
        fetchProducts(); 
      } catch (error) {
        console.error("Failed to delete bouquet:", error);
        toast.error("Failed to delete bouquet");
      }
    }
  };

  const filteredProducts = useMemo(() => {
    let data = products;
    // Filtering logic would go here if we had distinct API or properties
    if (active === "gifts") {
        data = []; // Placeholder
    }
    const s = q.trim().toLowerCase();
    if (!s) return data;
    return data.filter((b) => b.title.toLowerCase().includes(s));
  }, [q, products, active]);

  return {
    products: filteredProducts,
    loadingProducts,
    q,
    setQ,
    handleDeleteProduct,
    refreshProducts: fetchProducts
  };
}
