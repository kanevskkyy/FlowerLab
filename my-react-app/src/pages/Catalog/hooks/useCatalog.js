import { useState, useEffect, useMemo, useRef } from "react";
import { useSearchParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import catalogService from "../../../services/catalogService";
import { useCart } from "../../../context/CartContext";
import { useDebounce } from "../../../hooks/useDebounce";

export function useCatalog() {
  const { i18n } = useTranslation();
  const { addToCart } = useCart();
  const [searchParams, setSearchParams] = useSearchParams();

  const [sortOpen, setSortOpen] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const [filterOpen, setFilterOpen] = useState(false);

  // --- Local Search State (for smooth typing) ---
  const [searchTerm, setSearchTerm] = useState(
    searchParams.get("search") || "",
  );
  const debouncedSearchQuery = useDebounce(searchTerm.trim(), 500);

  // Sync URL -> Local state (e.g. on browser back button)
  useEffect(() => {
    const urlSearch = searchParams.get("search") || "";
    if (urlSearch !== searchTerm) {
      setSearchTerm(urlSearch);
    }
  }, [searchParams]);

  // Sync Local debounced -> URL
  useEffect(() => {
    setSearchParams((prev) => {
      const currentUrlSearch = prev.get("search") || "";
      if (debouncedSearchQuery !== currentUrlSearch) {
        if (debouncedSearchQuery) prev.set("search", debouncedSearchQuery);
        else prev.delete("search");
        prev.set("page", "1");
      }
      return prev;
    });
  }, [debouncedSearchQuery]);

  const searchQuery = searchTerm; // For the input value
  const sortBy = searchParams.get("sortBy") || "popularity";
  const currentPageRaw = parseInt(searchParams.get("page") || "1", 10);
  const currentPage = isNaN(currentPageRaw) ? 1 : currentPageRaw;

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
  const isLoadMoreMode = useRef(false);

  // Fetch bouquets from API
  useEffect(() => {
    const fetchBouquets = async () => {
      setLoading(true);
      try {
        const params = {};
        if (debouncedSearchQuery && debouncedSearchQuery.trim() !== "") {
          params.Name = debouncedSearchQuery;
        }
        params.SortBy = sortBy;

        // Simple Pagination Logic
        let fetchPage = currentPage;
        let fetchSize = ITEMS_PER_PAGE;

        // Smart Restoration
        const viewMode = sessionStorage.getItem("viewMode");
        const isRestoringAppend =
          viewMode === "append" && products.length === 0 && currentPage > 1;

        if (isRestoringAppend) {
          fetchPage = 1;
          fetchSize = currentPage * ITEMS_PER_PAGE;
        }

        params.PageSize = fetchSize;
        params.Page = fetchPage;

        if (filters) {
          Object.entries(filters).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== undefined) {
              // fixed typo in previous check logic if existed
              if (Array.isArray(value)) {
                if (value.length > 0) params[key] = value;
              } else {
                params[key] = value;
              }
            }
          });
        }

        const data = await catalogService.getBouquets(params);
        console.log(
          `[useCatalog] Language: ${i18n.language}, Fetch params:`,
          params,
        );
        console.log(`[useCatalog] Raw Data:`, data);

        if (data.items) {
          const langNormalized = i18n.language
            ? i18n.language.toLowerCase()
            : "";
          const currentLang =
            langNormalized.startsWith("uk") || langNormalized === "ua"
              ? "ua"
              : "en";

          const mappedItems = data.items.map((p) => {
            let minPriceSize = null;
            if (p.sizes && p.sizes.length > 0) {
              const sortedSizes = [...p.sizes].sort(
                (a, b) => a.price - b.price,
              );
              minPriceSize = sortedSizes[0];
            }

            const getLocalized = (val) => {
              if (typeof val === "object" && val !== null) {
                return val[currentLang] || val.ua || val.en || "";
              }
              return val || "";
            };

            return {
              id: p.id,
              bouquetId: p.id,
              sizeId: minPriceSize ? minPriceSize.sizeId : null,
              sizeName: minPriceSize
                ? getLocalized(minPriceSize.sizeName)
                : t("product.standard_size", { defaultValue: "Standard" }),
              title: getLocalized(p.name),
              price: minPriceSize ? `${minPriceSize.price} ₴` : `${p.price} ₴`,
              img: p.mainPhotoUrl,
            };
          });

          setProducts((prev) => {
            if (currentPage === 1 || prev.length === 0 || isRestoringAppend) {
              return mappedItems;
            }

            if (isLoadMoreMode.current) {
              // Append
              const existingIds = new Set(prev.map((p) => p.id));
              const uniqueNewItems = mappedItems.filter(
                (p) => !existingIds.has(p.id),
              );
              return [...prev, ...uniqueNewItems];
            }

            // Replace
            return mappedItems;
          });

          setTotalProducts(data.totalCount);
          setTotalPages(Math.ceil(data.totalCount / ITEMS_PER_PAGE));
        } else {
          // Handle empty/fallback
          if (currentPage === 1) {
            setProducts([]);
            setTotalProducts(0);
            setTotalPages(1);
          }
          // if page > 1 and no items, keeps previous.
        }
      } catch (error) {
        console.error("Failed to fetch bouquets:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchBouquets();
  }, [filters, sortBy, currentPage, debouncedSearchQuery, i18n.language]);

  // --- Actions that update URL ---

  const setSearchQuery = (term) => {
    setSearchTerm(term);
    // URL will be updated by the useEffect above once debounced
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
      prev.set("page", "1");
      // ... existing logic ...
      if (newFilters.minPrice) prev.set("minPrice", newFilters.minPrice);
      else prev.delete("minPrice");

      if (newFilters.maxPrice) prev.set("maxPrice", newFilters.maxPrice);
      else prev.delete("maxPrice");

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
    setFilterOpen(false);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleSortChange = (sortOption) => {
    setSortBy(sortOption);
    setSortOpen(false);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleLoadMore = () => {
    if (currentPage < totalPages) {
      isLoadMoreMode.current = true;
      sessionStorage.setItem("viewMode", "append");
      setCurrentPage(currentPage + 1);
      // Removed window.scrollTo to prevent jumping up
    }
  };

  const goToPage = (page) => {
    isLoadMoreMode.current = false;
    sessionStorage.setItem("viewMode", "replace");
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
