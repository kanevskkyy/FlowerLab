import axios from "axios";

const gatewayUrl = import.meta.env.VITE_API_URL;
const baseURL = `${gatewayUrl}/api`;

const axiosClient = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
  // Важливо для CORS та Cookies (якщо знадобляться)
  withCredentials: true,
});

// Автоматичне додавання токена до запитів
axiosClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Обробка відповідей та помилок
axiosClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // Якщо токен прострочився (401), розлогінюємо користувача
    if (error.response && error.response.status === 401) {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      // Можна розкоментувати, якщо хочете авто-редірект:
      // window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export default axiosClient;
