import { useState, useEffect } from "react";
import axiosClient from "../../../api/axiosClient";
import reviewService from "../../../services/reviewService";
import toast from "react-hot-toast";

export const useProductData = (id) => {
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [recommendations, setRecommendations] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [selectedSize, setSelectedSize] = useState("");
  const [selectedImageIndex, setSelectedImageIndex] = useState(0);

  // Reset image index when size changes
  useEffect(() => {
    setSelectedImageIndex(0);
  }, [selectedSize]);

  useEffect(() => {
    if (!id) return;

    window.scrollTo(0, 0);

    const fetchProductData = async () => {
      setLoading(true);
      try {
        const response = await axiosClient.get(`/api/catalog/bouquets/${id}`, {
          headers: { Accept: "application/json" },
        });
        const data = response.data;

        // Map backend DTO to frontend structure
        const mappedProduct = {
          id: data.id,
          title: data.name,
          description: data.description,
          composition:
            data.sizes[0]?.flowers
              .map((f) => `${f.name} (${f.quantity})`)
              .join(", ") || "Diverse floral mix",
          // Store Array of images for each size
          images: data.sizes.reduce((acc, size) => {
            const sizeImgs =
              size.images && size.images.length > 0
                ? size.images.map((i) => i.imageUrl)
                : [data.mainPhotoUrl];

            acc[size.sizeName] = sizeImgs;
            return acc;
          }, {}),
          prices: data.sizes.reduce((acc, size) => {
            acc[size.sizeName] = size.price;
            return acc;
          }, {}),
          sizeIds: data.sizes.reduce((acc, size) => {
            acc[size.sizeName] = size.sizeId;
            return acc;
          }, {}),
          availableSizes: data.sizes.map((s) => s.sizeName),
        };

        setProduct(mappedProduct);
        // Default to M if available, else first size
        const defaultSize = mappedProduct.availableSizes.includes("M")
          ? "M"
          : mappedProduct.availableSizes[0];
        setSelectedSize(defaultSize);
      } catch (error) {
        console.error("Failed to fetch product details:", error);
        toast.error("Product not found");
      } finally {
        setLoading(false);
      }
    };

    const fetchRecommendations = async () => {
      try {
        const response = await axiosClient.get("/api/catalog/bouquets", {
          params: { PageSize: 4 },
          headers: { Accept: "application/json" },
        });
        const items = response.data.items || response.data;
        setRecommendations(
          items
            .filter((p) => p.id !== id)
            .slice(0, 3)
            .map((p) => ({
              id: p.id,
              image: p.mainPhotoUrl,
              title: p.name,
              price: p.price,
            })),
        );
      } catch (error) {
        console.error("Failed to fetch recommendations:", error);
      }
    };

    // fetchReviews moved to refetchReviews

    fetchProductData();
    fetchRecommendations();
    // Review fetch is moved to be accessible
    refetchReviews();
  }, [id]);

  const refetchReviews = async () => {
    try {
      const data = await reviewService.getApprovedReviews(id);
      const mappedReviews = (data.items || data).map((r) => ({
        id: r.id,
        name: r.user
          ? `${r.user.firstName} ${r.user.lastName || ""}`.trim()
          : "Customer",
        rating: r.rating || 5, // Default to 5 if missing, but should be there
        text: r.comment,
        avatar: r.user?.photoUrl || null,
      }));
      setReviews(mappedReviews);
    } catch (error) {
      console.error("Failed to fetch reviews:", error);
    }
  };

  return {
    product,
    loading,
    recommendations,
    reviews,
    selectedSize,
    setSelectedSize,
    selectedImageIndex,
    setSelectedImageIndex,
    refetchReviews,
  };
};
