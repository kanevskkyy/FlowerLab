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
    const response = await axiosClient.post(
      "/api/users/auth/register",
      userData,
    );
    return response.data;
  },

  // Logout and token cleanup are now handled via AuthProvider and Backend cookies

  refreshToken: async (refreshToken) => {
    const response = await axiosClient.post("/api/users/auth/refresh", {
      refreshToken,
    });
    return response.data;
  },

  resendConfirmationEmail: async (email) => {
    const response = await axiosClient.post(
      "/api/users/auth/resend-confirm-email",
      {
        email,
      },
    );
    return response.data;
  },
};

export default authService;
