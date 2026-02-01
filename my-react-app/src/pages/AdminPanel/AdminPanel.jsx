import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import "./AdminPanel.css";
import logoutIco from "../../assets/icons/exit.svg";

// Components
import AdminSidebar from "./components/AdminSidebar";
import AdminProductList from "./components/AdminProductList";

import AdminOrdersList from "./components/AdminOrdersList";
import AdminCatalogSettings from "./components/AdminCatalogSettings";
import AdminReviewsList from "./components/AdminReviewsList";
import AdminUsersList from "./components/AdminUsersList";

// Services
import authService from "../../services/authService";

// Hooks
import { useAdminProducts } from "./hooks/useAdminProducts";
import { useAdminOrders } from "./hooks/useAdminOrders";
import { useAdminCatalog } from "./hooks/useAdminCatalog";
import { useAdminReviews } from "./hooks/useAdminReviews";
import { useAdminUsers } from "./hooks/useAdminUsers";
import { useSettings } from "../../context/useSettings";
import { useAuth } from "../../context/useAuth";

export default function AdminPanel() {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const { lang, setLang } = useSettings();
  const { logout } = useAuth();

  // Tab State
  const [activeTab, setActiveTab] = useState(
    localStorage.getItem("adminActiveTab") || "bouquets",
  );

  // Catalog Sidebar State
  const [isCatalogOpen, setIsCatalogOpen] = useState(() => {
    return ["bouquets", "gifts", "catalog"].includes(activeTab);
  });

  const handleTabChange = (tab) => {
    setActiveTab(tab);
    localStorage.setItem("adminActiveTab", tab);
  };

  const handleLogout = () => {
    logout();
  };

  // --- HOOKS ---
  // Products
  const {
    products,
    giftProducts,
    loading: productsLoading,
    q,
    setQ,
    selectedCategory,
    setSelectedCategory,
    categories,
    handleDeleteProduct,
    handleAddProduct,
    handleEditProduct,
    loadMore: loadProducts,
    hasNextPage: hasNextProducts,
    isLoadingMore: isLoadingProducts,
  } = useAdminProducts(activeTab);

  // Orders
  const {
    orders,
    statuses, // New
    q: orderQ,
    setQ: setOrderQ,
    sort,
    setSort,
    handleStatusChange,
    loadMore: loadOrders,
    hasNextPage: hasNextOrders,
    isLoadingMore: isLoadingOrders,
  } = useAdminOrders();

  // Reviews
  const {
    reviews,
    handleApproveReview,
    handleRejectReview,
    handleDeleteReview,
    loadMore: loadReviews,
    hasNextPage: hasNextReviews,
    isLoadingMore: isLoadingReviews,
  } = useAdminReviews();

  // Users
  const {
    users,
    loading: usersLoading,
    q: userQ,
    setQ: setUserQ,
    loadMore: loadUsers,
    hasNextPage: hasNextUsers,
    isLoadingMore: isLoadingUsers,
    handleUpdateDiscount,
  } = useAdminUsers(activeTab);

  // Catalog Settings
  const { settings, handleEdit } = useAdminCatalog();

  // Render logic
  const renderContent = () => {
    switch (activeTab) {
      case "bouquets":
        return (
          <AdminProductList
            active={activeTab}
            products={products}
            loading={productsLoading}
            q={q}
            setQ={setQ}
            selectedCategory={selectedCategory}
            setSelectedCategory={setSelectedCategory}
            onDelete={handleDeleteProduct}
            onAdd={handleAddProduct}
            onEdit={handleEditProduct}
            loadMore={loadProducts}
            hasNextPage={hasNextProducts}
            isLoadingMore={isLoadingProducts}
          />
        );
      case "gifts":
        return (
          <AdminProductList
            active={activeTab}
            products={products}
            loading={productsLoading}
            q={q}
            setQ={setQ}
            selectedCategory={selectedCategory}
            setSelectedCategory={setSelectedCategory}
            categories={categories}
            onDelete={handleDeleteProduct}
            onAdd={handleAddProduct}
            onEdit={handleEditProduct}
            loadMore={loadProducts}
            hasNextPage={hasNextProducts}
            isLoadingMore={isLoadingProducts}
          />
        );
      case "orders":
        return (
          <AdminOrdersList
            orders={orders}
            statuses={statuses}
            q={orderQ}
            setQ={setOrderQ}
            sort={sort}
            setSort={setSort}
            onStatusChange={handleStatusChange}
            onOrderClick={(id) => navigate(`/admin/orders/${id}`)}
            loadMore={loadOrders}
            hasNextPage={hasNextOrders}
            isLoadingMore={isLoadingOrders}
          />
        );
      case "catalog":
        return (
          <AdminCatalogSettings
            settings={settings}
            onEdit={() => navigate("/admin/catalog/edit")}
          />
        );
      case "reviews":
        return (
          <AdminReviewsList
            reviews={reviews}
            onApprove={handleApproveReview}
            onReject={handleRejectReview}
            onDelete={handleDeleteReview}
            loadMore={loadReviews}
            hasNextPage={hasNextReviews}
            isLoadingMore={isLoadingReviews}
          />
        );
      case "users":
        return (
          <AdminUsersList
            users={users}
            loading={usersLoading}
            q={userQ}
            setQ={setUserQ}
            onUpdateDiscount={handleUpdateDiscount}
            loadMore={loadUsers}
            hasNextPage={hasNextUsers}
            isLoadingMore={isLoadingUsers}
          />
        );
      default:
        return <div>{t("admin.select_tab")}</div>;
    }
  };

  return (
    <div className="admin-root">
      {/* HEADER */}
      <header className="admin-topbar">
        <div className="admin-brand" onClick={() => navigate("/")}>
          <h2 className="admin-brand-top">FLOWER LAB</h2>
          <div className="admin-brand-bottom-box">
            <span className="admin-brand-sub">VLADA</span>
            <span className="admin-brand-tag">ADMIN</span>
          </div>
        </div>

        <div className="admin-topbar-center">{t("admin.title")}</div>

        <div className="admin-topbar-right">
          <div className="admin-lang-switcher">
            <span
              className={lang === "ua" ? "active" : ""}
              onClick={() => setLang("ua")}>
              UA
            </span>
            <span className="divider">/</span>
            <span
              className={lang === "en" ? "active" : ""}
              onClick={() => setLang("en")}>
              ENG
            </span>
          </div>

          <button
            className="admin-top-logout"
            type="button"
            onClick={handleLogout}>
            <img src={logoutIco} alt="Logout" />
            <span>{t("admin.logout")}</span>
          </button>
        </div>
      </header>

      <div className="admin-body">
        {/* SIDEBAR */}
        <AdminSidebar
          active={activeTab}
          setActive={handleTabChange}
          isCatalogOpen={isCatalogOpen}
          setIsCatalogOpen={setIsCatalogOpen}
        />

        {/* CONTENT */}
        <main className="admin-content">{renderContent()}</main>
      </div>
    </div>
  );
}
