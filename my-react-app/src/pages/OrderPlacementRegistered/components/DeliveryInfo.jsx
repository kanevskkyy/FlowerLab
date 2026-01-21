import React from "react";
import { useFormContext } from "react-hook-form";

const DeliveryInfo = ({
  deliveryAddresses,
  shopAddresses,
  isAddingAddress,
  setIsAddingAddress,
  newAddress,
  setNewAddress,
  handleAddAddress,
}) => {
  const {
    watch,
    setValue,
    formState: { errors },
  } = useFormContext();

  const deliveryType = watch("deliveryType");
  const selectedAddressId = watch("selectedAddressId");
  const selectedShopId = watch("selectedShopId");

  return (
    <section className="form-section">
      <h2>Delivery</h2>

      <div className="delivery-tabs">
        <button
          type="button"
          className={
            deliveryType === "pickup" ? "delivery-tab active" : "delivery-tab"
          }
          onClick={() =>
            setValue("deliveryType", "pickup", { shouldValidate: true })
          }>
          Pickup at the shop
        </button>
        <button
          type="button"
          className={
            deliveryType === "delivery" ? "delivery-tab active" : "delivery-tab"
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
            {deliveryAddresses.length > 0 ? (
              deliveryAddresses.map((addr) => (
                <div
                  key={addr.id}
                  className={`address-item ${
                    selectedAddressId === addr.id ? "active" : ""
                  }`}
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
              ))
            ) : (
              <p className="no-data-text">No saved addresses found.</p>
            )}
          </div>

          {isAddingAddress ? (
            <div className="new-address-form">
              <input
                type="text"
                placeholder="Enter new address (City, Street, House, Apt)"
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
  );
};

export default DeliveryInfo;
