import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import AuthProvider from "./context/AuthProvider";
import { CartProvider } from "./context/CartProvider";

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

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <CartProvider>
          <Routes>
            {/* public */}
            <Route path="/" element={<HomePage />} />
            <Route path="/catalog" element={<Catalog />} />
            <Route path="/product/:id" element={<ProductCard />} />
            <Route path="/about" element={<AboutUs />} />

            <Route path="/order" element={<OrderPlacementPickUp />} />
            <Route path="/order-registered" element={<OrderPlacementRegistered />} />
            <Route path="/checkout" element={<CheckOut />} />

            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

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

            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </CartProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
