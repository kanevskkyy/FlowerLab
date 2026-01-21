import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useConfirm } from "../../../context/ModalProvider";
import catalogService from "../../../services/catalogService";
import { useCart } from "../../../context/CartContext";
import toast from "react-hot-toast";

export function useCheckOut({ orderData }) {
  const navigate = useNavigate();
  const confirm = useConfirm();
  const { clearCart } = useCart();

  const [loading, setLoading] = useState(false);

  // Derived data
  const orderNumber = orderData?.id || "---"; // Will be updated after creation if null
  const total = orderData?.total || 0;

  // Payment Submission
  const handleCompletePayment = async () => {
    setLoading(true);
    try {
      // 1. Create Order
      // 1. Create Order
      // Strip frontend-only fields (total, paymentMethod, paymentDetails etc.)
      const {
        total,
        paymentMethod,
        paymentDetails,
        cartItems,
        selectedGifts,
        isCardAdded,
        deliveryAddressText,
        shopAddressText,
        ...validOrderDto
      } = orderData;

      // Validate Items have SizeId (Critical for backend)
      // Filter out invalid items (missing SizeId)
      const validItems = [];
      const invalidItems = [];
      validOrderDto.items?.forEach((item) => {
        if (item.sizeId) {
          validItems.push(item);
        } else {
          invalidItems.push(item);
        }
      });

      if (invalidItems.length > 0) {
        console.warn("Filtered out items without SizeId:", invalidItems);
        toast("Some items were removed due to missing size information.", {
          icon: "⚠️",
        });
      }

      if (validItems.length === 0) {
        toast.error("No valid items to checkout.");
        setLoading(false);
        return;
      }

      const payload = {
        ...validOrderDto,
        items: validItems,
      };

      const newOrder = await catalogService.createOrder(payload);

      // 2. Get Payment URL
      const { paymentUrl } = await catalogService.payOrder(newOrder.id);

      // 3. Redirect to LiqPay
      if (paymentUrl) {
        window.location.href = paymentUrl;
      } else {
        toast.error("Failed to generate payment link.");
      }
    } catch (error) {
      console.error("Order creation failed", error);
      console.error("Error response:", error.response?.data);

      const errorMessage =
        typeof error.response?.data === "string"
          ? error.response.data
          : error.response?.data?.detail ||
            error.response?.data?.title ||
            "Failed to create order. Please try again.";

      toast.error(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleCancelPayment = () => {
    confirm({
      title: "Cancel payment?",
      message: "Are you sure you want to cancel the payment?",
      confirmText: "Yes, cancel",
      confirmType: "danger",
      onConfirm: () => {
        navigate("/order-registered");
      },
    });
  };

  return {
    orderNumber,
    total,
    handleCompletePayment,
    handleCancelPayment,
    loading,
  };
}
