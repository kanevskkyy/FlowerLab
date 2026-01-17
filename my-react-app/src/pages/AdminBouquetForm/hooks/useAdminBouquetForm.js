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
  });

  // { [sizeId]: { enabled: boolean, price: string, img: string|null, imgFile: File|null } }
  const [sizeStates, setSizeStates] = useState({});

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

  // Load bouquet data in edit mode
  useEffect(() => {
    if (isEditMode && !loading) {
      const fetchBouquet = async () => {
        try {
          const data = await catalogService.getBouquetById(id);

          const mappedSizeStates = {};
          if (data.sizes && data.sizes.length > 0) {
            data.sizes.forEach((s) => {
              const mainImgFn =
                s.bouquetImages?.find((img) => img.isMain)?.imageUrl || null;

              mappedSizeStates[s.sizeId] = {
                enabled: true,
                price: s.price,
                img: mainImgFn,
                imgFile: null,
              };
            });
          }

          setFormData({
            title: data.name,
            description: data.description || "",
            category: isGiftMode ? "Gifts" : "Bouquets",
            events: data.events.map((e) => e.id),
            forWho: data.recipients.map((r) => r.id),
            flowerTypes: data.sizes[0]?.flowers.map((f) => f.id) || [],
            img: data.mainPhotoUrl,
            imgFile: null,
          });
          setSizeStates(mappedSizeStates);
        } catch (error) {
          console.error("Failed to fetch bouquet:", error);
          toast.error("Failed to load bouquet data");
        }
      };

      fetchBouquet();
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

  const handleSizeImageUpload = (e, sizeId) => {
    const file = e.target.files[0];
    if (file) {
      const previewUrl = URL.createObjectURL(file);
      setSizeStates((prev) => ({
        ...prev,
        [sizeId]: { ...prev[sizeId], img: previewUrl, imgFile: file },
      }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const data = new FormData();
      data.append("Name", formData.title);
      data.append("Description", formData.description || "");

      // Add event IDs
      formData.events.forEach((eventId) => {
        data.append("EventIds", eventId);
      });

      // Validate flowers
      if (formData.flowerTypes.length === 0 && !isGiftMode) {
        toast.error("Please select at least one flower type");
        return;
      }

      // Collect enabled sizes
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

        // Flowers (Simple implementation: apply same flowers to all sizes)
        formData.flowerTypes.forEach((flowerId, fDescIdx) => {
          data.append(`Sizes[${sizeIndex}].FlowerIds[${fDescIdx}]`, flowerId);
          data.append(`Sizes[${sizeIndex}].FlowerQuantities[${fDescIdx}]`, 1);
        });

        // Size Image
        if (state.imgFile) {
          data.append(`Sizes[${sizeIndex}].MainImage`, state.imgFile);
        } else if (state.img) {
          data.append(`Sizes[${sizeIndex}].MainImageUrl`, state.img);
        } else if (formData.imgFile) {
           // Fallback to main bouquet image
           data.append(`Sizes[${sizeIndex}].MainImage`, formData.imgFile);
        }

        sizeIndex++;
      }

      // Main photo
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

      localStorage.setItem("adminActiveTab", isGiftMode ? "gifts" : "bouquets");
      navigate("/admin");
    } catch (error) {
      console.error("Failed to save bouquet:", error);
      if (error.response && error.response.data) {
        const serverMsg =
          error.response.data.title ||
          JSON.stringify(error.response.data.errors) ||
          error.message;
        toast.error(`Error: ${serverMsg}`);
      } else {
        toast.error(error.response?.data?.message || "Failed to save bouquet");
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
    handleSizeImageUpload,
    handleSubmit,
    navigate // exposed for cancel button
  };
}
