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

// Services
import authService from "../../services/authService";

// Hooks
import { useAdminProducts } from "./hooks/useAdminProducts";
import { useAdminOrders } from "./hooks/useAdminOrders";
import { useAdminCatalog } from "./hooks/useAdminCatalog";
import { useAdminReviews } from "./hooks/useAdminReviews";
import { useSettings } from "../../context/useSettings";

export default function AdminPanel() {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const { lang, setLang } = useSettings();

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
    authService.logout();
    navigate("/login");
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
            onPost={handleApproveReview}
            onDelete={handleDeleteReview}
            loadMore={loadReviews}
            hasNextPage={hasNextReviews}
            isLoadingMore={isLoadingReviews}
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
          <span className="admin-brand-sub">VLADA</span>
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
