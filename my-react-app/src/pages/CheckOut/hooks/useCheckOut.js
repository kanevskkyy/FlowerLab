import { useState } from "react";
import { useNavigate } from "react-router-dom";

export function useCheckOut({ orderData }) {
  const navigate = useNavigate();
  
  const [paymentMethod, setPaymentMethod] = useState(""); // "card" or "privat24"
  const [cardNumber, setCardNumber] = useState("");
  const [expiry, setExpiry] = useState("");
  const [cvv, setCvv] = useState("");
  const [sendReceipt, setSendReceipt] = useState(false);
  
  // Simulated order number
  const [orderNumber] = useState(() => `3663092/${Math.floor(Math.random() * 900000) + 100000}`);

  const total = orderData?.total || 1000;

  const handleCardNumberChange = (e) => {
    let value = e.target.value.replace(/\s/g, "");
    if (value.length <= 16) {
      value = value.match(/.{1,4}/g)?.join(" ") || value;
      setCardNumber(value);
    }
  };

  const handleExpiryChange = (e) => {
    let value = e.target.value.replace(/\D/g, "");
    if (value.length <= 4) {
      if (value.length >= 2) {
        value = value.slice(0, 2) + "/" + value.slice(2);
      }
      setExpiry(value);
    }
  };

  const handleCvvChange = (e) => {
    const value = e.target.value.replace(/\D/g, "");
    if (value.length <= 3) {
      setCvv(value);
    }
  };

  const handleCompletePayment = () => {
    if (!paymentMethod) {
      alert("Please select a payment method");
      return;
    }

    if (paymentMethod === "card") {
      if (!cardNumber || !expiry || !cvv) {
        alert("Please fill in all card details");
        return;
      }
    }

    console.log("Payment completed:", {
      orderNumber,
      paymentMethod,
      total,
      cardNumber: cardNumber.replace(/\s/g, "").slice(-4),
    });

    // Перенаправлення на сторінку успіху
    alert("Payment successful! Order #" + orderNumber);
    navigate("/");
  };

  const handleCancelPayment = () => {
    if (window.confirm("Are you sure you want to cancel the payment?")) {
      navigate("/order-registered");
    }
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
    handleCancelPayment
  };
}
