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
    const response = await axiosClient.put(
      `/api/catalog/bouquets/${id}`,
      formData,
      {
        headers: { "Content-Type": undefined },
      },
    );
    return response.data;
  },

  deleteBouquet: async (id) => {
    const response = await axiosClient.delete(`/api/catalog/bouquets/${id}`);
    return response.data;
  },

  // Gifts
  getGifts: async () => {
    const response = await axiosClient.get("/api/gifts");
    return response.data;
  },

  getGiftById: async (id) => {
    const response = await axiosClient.get(`/api/gifts/${id}`);
    return response.data;
  },

  createGift: async (formData) => {
    const response = await axiosClient.post("/api/gifts", formData, {
      headers: { "Content-Type": undefined },
    });
    return response.data;
  },

  updateGift: async (id, formData) => {
    const response = await axiosClient.put(`/api/gifts/${id}`, formData, {
      headers: { "Content-Type": undefined },
    });
    return response.data;
  },

  deleteGift: async (id) => {
    await axiosClient.delete(`/api/gifts/${id}`);
    return true; // or response.data
  },

  // Metadata - EVENTS
  getEvents: async () => {
    const response = await axiosClient.get("/api/catalog/events");
    return response.data;
  },

  createEvent: async (name) => {
    const response = await axiosClient.post("/api/catalog/events", { name });
    return response.data;
  },

  deleteEvent: async (id) => {
    await axiosClient.delete(`/api/catalog/events/${id}`);
    return true;
  },

  // Metadata - RECIPIENTS (For Who)
  getRecipients: async () => {
    const response = await axiosClient.get("/api/catalog/recipients");
    return response.data;
  },

  createRecipient: async (name) => {
    const response = await axiosClient.post("/api/catalog/recipients", {
      name,
    });
    return response.data;
  },

  deleteRecipient: async (id) => {
    await axiosClient.delete(`/api/catalog/recipients/${id}`);
    return true;
  },

  // Metadata - FLOWERS
  getFlowers: async () => {
    const response = await axiosClient.get("/api/catalog/flowers");
    return response.data;
  },

  createFlower: async (name) => {
    // Note: Backend might expect Quantity too, but for simple list management we send 0 or 1
    const response = await axiosClient.post("/api/catalog/flowers", {
      name,
      quantity: 0,
    });
    return response.data;
  },

  deleteFlower: async (id) => {
    await axiosClient.delete(`/api/catalog/flowers/${id}`);
    return true;
  },

  getSizes: async () => {
    // Note: The API might return { items: [] } or just [] depending on backend pagination
    const response = await axiosClient.get("/api/catalog/sizes");
    return response.data;
  },
};

export default catalogService;
