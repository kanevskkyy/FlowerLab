import axiosClient from "../api/axiosClient";

const authService = {
  login: async (email, password) => {
    const response = await axiosClient.post("/api/users/auth/login", {
      email,
      password,
    });
    return response.data;
  },

  register: async (userData) => {
    const response = await axiosClient.post("/api/users/auth/register", userData);
    return response.data;
  },

  logout: () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("adminActiveTab");
  },

  refreshToken: async (refreshToken) => {
    const response = await axiosClient.post("/api/users/auth/refresh", {
      refreshToken,
    });
    return response.data;
  },
};

export default authService;
