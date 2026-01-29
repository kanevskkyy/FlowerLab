import React from "react";
import { useFormContext } from "react-hook-form";
import { useTranslation } from "react-i18next";

const DeliveryInfo = ({
  deliveryAddresses,
  shopAddresses,
  isAddingAddress,
  setIsAddingAddress,
  newAddress,
  setNewAddress,
  handleAddAddress,
}) => {
  const { t } = useTranslation();
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
      <h2>{t("checkout.delivery_title")}</h2>

      <div className="delivery-tabs">
        <button
          type="button"
          className={
            deliveryType === "pickup" ? "delivery-tab active" : "delivery-tab"
          }
          onClick={() =>
            setValue("deliveryType", "pickup", { shouldValidate: true })
          }>
          {t("checkout.pickup_tab")}
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
          {t("checkout.delivery_tab")}
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
                  <span>
                    {addr.text}
                    {String(addr.id).startsWith("temp-") && (
                      <span
                        style={{
                          fontSize: "0.85em",
                          color: "#999",
                          marginLeft: "6px",
                          fontStyle: "italic",
                        }}>
                        {t("checkout.new_label")}
                      </span>
                    )}
                  </span>
                </div>
              ))
            ) : (
              <p className="no-data-text">{t("checkout.no_addresses")}</p>
            )}
          </div>

          {isAddingAddress ? (
            <div className="new-address-form">
              <input
                type="text"
                placeholder={t("checkout.address_placeholder")}
                value={newAddress}
                onChange={(e) => setNewAddress(e.target.value)}
                className="new-address-input"
              />
              <div className="new-address-buttons">
                <button
                  type="button"
                  onClick={handleAddAddress}
                  className="save-address-btn">
                  {t("cabinet.save")}
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setIsAddingAddress(false);
                    setNewAddress("");
                  }}
                  className="cancel-address-btn">
                  {t("cabinet.cancel")}
                </button>
              </div>
            </div>
          ) : (
            <button
              type="button"
              className="add-address-btn"
              onClick={() => setIsAddingAddress(true)}>
              {t("checkout.add_address")}
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
