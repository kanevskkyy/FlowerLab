import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import AuthProvider from "./context/AuthProvider";
import { CartProvider } from "./context/CartProvider";
import SettingsProvider from "./context/SettingsProvider";
import { Toaster } from "react-hot-toast";
import ProtectedRoute from "./routes/ProtectedRoute";
import AdminProtectedRoute from "./routes/AdminProtectedRoute";

// pages
import HomePage from "./pages/HomePage/HomePage";
import Catalog from "./pages/Catalog/Catalog";
import ProductCard from "./pages/ProductCard/ProductCard";
import AboutUs from "./pages/AboutUs/AboutUs";

import Login from "./pages/Login/Login";
import Register from "./pages/Register/Register";
import Cabinet from "./pages/Cabinet/Cabinet";

// твої існуючі
import OrderPlacementPickUp from "./pages/OrderPlacementPickUp/OrderPlacementPickUp";
import OrderPlacementRegistered from "./pages/OrderPlacementRegistered/OrderPlacementRegistered";
import CheckOut from "./pages/CheckOut/CheckOut";

// admin (створиш/підключиш)
import AdminPanel from "./pages/AdminPanel/AdminPanel";
import AdminBouquetForm from "./pages/AdminBouquetForm/AdminBouquetForm";
import AdminCatalogEdit from "./pages/AdminCatalogEdit/AdminCatalogEdit";
import AdminOrderDetails from "./pages/AdminOrderDetails/AdminOrderDetails";
import NotFound from "./pages/NotFound/NotFound";
import Gifts from "./pages/Gifts/Gifts";
import ForgotPassword from "./pages/ForgotPassword/ForgotPassword";
import ResetPassword from "./pages/ResetPassword/ResetPassword";

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <CartProvider>
          <SettingsProvider>
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

              <Route path="/order" element={<OrderPlacementPickUp />} />
              <Route
                path="/order-registered"
                element={<OrderPlacementRegistered />}
              />
              <Route path="/checkout" element={<CheckOut />} />

              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route path="/forgot-password" element={<ForgotPassword />} />
              <Route path="/reset-password" element={<ResetPassword />} />

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
          </SettingsProvider>
        </CartProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
