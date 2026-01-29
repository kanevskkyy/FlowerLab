import React from "react";
import { useTranslation } from "react-i18next";
import { useMyAddresses } from "../hooks/useMyAddresses";

// Icons
import AddressIcon from "../../../assets/icons/address.svg";
import TrashIcon from "../../../assets/icons/trash.svg";

export default function AddressesTab({ activeTab, TABS }) {
  const { t } = useTranslation();
  const {
    addressList,
    newAddress,
    setNewAddress,
    handleSaveAddress,
    handleDeleteAddress,
  } = useMyAddresses(activeTab, TABS);

  return (
    <div className="cabinet-panel-inner cabinet-addresses">
      <h1 className="cabinet-title">{t("cabinet.saved_addresses")}</h1>

      <div className="cabinet-grid-1">
        {/* List Existing Addresses */}
        {addressList.map((addr) => (
          <div
            key={addr.id}
            className="cabinet-pill"
            style={{ marginBottom: "1rem" }}>
            <div className="cabinet-pill-left">
              <img src={AddressIcon} className="cabinet-pill-icon" alt="" />
              <span className="cabinet-pill-text">{addr.address}</span>
            </div>
            <button
              className="cabinet-pill-btn cabinet-delete-btn"
              type="button"
              onClick={() => handleDeleteAddress(addr.id)}
              title={t("cabinet.delete_address")}>
              <img
                src={TrashIcon}
                alt="Delete"
                style={{ width: 20, height: 20 }}
              />
            </button>
          </div>
        ))}
      </div>

      <div className="cabinet-grid-1" style={{ marginTop: "2rem" }}>
        <div className="cabinet-field">
          <label>{t("cabinet.add_new_address")}</label>
          <input
            value={newAddress}
            onChange={(e) => setNewAddress(e.target.value)}
            placeholder={t("cabinet.enter_address_placeholder")}
          />
        </div>
      </div>

      <button
        className="cabinet-save cabinet-addresses-save"
        type="button"
        onClick={handleSaveAddress}
        disabled={!newAddress.trim()}>
        {t("cabinet.add_address_btn")}
      </button>
    </div>
  );
}
