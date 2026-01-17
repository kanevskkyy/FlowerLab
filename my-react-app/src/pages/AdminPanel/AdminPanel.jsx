import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/useAuth";
import { useMemo, useState, useEffect } from "react";
import toast from "react-hot-toast";
import "./AdminPanel.css"; 

import logoutIco from "../../assets/icons/exit.svg";

// Sub-components
import AdminSidebar from "./components/AdminSidebar";
import AdminProductList from "./components/AdminProductList";
import AdminOrdersList from "./components/AdminOrdersList";
import AdminCatalogSettings from "./components/AdminCatalogSettings";
import AdminReviewsList from "./components/AdminReviewsList";

// Hooks
import { useAdminProducts } from "./hooks/useAdminProducts";
import { useAdminOrders } from "./hooks/useAdminOrders";
import { useAdminReviews } from "./hooks/useAdminReviews";

export default function AdminPanel() {
  const navigate = useNavigate();
  const { logout } = useAuth();

  // --- TAB STATE ---
  const [active, setActive] = useState(() => {
    return localStorage.getItem("adminActiveTab") || "bouquets";
  });

  const [isCatalogOpen, setIsCatalogOpen] = useState(() => {
    const current = localStorage.getItem("adminActiveTab");
    return (
      current === "bouquets" || current === "gifts" || current === "catalog"
    );
  });

  useEffect(() => {
    localStorage.setItem("adminActiveTab", active);
  }, [active]);

  const handleTabChange = (key) => {
    setActive(key);
    // Sidebar logic related to submenu can stay here or be in hook if complex
    if (key !== "bouquets" && key !== "gifts" && key !== "catalog") {
       // logic to close submenus if needed
    }
  };

  const handleSignOut = () => {
    logout();
    toast.success("Successfully logged out");
    navigate("/login", { replace: true });
    localStorage.removeItem("adminActiveTab");
  };


  // --- HOOKS ---
  const { 
    products, 
    q, 
    setQ, 
    handleDeleteProduct 
  } = useAdminProducts(active);

  const { 
    orders, 
    sort, 
    setSort, 
    handleStatusChange 
  } = useAdminOrders();

  const {
    pendingReviews,
    handlePostReview,
    handleDeleteReview
  } = useAdminReviews();


  // --- VIEW HELPERS ---
  const handleEditProduct = (product) => {
      if (active === "gifts") navigate(`/admin/gifts/edit/${product.id}`);
      else navigate(`/admin/bouquets/edit/${product.id}`);
  };

  const handleAddProduct = () => {
    if (active === "gifts") navigate("/admin/gifts/new");
    else navigate("/admin/bouquets/new");
  };

  const catalogSettings = useMemo(
    () => ({
      events: ["Birthday", "Wedding", "Engagement"],
      forWho: ["Mom", "Wife", "Husband", "Kid", "Teacher", "Co-worker"],
      flowerTypes: ["Peony", "Rose", "Lily", "Tulip", "Orchid", "Hydrangea"],
    }),
    []
  );

  return (
    <div className="admin-root">
      {/* HEADER */}
      <header className="admin-topbar">
        <div className="admin-brand ">
          <h2 className="admin-brand-top">FLOWER LAB</h2>
          <span className="admin-brand-sub">VLADA</span>
        </div>

        <div className="admin-topbar-center">Admin panel</div>

        <div className="admin-topbar-right">
          <button
            className="admin-top-logout"
            type="button"
            onClick={handleSignOut}>
            <img src={logoutIco} alt="Logout" />
            <span>Log out</span>
          </button>
        </div>
      </header>

      <div className="admin-body">
        {/* SIDEBAR */}
        <AdminSidebar 
            active={active} 
            setActive={handleTabChange}
            isCatalogOpen={isCatalogOpen}
            setIsCatalogOpen={setIsCatalogOpen}
        />

        {/* CONTENT */}
        <main className="admin-content">
          {/* ========== BOUQUETS & GIFTS MANAGEMENT ========== */}
          {(active === "bouquets" || active === "gifts") && (
            <AdminProductList 
                active={active}
                products={products}
                q={q}
                setQ={setQ}
                onAdd={handleAddProduct}
                onEdit={handleEditProduct}
                onDelete={handleDeleteProduct}
            />
          )}

          {/* ========== ORDERS ========== */}
          {active === "orders" && (
            <AdminOrdersList 
                orders={orders}
                sort={sort}
                setSort={setSort}
                onStatusChange={handleStatusChange}
                onOrderClick={(id) => navigate(`/admin/orders/${id}`)}
            />
          )}

          {/* ========== CATALOG SETTINGS (active === 'catalog') ========== */}
          {active === "catalog" && (
            <AdminCatalogSettings 
                settings={catalogSettings}
                onEdit={() => navigate("/admin/catalog/edit")}
            />
          )}

          {/* ========== REVIEWS ========== */}
          {active === "reviews" && (
            <AdminReviewsList 
                reviews={pendingReviews}
                onPost={handlePostReview}
                onDelete={handleDeleteReview}
            />
          )}
        </main>
      </div>
    </div>
  );
}
