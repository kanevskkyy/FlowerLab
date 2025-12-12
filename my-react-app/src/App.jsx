import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import AuthProvider from "./context/AuthProvider";
import { CartProvider } from "./context/CartProvider";

import ProtectedRoute from "./routes/ProtectedRoute";

import HomePage from "./pages/HomePage/HomePage";
import Catalog from "./pages/Catalog/Catalog";
import ProductCard from "./pages/ProductCard/ProductCard";
import AboutUs from "./pages/AboutUs/AboutUs";

import OrderPlacementPickUp from "./pages/OrderPlacementPickUp/OrderPlacementPickUp";
import OrderPlacementRegistered from "./pages/OrderPlacementRegistered/OrderPlacementRegistered";
import CheckOut from "./pages/CheckOut/CheckOut";
import OrderRouter from "./pages/OrderRouter/OrderRouter"; // якщо реально використовується — розкоментуй

import Login from "./pages/Login/Login";
import Register from "./pages/Register/Register";
import Cabinet from "./pages/Cabinet/Cabinet";

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <CartProvider>
          <Routes>
            {/* MAIN */}
            <Route path="/" element={<HomePage />} />
            <Route path="/catalog" element={<Catalog />} />
            <Route path="/product/:id" element={<ProductCard />} />
            <Route path="/about" element={<AboutUs />} />

            {/* ORDER / CHECKOUT */}
            <Route path="/order" element={<OrderPlacementPickUp />} />
            <Route path="/order-registered" element={<OrderPlacementRegistered />} />
            <Route path="/checkout" element={<CheckOut />} />
            <Route path="/order-router" element={<OrderRouter />} /> 

            {/* AUTH */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            {/* CABINET (PROTECTED) */}
            <Route
              path="/cabinet"
              element={
                <ProtectedRoute>
                  <Cabinet />
                </ProtectedRoute>
              }
            />

            {/* FALLBACK */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </CartProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
