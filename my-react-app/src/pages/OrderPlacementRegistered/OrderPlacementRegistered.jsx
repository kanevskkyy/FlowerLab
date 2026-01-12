import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import PopupMenu from "../popupMenu/PopupMenu";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { useCart } from "../../context/CartContext";
import "./OrderPlacementRegistered.css";
import trash from "../../assets/icons/trash.svg";

// Import images for gifts
import gift1 from "../../assets/images/gift1.jpg";
import gift2 from "../../assets/images/gift2.jpg";
import gift3 from "../../assets/images/gift3.png";
import toast from "react-hot-toast";

// === 1. Zod Schema ===
const schema = z
  .object({
    firstName: z.string().min(1, "First Name is required"),
    lastName: z.string().min(1, "Last Name is required"),
    phone: z.string().regex(/^\+?[0-9]{10,12}$/, "Invalid phone format"),
    receiverType: z.enum(["self", "other"]),
    deliveryType: z.enum(["pickup", "delivery"]),
    message: z.string().optional(),
    noCall: z.boolean().optional(),
    selectedAddressId: z.number().optional(),
    selectedShopId: z.number().optional(),
  })
  .superRefine((data, ctx) => {
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
  });

const OrderPlacement = () => {
  const navigate = useNavigate();
  const { cartItems, removeItem } = useCart();
  const [menuOpen, setMenuOpen] = useState(false);
  const [isRegistered, setIsRegistered] = useState(true);

  // Local UI state
  const [isAddingAddress, setIsAddingAddress] = useState(false);
  const [newAddress, setNewAddress] = useState("");
  const [selectedGifts, setSelectedGifts] = useState([]);
  const [isCardAdded, setIsCardAdded] = useState(false);

  // Addresses data
  const [deliveryAddresses, setDeliveryAddresses] = useState([
    { id: 1, text: "м. Чернівці, вул. Головна 446, кв. 12" },
  ]);

  const shopAddresses = [
    { id: 1, text: "м. Чернівці, вул Герцена 2а" },
    { id: 2, text: "вул Васіле Александрі, 1" },
  ];

  const gifts = [
    { id: 1, name: "Gift", price: 1000, img: gift1 },
    { id: 2, name: "Star Baloon", price: 1000, img: gift2 },
    { id: 3, name: "Gift", price: 1000, img: gift3 },
  ];

  // === 2. Setup React Hook Form ===
  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(schema),
    defaultValues: {
      firstName: "",
      lastName: "",
      phone: "",
      receiverType: "self",
      deliveryType: "delivery",
      selectedAddressId: 1,
      selectedShopId: 1,
      message: "",
      noCall: false,
    },
  });

  // Watch fields
  const deliveryType = watch("deliveryType");
  const selectedAddressId = watch("selectedAddressId");
  const selectedShopId = watch("selectedShopId");

  // === Calculations ===
  const subtotal = cartItems.reduce((sum, item) => {
    const price =
      typeof item.price === "string"
        ? parseFloat(item.price.replace(/[^\d]/g, ""))
        : item.price;
    const qty = item.qty || 1;
    return sum + price * qty;
  }, 0);

  const deliveryCost = deliveryType === "delivery" ? 100 : 0;
  const discount = subtotal * 0.1;
  const giftsTotal = selectedGifts.reduce((sum, id) => {
    const gift = gifts.find((g) => g.id === id);
    return sum + (gift?.price || 0);
  }, 0);
  const cardFee = isCardAdded ? 50 : 0;
  const total = subtotal + deliveryCost - discount + giftsTotal + cardFee;

  // === Handlers ===
  const toggleGift = (giftId) => {
    setSelectedGifts((prev) =>
      prev.includes(giftId)
        ? prev.filter((id) => id !== giftId)
        : [...prev, giftId]
    );
  };

  const toggleCard = () => {
    setIsCardAdded((prev) => !prev);
  };

  const handleAddAddress = () => {
    if (newAddress.trim()) {
      const newId = Math.max(...deliveryAddresses.map((a) => a.id), 0) + 1;
      setDeliveryAddresses((prev) => [
        ...prev,
        { id: newId, text: newAddress },
      ]);
      // Auto-select new address
      setValue("selectedAddressId", newId, { shouldValidate: true });
      setNewAddress("");
      setIsAddingAddress(false);
    }
  };

  const onSubmit = (data) => {
    const finalOrderData = {
      ...data,
      cartItems,
      selectedGifts,
      isCardAdded,
      total,
      deliveryAddressText:
        deliveryType === "delivery"
          ? deliveryAddresses.find((a) => a.id === data.selectedAddressId)?.text
          : null,
      shopAddressText:
        deliveryType === "pickup"
          ? shopAddresses.find((a) => a.id === data.selectedShopId)?.text
          : null,
    };
    toast.success("Order validated! Proceeding to checkout...");
    console.log("Order confirmed:", finalOrderData);
    navigate("/checkout", { state: { orderData: finalOrderData } });
  };

  return (
    <div className="page-wrapper order-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />
      <main className="order-placement">
        <h1 className="order-title">ORDER PLACEMENT</h1>

        <button className="back-btn" onClick={() => navigate(-1)}>
          <span>←</span> Back to shopping
        </button>

        {/* ✅ ВАЖЛИВО: Використовуємо <form> з класом 'order-content'. 
            Це зберігає Grid-верстку з CSS (дві колонки), але дозволяє сабміт. */}
        <form className="order-content" onSubmit={handleSubmit(onSubmit)}>
          {/* LEFT SIDE - Form */}
          <div className="order-form-section">
            {/* Contact Information */}
            <section className="form-section">
              <h2>Your contact information</h2>

              <div className="registration-tabs">
                <button
                  type="button"
                  className={isRegistered ? "tab active" : "tab"}
                  onClick={() => setIsRegistered(true)}>
                  I am registered
                </button>
                <button
                  type="button"
                  className={!isRegistered ? "tab active" : "tab"}
                  onClick={() => setIsRegistered(false)}>
                  Buy without registering
                </button>
              </div>

              <div className="form-group">
                <label>First Name</label>
                <input
                  type="text"
                  placeholder="First Name"
                  // Додаємо клас помилки, якщо вона є
                  style={
                    errors.firstName
                      ? { border: "1px solid #d32f2f", background: "#fffbfb" }
                      : {}
                  }
                  {...register("firstName")}
                />
                {errors.firstName && (
                  <p className="error-text">{errors.firstName.message}</p>
                )}
              </div>

              <div className="form-group">
                <label>Last Name</label>
                <input
                  type="text"
                  placeholder="Last Name"
                  style={
                    errors.lastName
                      ? { border: "1px solid #d32f2f", background: "#fffbfb" }
                      : {}
                  }
                  {...register("lastName")}
                />
                {errors.lastName && (
                  <p className="error-text">{errors.lastName.message}</p>
                )}
              </div>

              <div className="form-group">
                <label>Phone</label>
                <input
                  type="tel"
                  placeholder="+38 066 001 02 03"
                  style={
                    errors.phone
                      ? { border: "1px solid #d32f2f", background: "#fffbfb" }
                      : {}
                  }
                  {...register("phone")}
                />
                {errors.phone && (
                  <p className="error-text">{errors.phone.message}</p>
                )}
              </div>

              <div className="receiver-section">
                <label>Receiver:</label>
                <div className="radio-group">
                  <label className="radio-label">
                    <input
                      type="radio"
                      value="self"
                      {...register("receiverType")}
                    />
                    <span>I am the receiver</span>
                  </label>
                  <label className="radio-label">
                    <input
                      type="radio"
                      value="other"
                      {...register("receiverType")}
                    />
                    <span>The receiver is other person</span>
                  </label>
                </div>
              </div>
            </section>

            {/* Delivery */}
            <section className="form-section">
              <h2>Delivery</h2>

              <div className="delivery-tabs">
                <button
                  type="button"
                  className={
                    deliveryType === "pickup"
                      ? "delivery-tab active"
                      : "delivery-tab"
                  }
                  onClick={() =>
                    setValue("deliveryType", "pickup", { shouldValidate: true })
                  }>
                  Pickup at the shop
                </button>
                <button
                  type="button"
                  className={
                    deliveryType === "delivery"
                      ? "delivery-tab active"
                      : "delivery-tab"
                  }
                  onClick={() =>
                    setValue("deliveryType", "delivery", {
                      shouldValidate: true,
                    })
                  }>
                  Delivery
                </button>
              </div>

              {errors.deliveryType && (
                <p className="error-text">{errors.deliveryType.message}</p>
              )}

              {/* DELIVERY */}
              {deliveryType === "delivery" && (
                <>
                  <div className="addresses-list">
                    {deliveryAddresses.map((addr) => (
                      <div
                        key={addr.id}
                        className={`address-item ${
                          selectedAddressId === addr.id ? "active" : ""
                        }`}
                        // Оновлюємо значення через setValue
                        onClick={() =>
                          setValue("selectedAddressId", addr.id, {
                            shouldValidate: true,
                          })
                        }>
                        <input
                          type="radio"
                          name="selectedAddress"
                          checked={selectedAddressId === addr.id}
                          readOnly
                        />
                        <span>{addr.text}</span>
                      </div>
                    ))}
                  </div>

                  {isAddingAddress ? (
                    <div className="new-address-form">
                      <input
                        type="text"
                        placeholder="Enter new address"
                        value={newAddress}
                        onChange={(e) => setNewAddress(e.target.value)}
                        className="new-address-input"
                      />
                      <div className="new-address-buttons">
                        <button
                          type="button"
                          onClick={handleAddAddress}
                          className="save-address-btn">
                          Save
                        </button>
                        <button
                          type="button"
                          onClick={() => {
                            setIsAddingAddress(false);
                            setNewAddress("");
                          }}
                          className="cancel-address-btn">
                          Cancel
                        </button>
                      </div>
                    </div>
                  ) : (
                    <button
                      type="button"
                      className="add-address-btn"
                      onClick={() => setIsAddingAddress(true)}>
                      + Add a new address
                    </button>
                  )}
                </>
              )}

              {/* PICKUP */}
              {deliveryType === "pickup" && (
                <div className="addresses-list">
                  {shopAddresses.map((addr) => (
                    <div
                      key={addr.id}
                      className={`address-item ${
                        selectedShopId === addr.id ? "active" : ""
                      }`}
                      onClick={() =>
                        setValue("selectedShopId", addr.id, {
                          shouldValidate: true,
                        })
                      }>
                      <input
                        type="radio"
                        name="selectedShopAddress"
                        checked={selectedShopId === addr.id}
                        readOnly
                      />
                      <span>{addr.text}</span>
                    </div>
                  ))}
                </div>
              )}
            </section>

            {/* Gifts */}
            <section className="form-section">
              <h2>Gifts</h2>
              <div className="gifts-grid">
                {gifts.map((gift) => (
                  <div key={gift.id} className="gift-card">
                    <div className="gift-image">
                      <img src={gift.img} alt={gift.name} />
                    </div>
                    <div className="gift-bottom">
                      <div className="gift-info">
                        <p className="gift-name">{gift.name}</p>
                        <p className="gift-price">{gift.price} ₴</p>
                      </div>
                      <button
                        type="button"
                        className={`add-gift-btn ${
                          selectedGifts.includes(gift.id) ? "active" : ""
                        }`}
                        onClick={() => toggleGift(gift.id)}>
                        +
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          </div>

          {/* RIGHT SIDE - Order Summary */}
          <div className="order-summary">
            <h2>YOUR ORDER:</h2>

            <div className="order-items">
              {cartItems.map((item) => (
                <div key={item.id} className="order-item">
                  <img src={item.img} alt={item.title} />
                  <div className="item-details">
                    <p className="item-name">{item.title}</p>
                    <p className="item-price">
                      {typeof item.price === "string"
                        ? item.price
                        : `${item.price} ₴`}
                    </p>
                    <p className="item-quantity">{item.qty || 1} pc</p>
                  </div>
                  <button
                    type="button"
                    className="remove-item-btn"
                    onClick={() => removeItem(item.id)}>
                    <img src={trash} alt="Remove" />
                  </button>
                </div>
              ))}

              {selectedGifts.map((giftId) => {
                const gift = gifts.find((g) => g.id === giftId);
                if (!gift) return null;
                return (
                  <div key={`gift-${gift.id}`} className="order-item">
                    <img src={gift.img} alt={gift.name} />
                    <div className="item-details">
                      <p className="item-name">{gift.name}</p>
                      <p className="item-price">{gift.price} ₴</p>
                      <p className="item-quantity">1 pc</p>
                    </div>
                    <button
                      type="button"
                      className="remove-item-btn"
                      onClick={() => toggleGift(gift.id)}>
                      <img src={trash} alt="Remove" />
                    </button>
                  </div>
                );
              })}
            </div>

            <div className="order-calculations">
              <div className="calc-row">
                <span>Subtotal:</span>
                <span>{subtotal} ₴</span>
              </div>
              <div className="calc-row">
                <span>Delivery:</span>
                <span>{deliveryCost} ₴</span>
              </div>
              <div className="calc-row discount">
                <span>Discount:</span>
                <span>10%</span>
              </div>
              <div className="calc-row total">
                <span>TOTAL:</span>
                <span>{total} ₴</span>
              </div>
            </div>

            <div className="add-card-section">
              <button
                type="button"
                className="add-card-btn"
                onClick={toggleCard}>
                {isCardAdded ? "- Remove card" : "+ Add a card"}
              </button>
              <span className="card-price">+ 50 ₴</span>
            </div>

            <textarea
              className="message-input"
              placeholder="Input your message"
              {...register("message")}
            />

            <label className="checkbox-label">
              <input type="checkbox" {...register("noCall")} />
              <span>Do not call me, I am certain with my order</span>
            </label>

            <button type="submit" className="confirm-btn">
              CONFIRM ORDER
            </button>
          </div>
        </form>
      </main>

      <Footer />
    </div>
  );
};

export default OrderPlacement;
