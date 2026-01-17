import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Cabinet.css";

import Header from "../../components/Header/Header";
import PopupMenu from "../popupMenu/PopupMenu";
import { useAuth } from "../../context/useAuth";

// Sub-components
import CabinetSidebar from "./components/CabinetSidebar";
import PersonalTab from "./components/PersonalTab";
import OrdersTab from "./components/OrdersTab";
import AddressesTab from "./components/AddressesTab";

const TABS = {
  PERSONAL: "personal",
  ORDERS: "orders",
  ADDRESSES: "addresses",
};

export default function Cabinet() {
  const navigate = useNavigate();
  const { logout } = useAuth();

  const [menuOpen, setMenuOpen] = useState(false);
  const [activeTab, setActiveTab] = useState(TABS.PERSONAL);

  const handleSignOut = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <div className="cabinet-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="cabinet-main">
        <div className="cabinet-shell">
          {/* SIDEBAR */}
          <CabinetSidebar 
            activeTab={activeTab} 
            setActiveTab={setActiveTab} 
            TABS={TABS} 
            onSignOut={handleSignOut} 
          />

          {/* PANEL CONTENT */}
          <section className="cabinet-panel">
            {activeTab === TABS.PERSONAL && <PersonalTab />}
            {activeTab === TABS.ORDERS && <OrdersTab activeTab={activeTab} TABS={TABS} />}
            {activeTab === TABS.ADDRESSES && <AddressesTab activeTab={activeTab} TABS={TABS} />}
          </section>
        </div>
      </main>
    </div>
  );
}
