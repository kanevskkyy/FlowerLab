import { useState, useEffect } from "react";
import catalogService from "../../../services/catalogService";
import toast from "react-hot-toast";

export function useGifts() {
  // Pagination State
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalProducts, setTotalProducts] = useState(0);
  const [gifts, setGifts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);
  const ITEMS_PER_PAGE = 9;

  useEffect(() => {
    const fetchGifts = async () => {
      try {
        setLoading(true);

        const params = {
          Page: currentPage,
          PageSize: ITEMS_PER_PAGE,
        };

        const data = await catalogService.getGifts(params);

        // Handle PagedList response
        const items = data.items || data || [];

        setGifts(items);
        setTotalProducts(data.totalCount || items.length);
        setTotalPages(data.totalPages || 1);
      } catch (error) {
        console.error("Failed to fetch gifts:", error);
        setError(true);
        toast.error("Не вдалося завантажити подарунки");
      } finally {
        setLoading(false);
      }
    };

    fetchGifts();
  }, [currentPage]);

  const handlePageChange = (page) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleLoadMore = () => {
    if (currentPage < totalPages) {
      setCurrentPage((prev) => prev + 1);
    }
  };

  return {
    gifts,
    loading,
    error,
    currentPage,
    totalPages,
    totalProducts,
    handlePageChange,
    handleLoadMore,
  };
}
