import React from "react";
import UserInfoIcon from "../../../assets/icons/userinfo.svg";
import OrderIcon from "../../../assets/icons/orders.svg";
import AddressIcon from "../../../assets/icons/address.svg";
import ExitIcon from "../../../assets/icons/exit.svg";

export default function CabinetSidebar({ activeTab, setActiveTab, TABS, onSignOut }) {
  return (
    <aside className="cabinet-sidebar">
      <button
        className={`cabinet-nav-item ${
          activeTab === TABS.PERSONAL ? "active" : ""
        }`}
        onClick={() => setActiveTab(TABS.PERSONAL)}
        type="button"
      >
        <img src={UserInfoIcon} className="cabinet-nav-icon" alt="" />
        <span>Personal information</span>
      </button>

      <button
        className={`cabinet-nav-item ${
          activeTab === TABS.ORDERS ? "active" : ""
        }`}
        onClick={() => setActiveTab(TABS.ORDERS)}
        type="button"
      >
        <img src={OrderIcon} className="cabinet-nav-icon" alt="" />
        <span>My orders</span>
      </button>

      <button
        className={`cabinet-nav-item ${
          activeTab === TABS.ADDRESSES ? "active" : ""
        }`}
        onClick={() => setActiveTab(TABS.ADDRESSES)}
        type="button"
      >
        <img src={AddressIcon} className="cabinet-nav-icon" alt="" />
        <span>Saved addresses</span>
      </button>

      <div className="cabinet-sidebar-spacer" />

      <button className="cabinet-signout" onClick={onSignOut} type="button">
        <img src={ExitIcon} className="cabinet-nav-icon" alt="" />
        <span>Sign out</span>
      </button>
    </aside>
  );
}
