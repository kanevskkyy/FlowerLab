// src/pages/AdminPanel/AdminPanel.jsx
import { useState } from 'react';
import './AdminPanel.css';


import Dashboard from './Dashboard/Dashboard';
import Bouquets from './Bouquets/Bouquets';
import Orders from './Orders/Orders';
import Catalog from './Catalog/Catalog';
import Reviews from './Reviews/Reviews';
import Settings from './Settings/Settings';

const ADMIN_TABS = {
  DASHBOARD: 'dashboard',
  BOUQUETS: 'bouquets',
  ORDERS: 'orders',
  CATALOG: 'catalog',
  REVIEWS: 'reviews',
  SETTINGS: 'settings',
};

export default function AdminPanel({ onLogout }) {
  const [activeTab, setActiveTab] = useState(ADMIN_TABS.DASHBOARD);
const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const renderContent = () => {
    switch (activeTab) {
      case ADMIN_TABS.DASHBOARD:
        return <Dashboard />;
      case ADMIN_TABS.BOUQUETS:
        return <Bouquets />;
      case ADMIN_TABS.ORDERS:
        return <Orders />;
      case ADMIN_TABS.CATALOG:
        return <Catalog />;
      case ADMIN_TABS.REVIEWS:
        return <Reviews />;
      case ADMIN_TABS.SETTINGS:
        return <Settings />;
      default:
        return null;
    }
  };

  return (
    <div className="admin-page">
      {/* HEADER */}
      <header className="admin-header">
  <div className="admin-header-left">
    <button
      className="admin-burger"
      onClick={() => setIsSidebarOpen(prev => !prev)}
    >
      <span></span>
      <span></span>
      <span></span>
    </button>

    <div className="admin-logo">[LOGO]</div>
  </div>

  <div className="admin-title">Admin panel</div>

  <button className="logout-btn" onClick={onLogout}>
    Log out
  </button>
</header>

      {/* MAIN */}
      <main className="admin-main">
        {/* SIDEBAR */}
        <aside className={`admin-sidebar ${isSidebarOpen ? 'open' : ''}`}>

          <SidebarButton
            active={activeTab === ADMIN_TABS.DASHBOARD}
            onClick={() => {
  setActiveTab(ADMIN_TABS.DASHBOARD);
  setIsSidebarOpen(false);
}} 
            label="Dashboard"
          />
          <SidebarButton
            active={activeTab === ADMIN_TABS.BOUQUETS}
            onClick={() => {
  setActiveTab(ADMIN_TABS.BOUQUETS);
  setIsSidebarOpen(false);
}}
            label="Bouquets"
          />
          <SidebarButton
            active={activeTab === ADMIN_TABS.ORDERS}
            onClick={() => {
  setActiveTab(ADMIN_TABS.ORDERS);
  setIsSidebarOpen(false);
}}
            
            label="Orders"
          />
          <SidebarButton
            active={activeTab === ADMIN_TABS.CATALOG}
           onClick={() => {
  setActiveTab(ADMIN_TABS.CATALOG);
  setIsSidebarOpen(false);
}}
            label="Catalog"
          />
          <SidebarButton
            active={activeTab === ADMIN_TABS.REVIEWS}
            onClick={() => {
  setActiveTab(ADMIN_TABS.REVIEWS);
  setIsSidebarOpen(false);
}}
            label="Reviews"
          />
          <SidebarButton
            active={activeTab === ADMIN_TABS.SETTINGS}
            onClick={() => {
  setActiveTab(ADMIN_TABS.SETTINGS);
  setIsSidebarOpen(false);
}}
            label="Settings"
          />
        </aside>

        {/* CONTENT */}
        <section className="admin-content">
          {renderContent()}
        </section>
      </main>
    </div>
  );
}

function SidebarButton({ active, onClick, label }) {
  return (
    <button
      className={`admin-sidebar-item ${active ? 'active' : ''}`}
      onClick={onClick}
    >
      {label}
    </button>
  );
}
