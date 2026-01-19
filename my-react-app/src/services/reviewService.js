import axiosClient from "../api/axiosClient";

const reviewService = {
  getPendingReviews: async () => {
    const response = await axiosClient.get("/api/reviews", {
      params: { Status: "Pending" },
    });
    return response.data;
  },

  approveReview: async (id) => {
    const response = await axiosClient.patch(
      `/api/reviews/status/${id}/confirm`,
    );
    return response.data;
  },

  deleteReview: async (id) => {
    await axiosClient.delete(`/api/reviews/${id}`);
  },
};

export default reviewService;
