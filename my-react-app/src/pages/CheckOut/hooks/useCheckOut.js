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
      let orderId = orderData.id;
      let guestToken = orderData.guestToken;

      if (!orderId) {
        toast.error("Order ID is missing. Please restart checkout.");
        setLoading(false);
        return;
      }

      // 2. Get Payment URL
      const { paymentUrl } = await catalogService.payOrder(orderId, guestToken);

      // 4. Redirect to LiqPay
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
