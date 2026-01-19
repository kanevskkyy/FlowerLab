import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useConfirm } from "../../../context/ModalProvider";

export function useCheckOut({ orderData }) {
  const navigate = useNavigate();
  const confirm = useConfirm();

  // ...

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
    paymentMethod,
    setPaymentMethod,
    cardNumber,
    expiry,
    cvv,
    sendReceipt,
    setSendReceipt,
    orderNumber,
    total,
    handleCardNumberChange,
    handleExpiryChange,
    handleCvvChange,
    handleCompletePayment,
    handleCancelPayment,
  };
}
