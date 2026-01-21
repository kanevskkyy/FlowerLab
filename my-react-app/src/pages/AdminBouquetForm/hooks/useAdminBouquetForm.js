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

  // { [sizeId]: { enabled: boolean, price: string, images: Array<{id, url, file, isMain}> } }
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

                mappedSizeStates[s.sizeId] = {
                  enabled: true,
                  price: s.price,
                  images: mappedImages,
                };
              });
            }

            const recipientIds = data.recipients.map((r) => r.id);

            setFormData({
              title: data.name,
              description: data.description || "",
              category: "Bouquets",
              events: data.events.map((e) => e.id),
              forWho: recipientIds,
              flowerTypes: data.sizes[0]?.flowers.map((f) => f.id) || [],
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

        if (formData.flowerTypes.length === 0) {
          toast.error("Please select at least one flower type");
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

          formData.flowerTypes.forEach((flowerId, fDescIdx) => {
            data.append(`Sizes[${sizeIndex}].FlowerIds[${fDescIdx}]`, flowerId);
            data.append(`Sizes[${sizeIndex}].FlowerQuantities[${fDescIdx}]`, 1);
          });

          // Images Logic - FIXED
          const newImages = state.images.filter((img) => img.file);

          // Image Deletion Logic
          const idsDel = imagesToDelete[size.id] || [];
          idsDel.forEach((delId, dIdx) => {
            data.append(`Sizes[${sizeIndex}].ImageIdsToDelete[${dIdx}]`, delId);
          });

          if (!isEditMode) {
            // CREATE MODE
            const [first, ...rest] = newImages;
            if (first) {
              data.append(`Sizes[${sizeIndex}].MainImage`, first.file);
            }
            rest.forEach((img, idx) => {
              data.append(
                `Sizes[${sizeIndex}].AdditionalImages[${idx}]`,
                img.file,
              );
            });

            if (newImages.length === 0 && formData.imgFile) {
              data.append(`Sizes[${sizeIndex}].MainImage`, formData.imgFile);
            }
          } else {
            // EDIT MODE - APPEND ONLY
            // We just take all new files and send them as NewImages
            const newImages = state.images.filter((img) => img.file);
            newImages.forEach((img, idx) => {
              data.append(
                `Sizes[${sizeIndex}].AdditionalImages[${idx}]`,
                img.file,
              );
            });
          }

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
    }
  };

  return {
    isEditMode,
    isGiftMode,
    loading,
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
    handleSizeImagesUpload,
    handleRemoveSizeImage,
    handleSubmit,
    navigate,
  };
}
