import axiosClient from "../api/axiosClient";

const orderService = {
  // Admin: Get all orders (paged)
  // Request URL: /api/orders?pageNumber=X&pageSize=Y...
  getAll: async (params) => {
    const response = await axiosClient.get("/api/orders", { params });
    return response.data;
  },

  // Get single order by ID
  getById: async (id) => {
    const response = await axiosClient.get(`/api/orders/${id}`);
    return response.data; // Returns full order details
  },

  // Get all order statuses (for dropdowns)
  getStatuses: async () => {
    const response = await axiosClient.get("/api/order-statuses");
    return response.data;
  },

  // Admin: Update status
  updateStatus: async (id, statusId) => {
    // Backend expects { StatusId: guid }
    const response = await axiosClient.put(`/api/orders/${id}/status`, {
      statusId: statusId,
    });
    return response.data;
  },

  checkDiscountEligibility: async () => {
    const response = await axiosClient.get(
      `/api/orders/discount-eligibility?_t=${Date.now()}`,
    );
    return response.data;
  },
};

export default orderService;
