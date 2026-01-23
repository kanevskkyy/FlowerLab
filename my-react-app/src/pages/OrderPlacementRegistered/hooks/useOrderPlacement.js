import { useState, useEffect, useCallback } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";

import { useCart } from "../../../context/CartContext";
import { useAuth } from "../../../context/useAuth";
import { useGifts } from "../../Gifts/hooks/useGifts";
import catalogService from "../../../services/catalogService";
import orderService from "../../../services/orderService";
import axiosClient from "../../../api/axiosClient";

// === Schema ===
const schema = z
  .object({
    firstName: z.string().min(1, "First Name is required"),
    lastName: z.string().min(1, "Last Name is required"),
    phone: z.string().regex(/^\+?[0-9]{10,12}$/, "Invalid phone format"),
    receiverType: z.enum(["self", "other"]),

    // Conditional Receiver Fields
    receiverName: z.string().optional(),
    receiverPhone: z.string().optional(),

    deliveryType: z.enum(["pickup", "delivery"]),
    message: z.string().optional(),
    cardMessage: z.string().optional(),
    noCall: z.boolean().optional(),
    isCardAdded: z.boolean().optional(),
    selectedAddressId: z.union([z.string(), z.number()]).optional(),
    selectedShopId: z.union([z.string(), z.number()]).optional(),
  })
  .superRefine((data, ctx) => {
    // Delivery Validation
    if (data.deliveryType === "delivery" && !data.selectedAddressId) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Please select a delivery address",
        path: ["deliveryType"],
      });
    }
    if (data.deliveryType === "pickup" && !data.selectedShopId) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Please select a shop",
        path: ["deliveryType"],
      });
    }

    // Card Validation
    if (
      data.isCardAdded &&
      (!data.cardMessage || data.cardMessage.trim().length === 0)
    ) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Please enter a message for the card",
        path: ["cardMessage"],
      });
    }

    // Receiver Validation
    if (data.receiverType === "other") {
      if (!data.receiverName || data.receiverName.length < 1) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Receiver Name is required",
          path: ["receiverName"],
        });
      }
      if (
        !data.receiverPhone ||
        !/^\+?[0-9]{10,12}$/.test(data.receiverPhone)
      ) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Valid Receiver Phone is required",
          path: ["receiverPhone"],
        });
      }
    }
  });

export const useOrderPlacement = () => {
  const navigate = useNavigate();
  const { cartItems, removeItem, clearCart } = useCart();
  const { user } = useAuth();
  const { gifts, loading: giftsLoading } = useGifts();

  // Local UI state
  const [isAddingAddress, setIsAddingAddress] = useState(false);
  const [newAddress, setNewAddress] = useState("");
  const [selectedGifts, setSelectedGifts] = useState(() => {
    try {
      const saved = localStorage.getItem("order_selectedGifts");
      return saved ? JSON.parse(saved) : [];
    } catch (error) {
      console.error("Failed to parse selected gifts", error);
      return [];
    }
  });
  const [isCardAdded, setIsCardAdded] = useState(() => {
    try {
      const saved = localStorage.getItem("order_isCardAdded");
      return saved ? JSON.parse(saved) : false;
    } catch (error) {
      console.error("Failed to parse card state", error);
      return false;
    }
  });

  const [isEligibleForDiscount, setIsEligibleForDiscount] = useState(false);

  // Persist Gifts and Card State
  useEffect(() => {
    localStorage.setItem("order_selectedGifts", JSON.stringify(selectedGifts));
  }, [selectedGifts]);

  useEffect(() => {
    localStorage.setItem("order_isCardAdded", JSON.stringify(isCardAdded));
  }, [isCardAdded]);

  const [deliveryAddresses, setDeliveryAddresses] = useState([]);

  // Mock Shop Addresses
  const shopAddresses = [
    { id: 1, text: "м. Чернівці, вул Герцена 2а" },
    { id: 2, text: "вул Васіле Александрі, 1" },
  ];

  // === Form Setup ===
  const methods = useForm({
    resolver: zodResolver(schema),
    defaultValues: {
      firstName: "",
      lastName: "",
      phone: "",
      receiverType: "self",
      receiverName: "",
      receiverPhone: "",
      deliveryType: "delivery",
      selectedAddressId: 0,
      selectedShopId: 1,
      message: "",
      cardMessage: "",
      noCall: false,
      isCardAdded: false,
    },
  });

  const { handleSubmit, setValue, watch } = methods;

  const deliveryType = watch("deliveryType");

  // === Effects ===

  // Sync Card State to Form
  useEffect(() => {
    setValue("isCardAdded", isCardAdded);
  }, [isCardAdded, setValue]);

  // Auto-fill user data
  useEffect(() => {
    if (user) {
      setValue("firstName", user.name || "");
      setValue("lastName", user.lastName || "");
      setValue("phone", user.phone || "");
    }
  }, [user, setValue]);

  // Fetch User Addresses
  useEffect(() => {
    // Cleanup legacy flag
    localStorage.removeItem("consumed_first_order_discount");

    const fetchUserAddresses = async () => {
      if (user) {
        try {
          const addrs = await catalogService.getUserAddresses();
          const mapped = addrs.map((a) => ({
            id: a.id,
            // Backend returns a single 'address' string, not structured data
            text: a.address || "Unknown Address",
          }));
          setDeliveryAddresses(mapped);
          if (mapped.length > 0) {
            setValue("selectedAddressId", mapped[0].id);
          }
        } catch (error) {
          console.error("Failed to fetch addresses", error);
        }
      }
    };
    fetchUserAddresses();
  }, [user, setValue]);

  const checkEligibility = useCallback(async () => {
    if (user) {
      try {
        const eligible = await orderService.checkDiscountEligibility();
        setIsEligibleForDiscount(eligible);
      } catch (error) {
        console.error("Failed to check discount eligibility", error);
        toast.error("Failed to check discount");
      }
    }
  }, [user]);

  // Initial check
  useEffect(() => {
    checkEligibility();
  }, [checkEligibility]);

  // Re-check on focus/pageshow (back button)
  useEffect(() => {
    const onResume = (event) => {
      // 1. Detect BFCache restore
      if (event && event.persisted) {
        window.location.reload();
        return;
      }

      // 2. Detect History Back Navigation (standard)
      const navEntry = performance.getEntriesByType("navigation")[0];
      if (navEntry && navEntry.type === "back_forward") {
        // We need to ensure we don't reload infinitely if the reload itself counts as back_forward
        const hasReloaded = sessionStorage.getItem("order_placement_reloaded");
        if (!hasReloaded) {
          sessionStorage.setItem("order_placement_reloaded", "true");
          window.location.reload();
          return;
        }
      } else {
        // Clear the flag if it's a normal navigation so next back works
        sessionStorage.removeItem("order_placement_reloaded");
      }

      // Optimistically reset to false to avoid showing stale 10%
      // setIsEligibleForDiscount(false);
      checkEligibility();
    };

    window.addEventListener("focus", onResume);
    window.addEventListener("pageshow", onResume);
    document.addEventListener("visibilitychange", onResume);

    // Initial check for back_forward on mount
    const navEntry = performance.getEntriesByType("navigation")[0];
    if (navEntry && navEntry.type === "back_forward") {
      const hasReloaded = sessionStorage.getItem("order_placement_reloaded");
      if (!hasReloaded) {
        sessionStorage.setItem("order_placement_reloaded", "true");
        window.location.reload();
      } else {
        // If we already reloaded, just check eligibility
        sessionStorage.removeItem("order_placement_reloaded");
      }
    }

    return () => {
      window.removeEventListener("focus", onResume);
      window.removeEventListener("pageshow", onResume);
      document.removeEventListener("visibilitychange", onResume);
    };
  }, [checkEligibility]);

  // === Calculations ===
  const subtotal = cartItems.reduce((sum, item) => {
    const price =
      typeof item.price === "string"
        ? parseFloat(item.price.replace(/[^\d.]/g, ""))
        : item.price;
    const qty = item.qty || 1;
    return sum + price * qty;
  }, 0);

  //const deliveryCost = deliveryType === "delivery" ? 100 : 0;
  const discount = isEligibleForDiscount ? subtotal * 0.1 : 0;

  const giftsTotal = selectedGifts.reduce((sum, id) => {
    const gift = gifts.find((g) => g.id === id);
    return sum + (gift?.price || 0);
  }, 0);
  const cardFee = isCardAdded ? 50 : 0;
  const total = subtotal - discount + giftsTotal + cardFee;

  // === Handlers ===
  const toggleGift = (giftId) => {
    setSelectedGifts((prev) =>
      prev.includes(giftId)
        ? prev.filter((id) => id !== giftId)
        : [...prev, giftId],
    );
  };

  const toggleCard = () => {
    setIsCardAdded((prev) => !prev);
  };

  const handleAddAddress = async () => {
    if (!newAddress.trim()) return;

    if (user) {
      // Authenticated: Persist to Backend
      try {
        // 1. Persist to backend
        const response = await axiosClient.post("/api/users/me/addresses", {
          address: newAddress,
          isDefault: false,
        });

        // 2. Fetch updated list or manually add
        const addrs = await catalogService.getUserAddresses();
        const mapped = addrs.map((a) => ({
          id: a.id,
          text: a.address || "Unknown Address",
        }));
        setDeliveryAddresses(mapped);

        // 3. Select the new address
        const created = mapped.find((a) => a.text === newAddress);
        if (created) {
          setValue("selectedAddressId", created.id, { shouldValidate: true });
        } else if (mapped.length > 0) {
          setValue("selectedAddressId", mapped[mapped.length - 1].id, {
            shouldValidate: true,
          });
        }

        setNewAddress("");
        setIsAddingAddress(false);
        toast.success("Address saved successfully!");
      } catch (error) {
        console.error("Failed to save address", error);
        toast.error("Failed to save address (Login required?)");
      }
    } else {
      // Guest: Add locally to state
      const tempId = `temp-${Date.now()}`;
      const newAddrObj = {
        id: tempId,
        text: newAddress,
      };

      setDeliveryAddresses((prev) => [...prev, newAddrObj]);
      setValue("selectedAddressId", tempId, { shouldValidate: true });

      setNewAddress("");
      setIsAddingAddress(false);
      toast.success("Address added for this order");
    }
  };

  const onSubmit = async (data) => {
    // Helper to map Shop ID to Backend Enum
    const getShopEnum = (id) => {
      // Frontend IDs: 1 (Hertsena), 2 (Vasile)
      // Backend Enum: Hertsena2A, VasileAlexandri1
      if (Number(id) === 1) return "Hertsena2A";
      if (Number(id) === 2) return "VasileAlexandri1";
      return null;
    };

    const deliveryAddressText = deliveryAddresses.find(
      (a) => a.id === data.selectedAddressId,
    )?.text;

    const finalOrderData = {
      firstName: data.firstName,
      lastName: data.lastName,
      phoneNumber: data.phone,
      notes: data.message,
      receiverName: data.receiverType === "other" ? data.receiverName : null,
      receiverPhone: data.receiverType === "other" ? data.receiverPhone : null,

      isDelivery: deliveryType === "delivery",

      // Items Mapping
      items: cartItems
        .filter((item) => item.bouquetId && item.sizeId) // Only real bouquets
        .map((item) => ({
          bouquetId: item.bouquetId,
          sizeId: item.sizeId,
          count: item.qty || 1,
        })),

      // Gifts Mapping
      // Combine:
      // 1. Gifts selected via checkboxes in this form (selectedGifts)
      // 2. Gifts added to the cart explicitly (cartItems where isGift is true or mapped)
      gifts: [
        ...selectedGifts.map((giftId) => ({
          giftId: giftId,
          count: 1,
        })),
        ...cartItems
          .filter((item) => item.isGift || (!item.bouquetId && item.id)) // Heuristic for gifts: has flag OR has ID but no bouquetId
          // Note: Cart items might use 'id' as 'giftId' if it's a direct gift add.
          // We need to ensure we send the correct ID.
          .map((item) => ({
            giftId: item.productId || item.id, // Depending on how it was added to cart
            count: item.qty || 1,
          })),
      ],

      // Delivery Info
      deliveryInformation:
        deliveryType === "delivery"
          ? {
              address: deliveryAddressText || "Unknown Address",
            }
          : null,

      // Pickup Store Enum
      pickupStoreAddress:
        deliveryType === "pickup" ? getShopEnum(data.selectedShopId) : null,

      giftMessage: isCardAdded ? data.cardMessage : null,

      // Pass total for display on Checkout page
      total: total,
    };

    // Create Order immediately
    try {
      const response = await catalogService.createOrder(finalOrderData);

      // Clear cart immediately as order is created/reserved
      clearCart();

      toast.success("Order confirmed!");

      // Pass created order data including ID and createdAt to Checkout
      navigate("/checkout", {
        state: {
          orderData: {
            // Merge form data with response data
            ...finalOrderData,
            id: response.id,
            guestToken: response.guestToken,
            createdAt: response.createdAt,
          },
        },
      });
    } catch (error) {
      console.error("Failed to create order", error);

      const errorMessage =
        error.response?.data?.detail ||
        error.response?.data?.title ||
        (typeof error.response?.data === "string"
          ? error.response?.data
          : "Failed to create order");

      toast.error(errorMessage);
    }
  };

  return {
    methods, // Form methods to pass to FormProvider
    onSubmit: handleSubmit(onSubmit, (errors) => {
      console.error("Form Validation Errors:", errors);
      toast.error("Please fill in all required fields correctly.");
    }),

    // Data
    gifts,
    giftsLoading,
    cartItems,
    deliveryAddresses,
    shopAddresses,

    // UI State
    isAddingAddress,
    setIsAddingAddress,
    newAddress,
    setNewAddress,
    selectedGifts,
    isCardAdded,

    // Calculated Values
    subtotal,
    discount,
    total,

    // Handlers
    toggleGift,
    toggleCard,
    handleAddAddress,
    removeItem, // Exposed from useCart
    navigate, // Exposed mainly for Back button
  };
};
