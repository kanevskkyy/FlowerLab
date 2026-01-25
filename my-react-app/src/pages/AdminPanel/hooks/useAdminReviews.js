import { useState, useEffect } from "react";
import toast from "react-hot-toast";
import reviewService from "../../../services/reviewService";
import { extractErrorMessage } from "../../../utils/errorUtils";

export function useAdminReviews() {
  const [pendingReviews, setPendingReviews] = useState([]);
  const [loading, setLoading] = useState(false);

  const [pagination, setPagination] = useState({
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 1,
  });

  const fetchReviews = async (isLoadMore = false) => {
    setLoading(true);
    try {
      const data = await reviewService.getPendingReviews({
        pageNumber: isLoadMore ? pagination.pageNumber : 1, // Correct page logic handled below/in state
        pageSize: pagination.pageSize,
      });
      // Adjust handling based on your PagedList structure (e.g. data.items or data)
      const rawItems = data.items || data || [];

      const mapped = rawItems.map((r) => ({
        id: r.id,
        name: `${r.user?.firstName || "Unknown"} ${r.user?.lastName || ""}`.trim(),
        stars: r.rating,
        text: r.comment,
        avatar: r.user?.photoUrl || "",
        date: r.createdAt, // if needed
      }));

      if (isLoadMore) {
        setPendingReviews((prev) => [...prev, ...mapped]);
      } else {
        setPendingReviews(mapped);
      }

      setPagination((prev) => ({
        ...prev,
        totalCount: data.totalCount || 0,
        totalPages: Math.ceil((data.totalCount || 0) / prev.pageSize),
      }));
    } catch (error) {
      console.error("Failed to fetch reviews:", error);
      toast.error(extractErrorMessage(error, "Could not load reviews"));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchReviews();
  }, []);

  const loadMore = () => {
    if (pagination.pageNumber < pagination.totalPages && !loading) {
      setPagination((prev) => ({ ...prev, pageNumber: prev.pageNumber + 1 }));
    }
  };

  useEffect(() => {
    if (pagination.pageNumber > 1) {
      fetchReviews(true);
    }
  }, [pagination.pageNumber]);

  const handleApproveReview = async (id) => {
    try {
      await reviewService.approveReview(id);
      setPendingReviews((prev) => prev.filter((r) => r.id !== id));
      toast.success("Review approved!");
    } catch (error) {
      console.error(error);
      toast.error(extractErrorMessage(error, "Failed to approve review"));
    }
  };

  const handleDeleteReview = async (id) => {
    try {
      await reviewService.deleteReview(id);
      setPendingReviews((prev) => prev.filter((r) => r.id !== id));
      toast.success("Review deleted");
    } catch (error) {
      console.error(error);
      toast.error(extractErrorMessage(error, "Failed to delete review"));
    }
  };

  return {
    reviews: pendingReviews,
    loading,
    handleApproveReview,
    handleRejectReview: handleDeleteReview, // Reuse delete for reject
    handleDeleteReview,
    loadMore,
    hasNextPage: pagination.pageNumber < pagination.totalPages,
    isLoadingMore: loading && pagination.pageNumber > 1,
  };
}
