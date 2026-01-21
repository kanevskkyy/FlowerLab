import { useState, useEffect } from "react";
import catalogService from "../../../services/catalogService";
import { useCart } from "../../../context/CartContext";

export function useCatalog() {
  const { addToCart } = useCart();

  const [sortOpen, setSortOpen] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const [filterOpen, setFilterOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [sortBy, setSortBy] = useState("popularity");
  const [currentPage, setCurrentPage] = useState(1);
  const [filters, setFilters] = useState(null);

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

  // Actions
  const applyFilters = (filterData) => {
    setFilters(filterData);
    setCurrentPage(1);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleSortChange = (sortOption) => {
    setSortBy(sortOption);
    setSortOpen(false);
    setCurrentPage(1);
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
