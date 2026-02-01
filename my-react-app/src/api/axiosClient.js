import axios from "axios";
import i18n from "../i18n";

const baseURL = import.meta.env.VITE_API_URL;

const axiosClient = axios.create({
  baseURL: baseURL,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
  paramsSerializer: (params) => {
    const parts = [];
    for (const [key, value] of Object.entries(params)) {
      if (value === null || value === undefined) continue;
      if (Array.isArray(value)) {
        value.forEach((v) =>
          parts.push(`${encodeURIComponent(key)}=${encodeURIComponent(v)}`),
        );
      } else {
        parts.push(`${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
      }
    }
    return parts.join("&");
  },
});

// Інтерсептор для додавання Access Token та локалізації до кожного запиту
axiosClient.interceptors.request.use((config) => {
  // Налаштування мови
  const langNormalized = i18n.language ? i18n.language.toLowerCase() : "";
  const currentLang =
    langNormalized.startsWith("uk") || langNormalized === "ua" ? "ua" : "en";
  config.headers["Accept-Language"] = currentLang;

  // Винятково для адмінських редагувань
  if (
    config.skipLocalization ||
    window.location.pathname.startsWith("/admin")
  ) {
    config.headers["X-Skip-Localization"] = "true";
  }

  return config;
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

// Інтерсептор для обробки помилок
axiosClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Якщо помилка 401 (Unauthorized)
    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      !originalRequest.url.includes("/login")
    ) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({
            resolve: () => resolve(axiosClient(originalRequest)),
            reject: (err) => reject(err),
          });
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        // Кличемо refresh без тіла, бо токен у куках
        await axios.post(
          `${baseURL}/api/auth/refresh`,
          {},
          { withCredentials: true },
        );

        processQueue(null);
        return axiosClient(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError, null);

        // Очищаємо залишки
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");

        if (
          !originalRequest.skipAuthRedirect &&
          !window.location.pathname.includes("/login")
        ) {
          window.location.href = "/login";
        }
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }
    return Promise.reject(error);
  },
);

export default axiosClient;
