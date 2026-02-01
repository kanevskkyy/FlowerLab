import axiosClient from "../api/axiosClient";

const adminService = {
  // Users
  getUsers: async (params) => {
    const response = await axiosClient.get("/api/users/admin/users", {
      params,
    });
    return response.data;
  },

  updateUserDiscount: async (userId, discountPercentage) => {
    const response = await axiosClient.put(
      `/api/users/admin/users/${userId}/discount`,
      {
        personalDiscountPercentage: discountPercentage,
      },
    );
    return response.data;
  },
};

export default adminService;
