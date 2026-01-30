import { useState, useMemo, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import catalogService from "../../../services/catalogService";
import toast from "react-hot-toast";
import { useTranslation } from "react-i18next";
import { useConfirm } from "../../../context/ModalProvider";
import { useDebounce } from "../../../hooks/useDebounce";
import { extractErrorMessage } from "../../../utils/errorUtils";
import { getLocalizedValue } from "../../../utils/localizationUtils";

// ...

export function useAdminProducts(active) {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const confirm = useConfirm();
  const [products, setProducts] = useState([]);
  const [loadingProducts, setLoadingProducts] = useState(false);
  const [q, setQ] = useState("");
  const debouncedQ = useDebounce(q.trim(), 500);

  const [pagination, setPagination] = useState({
    pageNumber: 1,
    pageSize: 12,
    totalCount: 0,
    totalPages: 1,
  });

  const fetchProducts = async (isLoadMore = false) => {
    try {
      setLoadingProducts(true);
      let data = [];

      const params = {
        Page: isLoadMore ? pagination.pageNumber : 1,
        PageSize: pagination.pageSize,
        Name: debouncedQ || undefined,
      };

      // NOTE: We're assuming catalogService.getBouquets() accepts params object.
      // If it currently takes no args, we might need to check catalogService.js.
      // Checking local file viewing history, catalogService methods likely take params derived from usage patterns.

      if (active === "bouquets") {
        data = await catalogService.getBouquets(params);
        // Sort by CreatedAt Desc (newest first)
        if (Array.isArray(data.items || data)) {
          // Handle both paginated 'items' and direct array
          const list = data.items || data;
          list.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
        }
      } else if (active === "gifts") {
        data = await catalogService.getGifts(params, {
          headers: { "X-Skip-Localization": "true" },
        });
        // Sort by UpdatedAt or CreatedAt Desc (newest or recently edited first)
        if (Array.isArray(data)) {
          data.sort((a, b) => {
            const dateA = new Date(a.updatedAt || a.createdAt);
            const dateB = new Date(b.updatedAt || b.createdAt);
            return dateB - dateA;
          });
        }
      } else {
        setProducts([]);
        return;
      }

      const items = data.items || data || [];

      const optimizeCloudinaryUrl = (url) => {
        if (!url || !url.includes("cloudinary.com")) return url;
        // Avoid double optimization if already present (basic check)
        if (url.includes("/w_")) return url;
        const parts = url.split("upload/");
        if (parts.length < 2) return url;
        return `${parts[0]}upload/w_400,h_400,c_fill,q_auto,f_auto/${parts[1]}`;
      };

      const mapped = items.map((item) => ({
        id: item.id,
        title: getLocalizedValue(item.name, i18n.language),
        img: optimizeCloudinaryUrl(
          item.mainPhotoUrl || item.imageUrl || item.ImageUrl,
        ), // Handle all cases
        price: `${item.price} ₴`,
        stock: item.availableCount, // Added stock
        category:
          active === "bouquets"
            ? t("admin.catalog.events")
            : t("admin.catalog.for_who"), // Just a label placeholder or use actual category if needed
      }));

      if (isLoadMore) {
        setProducts((prev) => [...prev, ...mapped]);
      } else {
        setProducts(mapped);
      }

      setPagination((prev) => ({
        ...prev,
        totalCount: data.totalCount || 0,
        totalPages: Math.ceil((data.totalCount || 0) / prev.pageSize),
      }));
    } catch (error) {
      console.error(`Failed to fetch ${active}:`, error);
      toast.error(
        extractErrorMessage(
          error,
          t("toasts.admin_products_load_failed", { type: active }),
        ),
      );
    } finally {
      setLoadingProducts(false);
    }
  };

  useEffect(() => {
    if (active === "bouquets" || active === "gifts") {
      // Reset pagination on tab, search or language change
      setPagination((p) => ({ ...p, pageNumber: 1 }));
      setProducts([]); // Clear existing
    }
  }, [active, debouncedQ, i18n.language]);

  // Initial fetch when active changes, page number resets to 1, or search query changes
  useEffect(() => {
    if (
      (active === "bouquets" || active === "gifts") &&
      pagination.pageNumber === 1
    ) {
      fetchProducts(false);
    }
  }, [active, pagination.pageNumber, debouncedQ, i18n.language]); // Add pagination.pageNumber, debouncedQ and i18n.language to dependencies

  const loadMore = () => {
    if (pagination.pageNumber < pagination.totalPages && !loadingProducts) {
      setPagination((prev) => ({ ...prev, pageNumber: prev.pageNumber + 1 }));
    }
  };

  // Trigger fetch on page increment
  useEffect(() => {
    if (pagination.pageNumber > 1) {
      fetchProducts(true);
    }
  }, [pagination.pageNumber]);

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
          toast.success(t("toasts.admin_deleted_success"));
          fetchProducts();
        } catch (error) {
          console.error("Failed to delete item:", error);
          toast.error(
            extractErrorMessage(error, t("toasts.admin_delete_failed")),
          );
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

  const finalProducts = useMemo(() => {
    if (active === "gifts") {
      const s = q.trim().toLowerCase();
      if (!s) return products;
      return products.filter((b) => b.title.toLowerCase().includes(s));
    }
    return products;
  }, [q, products, active]);

  return {
    products: finalProducts,
    loadingProducts,
    q,
    setQ,
    refreshProducts: fetchProducts,
    // CRUD
    handleDeleteProduct,
    handleAddProduct,
    handleEditProduct,
    loadMore,
    hasNextPage: pagination.pageNumber < pagination.totalPages,
    isLoadingMore: loadingProducts && pagination.pageNumber > 1,
  };
}
