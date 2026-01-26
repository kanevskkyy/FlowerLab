import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useConfirm } from "../../../context/ModalProvider";
import catalogService from "../../../services/catalogService";
import orderService from "../../../services/orderService";
import { useCart } from "../../../context/CartContext";
import toast from "react-hot-toast";
import { extractErrorMessage } from "../../../utils/errorUtils";

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
        toast.error("Номер замовлення відсутній. Перезапустіть оформлення.");
        setLoading(false);
        return;
      }

      // 2. Get Payment URL
      const { paymentUrl } = await catalogService.payOrder(orderId, guestToken);

      // 4. Redirect to LiqPay
      if (paymentUrl) {
        window.location.href = paymentUrl;
      } else {
        toast.error("Не вдалося сформувати посилання на оплату.");
      }
    } catch (error) {
      console.error("Order creation failed", error);
      toast.error(
        extractErrorMessage(
          error,
          "Не вдалося створити замовлення. Спробуйте ще раз.",
        ),
      );
    } finally {
      setLoading(false);
    }
  };

  const handleCancelPayment = () => {
    confirm({
      title: "Cancel payment?",
      message:
        "Are you sure you want to cancel the payment? The order will be deleted.",
      confirmText: "Yes, cancel",
      confirmType: "danger",
      onConfirm: async () => {
        try {
          if (orderData?.id) {
            await orderService.deleteOrder(orderData.id, orderData.guestToken);
            toast.success("Замовлення скасовано та видалено.");
          }
          navigate("/order-registered");
        } catch (error) {
          console.error("Failed to delete order", error);
          toast.error("Не вдалося скасувати замовлення коректно.");
          navigate("/order-registered");
        }
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
