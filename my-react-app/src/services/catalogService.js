import axiosClient from "../api/axiosClient";

const catalogService = {
  // Bouquets
  getBouquets: async (params = {}) => {
    const response = await axiosClient.get("/api/catalog/bouquets", { params });
    return response.data;
  },

  getBouquetById: async (id) => {
    const response = await axiosClient.get(`/api/catalog/bouquets/${id}`);
    return response.data;
  },

  createBouquet: async (formData) => {
    // Content-Type header is handled by the browser/axios when submitting FormData,
    // explicitly setting it to undefined avoids boundary issues in some cases.
    const response = await axiosClient.post("/api/catalog/bouquets", formData, {
      headers: { "Content-Type": undefined },
    });
    return response.data;
  },

  updateBouquet: async (id, formData) => {
    const response = await axiosClient.put(`/api/catalog/bouquets/${id}`, formData, {
      headers: { "Content-Type": undefined },
    });
    return response.data;
  },

  deleteBouquet: async (id) => {
    const response = await axiosClient.delete(`/api/catalog/bouquets/${id}`);
    return response.data;
  },

  // Metadata
  getEvents: async () => {
    const response = await axiosClient.get("/api/catalog/events");
    return response.data;
  },

  getRecipients: async () => {
    const response = await axiosClient.get("/api/catalog/recipients");
    return response.data;
  },

  getFlowers: async () => {
    const response = await axiosClient.get("/api/catalog/flowers");
    return response.data;
  },

  getSizes: async () => {
    // Note: The API might return { items: [] } or just [] depending on backend pagination
    const response = await axiosClient.get("/api/catalog/sizes");
    return response.data;
  },
};

export default catalogService;
