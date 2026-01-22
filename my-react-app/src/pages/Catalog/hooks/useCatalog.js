import { useState, useEffect, useMemo } from "react";
import { useSearchParams } from "react-router-dom";
import catalogService from "../../../services/catalogService";
import { useCart } from "../../../context/CartContext";

export function useCatalog() {
  const { addToCart } = useCart();
  const [searchParams, setSearchParams] = useSearchParams();

  const [sortOpen, setSortOpen] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const [filterOpen, setFilterOpen] = useState(false);

  // --- Derived State from URL ---
  const searchQuery = searchParams.get("search") || "";
  const sortBy = searchParams.get("sortBy") || "popularity";
  const currentPage = parseInt(searchParams.get("page") || "1", 10);

  // Parse filters from URL
  const filters = useMemo(() => {
    const f = {};
    const maxPrice = searchParams.get("maxPrice");
    if (maxPrice) f.maxPrice = Number(maxPrice);

    const sizeIds = searchParams.getAll("sizeIds");
    if (sizeIds.length > 0) f.sizeIds = sizeIds;

    const quantities = searchParams.getAll("quantities");
    if (quantities.length > 0) f.quantities = quantities.map(Number);

    const eventIds = searchParams.getAll("eventIds");
    if (eventIds.length > 0) f.eventIds = eventIds;

    const recipientIds = searchParams.getAll("recipientIds");
    if (recipientIds.length > 0) f.recipientIds = recipientIds;

    const flowerIds = searchParams.getAll("flowerIds");
    if (flowerIds.length > 0) f.flowerIds = flowerIds;

    // Return null if empty to match original logic
    return Object.keys(f).length > 0 ? f : null;
  }, [searchParams.toString()]);

  const [products, setProducts] = useState([]);
  const [totalProducts, setTotalProducts] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);

  const ITEMS_PER_PAGE = 9;

  // Fetch bouquets from API
  useEffect(() => {
    const fetchBouquets = async () => {
      setLoading(true);
      try {
        const params = {};
        if (searchQuery && searchQuery.trim() !== "") {
          params.Name = searchQuery;
        }
        params.SortBy = sortBy;
        params.PageSize = ITEMS_PER_PAGE;
        params.Page = currentPage;

        if (filters) {
          Object.entries(filters).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== "") {
              if (Array.isArray(value)) {
                if (value.length > 0) params[key] = value;
              } else {
                params[key] = value;
              }
            }
          });
        }


        const data = await catalogService.getBouquets(params);

        if (data.items) {
          setProducts(
            data.items.map((p) => {
              // Find size with minimum price (ignoring 0 if possible, or just strict min)
              // Assuming price > 0 is valid.
              let minPriceSize = null;
              if (p.sizes && p.sizes.length > 0) {
                // Sort by price ascending
                const sortedSizes = [...p.sizes].sort(
                  (a, b) => a.price - b.price,
                );
                minPriceSize = sortedSizes[0];
              }

              return {
                id: p.id,
                bouquetId: p.id,
                sizeId: minPriceSize ? minPriceSize.sizeId : null,
                sizeName: minPriceSize ? minPriceSize.sizeName : "Standard",
                title: p.name,
                // Fallback Price Strategy
                price: minPriceSize
                  ? `${minPriceSize.price} ₴`
                  : `${p.price} ₴`,
                img: p.mainPhotoUrl,
              };
            }),
          );
          setTotalProducts(data.totalCount);
          setTotalPages(data.totalPages);
        } else {
          const items = Array.isArray(data) ? data : [];
          setProducts(
            items.map((p) => {
              let minPriceSize = null;
              if (p.sizes && p.sizes.length > 0) {
                const sortedSizes = [...p.sizes].sort(
                  (a, b) => a.price - b.price,
                );
                minPriceSize = sortedSizes[0];
              }
              return {
                id: p.id,
                bouquetId: p.id,
                sizeId: minPriceSize ? minPriceSize.sizeId : null,
                sizeName: minPriceSize ? minPriceSize.sizeName : "Standard",
                title: p.name,
                // Fallback Price Strategy
                price: minPriceSize
                  ? `${minPriceSize.price} ₴`
                  : `${p.price} ₴`,
                img: p.mainPhotoUrl,
              };
            }),
          );
          setTotalProducts(items.length);
          setTotalPages(1);
        }
      } catch (error) {
        console.error("Failed to fetch bouquets:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchBouquets();
  }, [filters, sortBy, currentPage, searchQuery]);

  // --- Actions that update URL ---

  const setSearchQuery = (term) => {
    setSearchParams((prev) => {
      if (term) prev.set("search", term);
      else prev.delete("search");
      prev.set("page", "1"); // Reset to page 1
      return prev;
    });
  };

  const setSortBy = (sort) => {
    setSearchParams((prev) => {
      if (sort) prev.set("sortBy", sort);
      else prev.delete("sortBy");
      prev.set("page", "1");
      return prev;
    });
  };

  const setCurrentPage = (page) => {
    setSearchParams((prev) => {
      prev.set("page", page.toString());
      return prev;
    });
  };

  const applyFilters = (newFilters) => {
    setSearchParams((prev) => {
      // Clear old filter keys first (to avoid appending if we want to replace)
      // Since getAll returns array, we can't easily "replace" without deleting first in some routers,
      // but react-router-dom's set/delete works fine.

      // Reset Page
      prev.set("page", "1");

      // 1. Max Price
      if (newFilters.maxPrice) prev.set("maxPrice", newFilters.maxPrice);
      else prev.delete("maxPrice");

      // 2. Arrays: helper to set or delete
      const updateArrayParam = (key, values) => {
        prev.delete(key);
        if (values && values.length > 0) {
          values.forEach((v) => prev.append(key, v));
        }
      };

      updateArrayParam("sizeIds", newFilters.sizeIds);
      updateArrayParam("quantities", newFilters.quantities);
      updateArrayParam("eventIds", newFilters.eventIds);
      updateArrayParam("recipientIds", newFilters.recipientIds);
      updateArrayParam("flowerIds", newFilters.flowerIds);

      return prev;
    });
    setFilterOpen(false); // Close menu
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleSortChange = (sortOption) => {
    setSortBy(sortOption);
    setSortOpen(false);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleLoadMore = () => {
    if (currentPage < totalPages) {
      setCurrentPage((prev) => prev + 1);
      window.scrollTo({ top: 0, behavior: "smooth" });
    }
  };

  const goToPage = (page) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleAddToCart = (product) => {
    // Safeguard: If sizeId is missing (e.g. backend cache stale), redirect to product page
    if (!product.sizeId) {
      // Optional: Toast message explaining why? Or just redirect.
      // toast.error("Please select a size");
      window.location.href = `/product/${product.id}`; // using window.location to be safe or navigate if available
      return;
    }

    addToCart({
      ...product,
      id: `${product.bouquetId}-${product.sizeName}`,
      qty: 1,
    });
  };

  return {
    // State
    products,
    totalProducts,
    totalPages,
    currentPage,
    loading,
    sortOpen,
    menuOpen,
    filterOpen,
    searchQuery,
    filters, // Exposed derived filters

    // Setters (if needed directly)
    setSortOpen,
    setMenuOpen,
    setFilterOpen,
    setSearchQuery,
    setCurrentPage, // exposed for search input onChange

    // Handlers
    applyFilters,
    handleSortChange,
    handleLoadMore,
    goToPage,
    handleAddToCart,
  };
}
