import axiosClient from "../api/axiosClient";

const catalogService = {
  // Bouquets - NOW USING AGGREGATOR
  getBouquets: async (params = {}) => {
    const response = await axiosClient.get("/api/bouquet", { params });
    return response.data;
  },

  getBouquetById: async (id) => {
    // This was already using /api/catalog/bouquets/${id} in previous code?
    // Wait, useProductData used /api/catalog/bouquets/${id}.
    // But Aggregator also has /api/bouquet/${id} (GetBouquetWithReviews).
    // Should we switch this too? The plan said "connect filtering".
    // But if we want consistency, we can switch, but GetBouquetWithReviews returns { bouquet, reviews }.
    // The existing frontend expects "data" to be the bouquet only?
    // Let's stick to getBouquets update for now as per plan for Filtering.
    // The existing getBouquetById in this file calls /api/catalog/bouquets/${id}.
    // I will NOT change getBouquetById unless requested, to avoid breaking product page if structure differs.
    const response = await axiosClient.get(`/api/catalog/bouquets/${id}`);
    return response.data;
  },

  // ... (create/update/delete stay on CatalogService likely, as Aggregator is read-only usually)

  // ...

  // Metadata - AGGREGATOR
  getFilters: async () => {
    const response = await axiosClient.get("/api/filters");
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
  getGifts: async (params = {}) => {
    const response = await axiosClient.get("/api/gifts", { params });
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

  createFlower: async (name, quantity = 0) => {
    const response = await axiosClient.post("/api/catalog/flowers", {
      name,
      quantity,
    });
    return response.data;
  },

  updateFlower: async (id, name, quantity) => {
    const response = await axiosClient.put(`/api/catalog/flowers/${id}`, {
      name,
      quantity,
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
  // Orders
  createOrder: async (orderData) => {
    const response = await axiosClient.post("/api/orders", orderData);
    return response.data;
  },

  // Addresses (for checkout dropdown)
  getUserAddresses: async () => {
    const response = await axiosClient.get("/api/users/me/addresses");
    return response.data;
  },

  // Payment
  payOrder: async (orderId, guestToken = null) => {
    const config = {
      params: {},
      headers: {},
    };
    const payload = {};

    if (guestToken) {
      // Body variants
      payload.guestToken = guestToken;
      payload.GuestToken = guestToken;
      payload.token = guestToken;

      // Query variants
      config.params.guestToken = guestToken;
      config.params.token = guestToken;

      // Header variants
      config.headers["Guest-Token"] = guestToken;
      config.headers["X-Guest-Token"] = guestToken;
      config.headers["Authorization"] = `Guest ${guestToken}`;
    }

    const response = await axiosClient.post(
      `/api/orders/${orderId}/pay`,
      payload,
      config,
    );
    return response.data;
  },
};

export default catalogService;
