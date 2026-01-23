import { useState, useEffect } from "react";
import { useNavigate, useParams, useLocation } from "react-router-dom";
import catalogService from "../../../services/catalogService";
import toast from "react-hot-toast";

export function useAdminBouquetForm() {
  const navigate = useNavigate();
  const { id } = useParams();
  const location = useLocation();

  const isEditMode = Boolean(id);
  const isGiftMode = location.pathname.includes("gifts");

  // Metadata
  const [events, setEvents] = useState([]);
  const [recipients, setRecipients] = useState([]);
  const [flowers, setFlowers] = useState([]);
  const [sizes, setSizes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // ... (Lines 21-262 skipped)

  const [formData, setFormData] = useState({
    title: "",
    description: "",
    category: isGiftMode ? "Gifts" : "Bouquets",
    events: [],
    forWho: [],
    flowerTypes: [],
    img: null,
    imgFile: null,
    price: "",
    availableCount: "",
  });

  // { [sizeId]: { enabled: boolean, price: string, flowerQuantities: { [flowerId]: number }, images: Array<{id, url, file, isMain}> } }
  const [sizeStates, setSizeStates] = useState({});
  // imagesToDelete: { [sizeId]: [guid] }
  const [imagesToDelete, setImagesToDelete] = useState({});

  // Fetch metadata on mount
  useEffect(() => {
    const fetchMetadata = async () => {
      try {
        setLoading(true);
        const [eventsRes, recipientsRes, flowersRes, sizesRes] =
          await Promise.all([
            catalogService.getEvents(),
            catalogService.getRecipients(),
            catalogService.getFlowers(),
            catalogService.getSizes(),
          ]);

        setEvents(eventsRes);
        setRecipients(recipientsRes);
        setFlowers(flowersRes);
        setSizes(sizesRes.items || sizesRes);
      } catch (error) {
        console.error("Failed to fetch metadata:", error);
        toast.error("Failed to load form data");
      } finally {
        setLoading(false);
      }
    };

    fetchMetadata();
  }, []);

  // Load data in edit mode
  useEffect(() => {
    if (isEditMode && !loading) {
      const fetchData = async () => {
        try {
          if (isGiftMode) {
            // Fetch Gift
            const data = await catalogService.getGiftById(id);
            setFormData({
              title: data.name,
              description: "",
              category: "Gifts",
              events: [],
              forWho: [],
              flowerTypes: [],
              img: data.imageUrl,
              imgFile: null,
              price: data.price,
              availableCount: data.availableCount,
            });
          } else {
            // Fetch Bouquet
            const data = await catalogService.getBouquetById(id);
            const mappedSizeStates = {};
            if (data.sizes && data.sizes.length > 0) {
              data.sizes.forEach((s) => {
                const rawImages = s.images || s.bouquetImages || [];
                const mappedImages = rawImages.map((img) => ({
                  id: img.id,
                  url: img.imageUrl,
                  file: null,
                  isMain: img.isMain,
                }));

                const flowerQuantities = {};
                s.flowers?.forEach((f) => {
                  // Assuming backend returns { id, name, count } or similar
                  // If backend structure for size flowers is different, adjust here.
                  // Usually: BouquetSizeFlowers table -> { FlowerId, Count }
                  // If the current getBouquetById returns simple list, we might need to assume count is hidden or fetch differently.
                  // For now, mapping if 'count' exists, else 1.
                  flowerQuantities[f.id] = f.quantity || f.count || 1;
                });

                mappedSizeStates[s.sizeId] = {
                  enabled: true,
                  price: s.price,
                  flowerQuantities: flowerQuantities,
                  images: mappedImages,
                };
              });
            }

            const recipientIds = data.recipients.map((r) => r.id);

            // Collect all unique flower IDs from all sizes for the global checked list
            const allFlowerIds = new Set();
            data.sizes.forEach((s) =>
              s.flowers?.forEach((f) => allFlowerIds.add(f.id)),
            );

            setFormData({
              title: data.name,
              description: data.description || "",
              category: "Bouquets",
              events: data.events.map((e) => e.id),
              forWho: recipientIds,
              flowerTypes: Array.from(allFlowerIds),
              img: data.mainPhotoUrl,
              imgFile: null,
              price: "",
              availableCount: "",
            });
            setSizeStates(mappedSizeStates);
          }
        } catch (error) {
          console.error("Failed to fetch data:", error);
          toast.error("Failed to load data");
        }
      };

      fetchData();
    }
  }, [isEditMode, id, isGiftMode, loading]);

  // Handlers
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleCheckboxChange = (category, item) => {
    setFormData((prev) => {
      const list = prev[category];
      if (list.includes(item)) {
        return { ...prev, [category]: list.filter((i) => i !== item) };
      } else {
        return { ...prev, [category]: [...list, item] };
      }
    });
  };

  const handleImageUpload = (e) => {
    const file = e.target.files[0];
    if (file) {
      const previewUrl = URL.createObjectURL(file);
      setFormData((prev) => ({ ...prev, img: previewUrl, imgFile: file }));
    }
  };

  const handleSizeCheckbox = (sizeId) => {
    setSizeStates((prev) => {
      const currentState = prev[sizeId] || {};
      return {
        ...prev,
        [sizeId]: {
          ...currentState,
          enabled: !currentState.enabled,
          price: currentState.price || "",
          flowerQuantities: currentState.flowerQuantities || {},
          images: currentState.images || [],
        },
      };
    });
  };

  const handleSizePriceChange = (sizeId, newPrice) => {
    setSizeStates((prev) => ({
      ...prev,
      [sizeId]: { ...prev[sizeId], price: newPrice },
    }));
  };

  const handleSizeFlowerQuantityChange = (sizeId, flowerId, qty) => {
    setSizeStates((prev) => {
      const sizeState = prev[sizeId] || {};
      const currentQuantities = sizeState.flowerQuantities || {};
      return {
        ...prev,
        [sizeId]: {
          ...sizeState,
          flowerQuantities: {
            ...currentQuantities,
            [flowerId]: qty,
          },
        },
      };
    });
  };

  // MULTI-IMAGE HANDLERS
  const handleSizeImagesUpload = (e, sizeId) => {
    const files = Array.from(e.target.files);
    if (!files.length) return;

    const newImages = files.map((file) => ({
      id: null,
      url: URL.createObjectURL(file),
      file: file,
      isMain: false,
    }));

    setSizeStates((prev) => {
      const currentImages = prev[sizeId]?.images || [];
      return {
        ...prev,
        [sizeId]: {
          ...prev[sizeId],
          images: [...currentImages, ...newImages],
        },
      };
    });
  };

  const handleRemoveSizeImage = (sizeId, index) => {
    setSizeStates((prev) => {
      const currentImages = [...(prev[sizeId]?.images || [])];
      const imageToRemove = currentImages[index];

      if (!imageToRemove) return prev;

      if (imageToRemove.id) {
        setImagesToDelete((prevDel) => {
          const sizeDel = prevDel[sizeId] || [];
          return { ...prevDel, [sizeId]: [...sizeDel, imageToRemove.id] };
        });
      }

      currentImages.splice(index, 1);
      return {
        ...prev,
        [sizeId]: { ...prev[sizeId], images: currentImages },
      };
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (isSubmitting) return;
    setIsSubmitting(true);

    try {
      const data = new FormData();
      data.append("Name", formData.title);

      if (isGiftMode) {
        data.append("Price", formData.price);
        data.append("AvailableCount", formData.availableCount);
        if (formData.imgFile) data.append("Image", formData.imgFile);
        else if (!isEditMode) {
          toast.error("Please upload an image");
          return;
        }

        if (isEditMode) await catalogService.updateGift(id, data);
        else await catalogService.createGift(data);
        toast.success("Gift saved!");
      } else {
        // BOUQUET SUBMISSION
        data.append("Description", formData.description || "");
        formData.events.forEach((id) => data.append("EventIds", id));
        formData.forWho.forEach((id) => data.append("RecipientIds", id));

        // Use formData.flowerTypes only to determine which flowers are "active" for the bouquet context
        // But actual quantities come from size states.
        if (formData.flowerTypes.length === 0) {
          toast.error("Please select available flowers for this bouquet");
          return;
        }

        const enabledSizes = sizes.filter((s) => sizeStates[s.id]?.enabled);
        if (enabledSizes.length === 0) {
          toast.error("Please select at least one size");
          return;
        }

        let sizeIndex = 0;
        for (const size of enabledSizes) {
          const state = sizeStates[size.id];
          if (!state.price) {
            toast.error(`Please enter a price for size ${size.name}`);
            return;
          }

          data.append(`Sizes[${sizeIndex}].SizeId`, size.id);
          data.append(`Sizes[${sizeIndex}].Price`, state.price);

          // Flower Quantities
          let fIndex = 0;
          let hasAnyFlower = false;
          formData.flowerTypes.forEach((flowerId) => {
            const qty = state.flowerQuantities?.[flowerId] || 0;
            if (qty > 0) {
              data.append(`Sizes[${sizeIndex}].FlowerIds[${fIndex}]`, flowerId);
              data.append(
                `Sizes[${sizeIndex}].FlowerQuantities[${fIndex}]`,
                qty,
              );
              fIndex++;
              hasAnyFlower = true;
            }
          });

          if (!hasAnyFlower) {
            toast.error(
              `Please specify flower quantities for size ${size.name}`,
            );
            return;
          }

          // Images Logic - Robust Main/Additional separation
          const newImages = state.images.filter((img) => img.file);

          // Identify Main Image:
          // 1. If user explicitly marked one (mock implementation)
          // 2. OR default to the first uploaded new image if no main exists
          // For now, let's assume the first new image is Main if there are NO existing images,
          // or we just take the first one as Main and rest as Additional as per curl example.
          // Curl example: MainImage, AdditionalImages[0], ...

          // Implementation:
          // If we have new images, pop the first one as Main (if strictly creating new).
          // But wait, user might have existing images.

          // Let's iterate all valid images (new ones)
          if (newImages.length > 0) {
            const mainImg = newImages[0];
            const additional = newImages.slice(1);

            data.append(`Sizes[${sizeIndex}].MainImage`, mainImg.file);
            additional.forEach((img, idx) => {
              data.append(
                `Sizes[${sizeIndex}].AdditionalImages[${idx}]`,
                img.file,
              );
            });
          } else if (formData.imgFile && sizeIndex === 0 && !isEditMode) {
            // Fallback: use the global 'main photo' as the size's main photo if provided?
            // Logic in curl didn't exactly show this, but it's safe.
            // data.append(`Sizes[${sizeIndex}].MainImage`, formData.imgFile);
          }

          // Image Deletion Logic
          const idsDel = imagesToDelete[size.id] || [];
          idsDel.forEach((delId, dIdx) => {
            data.append(`Sizes[${sizeIndex}].ImageIdsToDelete[${dIdx}]`, delId);
          });

          sizeIndex++;
        }

        if (formData.imgFile) {
          data.append("MainPhoto", formData.imgFile);
        } else if (!isEditMode) {
          toast.error("Please upload a main image");
          return;
        }

        if (isEditMode) {
          await catalogService.updateBouquet(id, data);
          toast.success("Bouquet updated successfully!");
        } else {
          await catalogService.createBouquet(data);
          toast.success("Bouquet created successfully!");
        }
      }

      localStorage.setItem("adminActiveTab", isGiftMode ? "gifts" : "bouquets");
      navigate("/admin");
    } catch (error) {
      console.error("Failed to save item:", error);
      if (error.response && error.response.data) {
        const serverMsg =
          error.response.data.title ||
          JSON.stringify(error.response.data.errors) ||
          error.message;
        toast.error(`Error: ${serverMsg}`);
      } else {
        toast.error(error.response?.data?.message || "Failed to save item");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return {
    isEditMode,
    isGiftMode,
    loading,
    isSubmitting,
    events,
    recipients,
    flowers,
    sizes,
    formData,
    sizeStates,
    handleChange,
    handleCheckboxChange,
    handleImageUpload,
    handleSizeCheckbox,
    handleSizePriceChange,
    handleSizeFlowerQuantityChange,
    handleSizeImagesUpload,
    handleRemoveSizeImage,
    handleSubmit,
    navigate,
  };
}
