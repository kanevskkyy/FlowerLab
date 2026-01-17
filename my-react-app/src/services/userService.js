import axiosClient from "../api/axiosClient";

const userService = {
  getProfile: async () => {
    // Adjust endpoint based on User Service API
    const response = await axiosClient.get("/api/users/profile");
    return response.data;
  },

  updateProfile: async (data) => {
    const response = await axiosClient.put("/api/users/profile", data);
    return response.data;
  },
};

export default userService;
