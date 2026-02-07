import { useState, useEffect } from "react";
import { FormProvider } from "react-hook-form";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../../components/PopupMenu/PopupMenu";
import { useTranslation } from "react-i18next";
import { useOrderPlacement } from "./hooks/useOrderPlacement";

// Sub-components
import ContactInfo from "./components/ContactInfo";
import ReceiverInfo from "./components/ReceiverInfo";
import DeliveryInfo from "./components/DeliveryInfo";
import GiftsSlider from "./components/GiftsSlider";
import OrderSummary from "./components/OrderSummary";

import "./OrderPlacementRegistered.css";

const OrderPlacement = () => {
  const { t } = useTranslation();
  const [menuOpen, setMenuOpen] = useState(false);

  const {
    methods,
    onSubmit,
    gifts,
    giftsLoading,
    cartItems,
    deliveryAddresses,
    shopAddresses,
    isAddingAddress,
    setIsAddingAddress,
    newAddress,
    setNewAddress,
    selectedGifts,
    isCardAdded,
    subtotal,
    discount,
    discountPercentage,
    deliveryFee,
    deliveryType,
    total,
    toggleGift,
    toggleCard,
    handleAddAddress,
    removeItem,
    navigate,
  } = useOrderPlacement();

  return (
    <div className="page-wrapper order-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="order-placement">
        <h1 className="order-title">{t("checkout.order_placement")}</h1>

        <button className="back-btn" onClick={() => navigate(-1)}>
          <span>←</span> {t("checkout.back_to_shopping")}
        </button>

        <FormProvider {...methods}>
          <form className="order-content" onSubmit={onSubmit}>
            {/* LEFT SIDE - Form */}
            <div className="order-form-section">
              <ContactInfo />

              <ReceiverInfo />

              <DeliveryInfo
                deliveryAddresses={deliveryAddresses}
                shopAddresses={shopAddresses}
                isAddingAddress={isAddingAddress}
                setIsAddingAddress={setIsAddingAddress}
                newAddress={newAddress}
                setNewAddress={setNewAddress}
                handleAddAddress={handleAddAddress}
              />

              <GiftsSlider
                gifts={gifts}
                giftsLoading={giftsLoading}
                selectedGifts={selectedGifts}
                toggleGift={toggleGift}
              />
            </div>

            {/* RIGHT SIDE - Order Summary */}
            <OrderSummary
              cartItems={cartItems}
              removeItem={removeItem}
              selectedGifts={selectedGifts}
              gifts={gifts}
              toggleGift={toggleGift}
              subtotal={subtotal}
              discount={discount}
              discountPercentage={discountPercentage}
              deliveryFee={deliveryFee}
              deliveryType={deliveryType}
              total={total}
              isCardAdded={isCardAdded}
              toggleCard={toggleCard}
            />
          </form>
        </FormProvider>
      </main>

      <Footer />
    </div>
  );
};

export default OrderPlacement;
