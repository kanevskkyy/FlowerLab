import axiosClient from "../api/axiosClient";

const userService = {
  getProfile: async () => {
    // Adjust endpoint based on User Service API
    const response = await axiosClient.get("/api/users/me");
    return response.data;
  },

  updateProfile: async (data, isFormData = false) => {
    const config = isFormData
      ? { headers: { "Content-Type": "multipart/form-data" } }
      : {};
    const response = await axiosClient.put("/api/users/me", data, config);
    return response.data;
  },

  changePassword: async (data) => {
    const response = await axiosClient.put("/api/users/me/password", data);
    return response.data;
  },

  deleteAccount: async () => {
    await axiosClient.delete("/api/users/me");
  },

  getAddresses: async () => {
    const response = await axiosClient.get("/api/users/me/addresses");
    return response.data;
  },

  addAddress: async (data) => {
    const response = await axiosClient.post("/api/users/me/addresses", data);
    return response.data;
  },

  deleteAddress: async (id) => {
    await axiosClient.delete(`/api/users/me/addresses/${id}`);
  },
};

export default userService;
