import { useState, useMemo, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import catalogService from "../../../services/catalogService";
import toast from "react-hot-toast";
import { useConfirm } from "../../../context/ModalProvider";

// ...

export function useAdminProducts(active) {
  const navigate = useNavigate();
  const confirm = useConfirm();
  const [products, setProducts] = useState([]);
  const [loadingProducts, setLoadingProducts] = useState(false);
  const [q, setQ] = useState("");

  const fetchProducts = async () => {
    try {
      setLoadingProducts(true);
      let data = [];

      if (active === "bouquets") {
        data = await catalogService.getBouquets();
      } else if (active === "gifts") {
        data = await catalogService.getGifts();
      } else {
        setProducts([]);
        return;
      }

      const items = data.items || data || [];

      setProducts(
        items.map((item) => ({
          id: item.id,
          title: item.name,
          img: item.mainPhotoUrl || item.imageUrl, // Handle both
          price: `${item.price} ₴`,
          category: active === "bouquets" ? "Bouquets" : "Gifts",
        })),
      );
    } catch (error) {
      console.error(`Failed to fetch ${active}:`, error);
      toast.error(`Failed to load ${active}`);
    } finally {
      setLoadingProducts(false);
    }
  };

  useEffect(() => {
    if (active === "bouquets" || active === "gifts") {
      fetchProducts();
    }
  }, [active]);

  const handleDeleteProduct = (id) => {
    confirm({
      title: "Delete item?",
      message:
        "Are you sure you want to delete this item? This action cannot be undone.",
      confirmText: "Delete",
      confirmType: "danger",
      onConfirm: async () => {
        try {
          if (active === "bouquets") {
            await catalogService.deleteBouquet(id);
          } else if (active === "gifts") {
            await catalogService.deleteGift(id);
          }
          toast.success("Item deleted successfully");
          fetchProducts();
        } catch (error) {
          console.error("Failed to delete item:", error);
          toast.error("Failed to delete item");
        }
      },
    });
  };

  const handleAddProduct = () => {
    navigate(`/admin/${active}/new`);
  };

  const handleEditProduct = (product) => {
    navigate(`/admin/${active}/edit/${product.id}`);
  };

  const filteredProducts = useMemo(() => {
    let data = products;
    // Filtering logic would go here if we had distinct API or properties
    const s = q.trim().toLowerCase();
    if (!s) return data;
    return data.filter((b) => b.title.toLowerCase().includes(s));
  }, [q, products, active]);

  return {
    products: filteredProducts,
    loadingProducts,
    q,
    setQ,
    refreshProducts: fetchProducts,
    // CRUD
    handleDeleteProduct,
    handleAddProduct,
    handleEditProduct,
  };
}
