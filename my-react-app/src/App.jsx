import { useEffect } from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import AuthProvider from "./context/AuthProvider";
import { CartProvider } from "./context/CartProvider";
import SettingsProvider from "./context/SettingsProvider";
import { Toaster } from "react-hot-toast";
import ProtectedRoute from "./routes/ProtectedRoute";
import AdminProtectedRoute from "./routes/AdminProtectedRoute";

// pages
// pages
import HomePage from "./pages/HomePage/HomePage";
import Catalog from "./pages/Catalog/Catalog";
import ProductCard from "./pages/ProductCard/ProductCard";
import AboutUs from "./pages/AboutUs/AboutUs";

import Login from "./pages/Login/Login";
import Register from "./pages/Register/Register";
import Cabinet from "./pages/Cabinet/Cabinet";

import OrderPlacementRegistered from "./pages/OrderPlacementRegistered/OrderPlacementRegistered";
import CheckOut from "./pages/CheckOut/CheckOut";
import OrderSuccess from "./pages/OrderSuccess/OrderSuccess";

// admin (створиш/підключиш)
import AdminPanel from "./pages/AdminPanel/AdminPanel";
import AdminBouquetForm from "./pages/AdminBouquetForm/AdminBouquetForm";
import AdminCatalogEdit from "./pages/AdminCatalogEdit/AdminCatalogEdit";
import AdminOrderDetails from "./pages/AdminOrderDetails/AdminOrderDetails";
import NotFound from "./pages/NotFound/NotFound";
import Gifts from "./pages/Gifts/Gifts";
import ForgotPassword from "./pages/ForgotPassword/ForgotPassword";
import ResetPassword from "./pages/ResetPassword/ResetPassword";
import EmailConfirmationPending from "./pages/EmailConfirmation/EmailConfirmationPending";
import EmailConfirmation from "./pages/EmailConfirmation/EmailConfirmation";

import ModalProvider from "./context/ModalProvider";

// Global handler for back-navigation reloads
const BackNavigationHandler = () => {
  useEffect(() => {
    const onResume = (event) => {
      if (event && event.persisted) {
        window.location.reload();
        return;
      }

      const navEntry = performance.getEntriesByType("navigation")[0];
      if (navEntry && navEntry.type === "back_forward") {
        const hasReloaded = sessionStorage.getItem("global_reloaded");
        if (!hasReloaded) {
          sessionStorage.setItem("global_reloaded", "true");
          window.location.reload();
        } else {
          sessionStorage.removeItem("global_reloaded");
        }
      } else {
        sessionStorage.removeItem("global_reloaded");
      }
    };

    window.addEventListener("pageshow", onResume);
    return () => window.removeEventListener("pageshow", onResume);
  }, []);

  return null;
};

function App() {
  return (
    <BrowserRouter>
      <BackNavigationHandler />
      <AuthProvider>
        <CartProvider>
          <SettingsProvider>
            <ModalProvider>
              <Toaster
                position="top-center"
                toastOptions={{
                  duration: 3000,
                  style: {
                    background: "#333",
                    color: "#fff",
                  },
                  success: {
                    iconTheme: {
                      primary: "#F4BCE5", // Ваш фірмовий рожевий
                      secondary: "#fff",
                    },
                  },
                }}
              />
              <Routes>
                {/* public */}
                <Route path="/" element={<HomePage />} />
                <Route path="/catalog" element={<Catalog />} />
                <Route path="/gifts" element={<Gifts />} />
                <Route path="/product/:id" element={<ProductCard />} />
                <Route path="/about" element={<AboutUs />} />

                <Route path="/order" element={<OrderPlacementRegistered />} />
                <Route
                  path="/order-registered"
                  element={<Navigate to="/order" replace />}
                />
                <Route path="/checkout" element={<CheckOut />} />
                <Route path="/order-success" element={<OrderSuccess />} />

                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/forgot-password" element={<ForgotPassword />} />
                <Route path="/reset-password" element={<ResetPassword />} />

                {/* Email Confirmation */}
                <Route
                  path="/email-confirmation-pending"
                  element={<EmailConfirmationPending />}
                />
                <Route path="/confirm-email" element={<EmailConfirmation />} />

                {/* user protected */}
                <Route
                  path="/cabinet"
                  element={
                    <ProtectedRoute>
                      <Cabinet />
                    </ProtectedRoute>
                  }
                />

                {/* admin protected */}
                <Route
                  path="/admin"
                  element={
                    <AdminProtectedRoute>
                      <AdminPanel />
                    </AdminProtectedRoute>
                  }
                />

                <Route
                  path="/admin/bouquets/new"
                  element={
                    <AdminProtectedRoute>
                      <AdminBouquetForm />
                    </AdminProtectedRoute>
                  }
                />

                <Route
                  path="/admin/bouquets/edit/:id"
                  element={
                    <AdminProtectedRoute>
                      <AdminBouquetForm />
                    </AdminProtectedRoute>
                  }
                />
                <Route
                  path="/admin/gifts/new"
                  element={
                    <AdminProtectedRoute>
                      <AdminBouquetForm />
                    </AdminProtectedRoute>
                  }
                />
                <Route
                  path="/admin/gifts/edit/:id"
                  element={
                    <AdminProtectedRoute>
                      <AdminBouquetForm />
                    </AdminProtectedRoute>
                  }
                />
                <Route
                  path="/admin/catalog/edit"
                  element={
                    <AdminProtectedRoute>
                      <AdminCatalogEdit />
                    </AdminProtectedRoute>
                  }
                />
                <Route
                  path="/admin/orders/:id"
                  element={
                    <AdminProtectedRoute>
                      <AdminOrderDetails />
                    </AdminProtectedRoute>
                  }
                />

                <Route path="*" element={<NotFound />} />
              </Routes>
            </ModalProvider>
          </SettingsProvider>
        </CartProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
