import { useState, useEffect } from "react";
import toast from "react-hot-toast";
import reviewService from "../../../services/reviewService";

export function useAdminReviews() {
  const [pendingReviews, setPendingReviews] = useState([]);
  const [loading, setLoading] = useState(false);

  const fetchReviews = async () => {
    setLoading(true);
    try {
      const data = await reviewService.getPendingReviews();
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

      setPendingReviews(mapped);
    } catch (error) {
      console.error("Failed to fetch reviews:", error);
      toast.error("Could not load reviews");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchReviews();
  }, []);

  const handleApproveReview = async (id) => {
    try {
      await reviewService.approveReview(id);
      setPendingReviews((prev) => prev.filter((r) => r.id !== id));
      toast.success("Review approved!");
    } catch (error) {
      console.error(error);
      toast.error("Failed to approve review");
    }
  };

  const handleDeleteReview = async (id) => {
    try {
      await reviewService.deleteReview(id);
      setPendingReviews((prev) => prev.filter((r) => r.id !== id));
      toast.success("Review deleted");
    } catch (error) {
      console.error(error);
      toast.error("Failed to delete review");
    }
  };

  return {
    reviews: pendingReviews,
    loading,
    handleApproveReview,
    handleRejectReview: handleDeleteReview, // Reuse delete for reject
    handleDeleteReview,
  };
}
