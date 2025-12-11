import { BrowserRouter, Routes, Route } from "react-router-dom";
import Catalog from "./pages/Catalog/Catalog";
import ProductCard from "./pages/ProductCard/ProductCard";
import AboutUs from "./pages/AboutUs/AboutUs";
import OrderRouter from "./pages/OrderRouter/OrderRouter";
import OrderPlacementPickUp from "./pages/OrderPlacementPickUp/OrderPlacementPickUp";
import HomePage from "./pages/HomePage/HomePage";
import Cabinet from "./pages/Cabinet/Cabinet";
import { CartProvider } from "./context/CartProvider";

function App() {
  return (

    <CartProvider>
      <BrowserRouter>
      <Routes>

        {/* Головна сторінка */}
        <Route path="/" element={<HomePage />} />

        {/* Каталог */}
        <Route path="/catalog" element={<Catalog />} />

        {/* Сторінка товару */}
        <Route path="/product/:id" element={<ProductCard />} />

        {/* About */}
        <Route path="/about" element={<AboutUs />} />

        {/* Order */}
        <Route path="/order" element={<OrderPlacementPickUp />} />

        {/* Fallback (все інше → HomePage або Catalog, як хочеш) */}
        <Route path="*" element={<HomePage />} />
        {/* Cabinet */}
        <Route path="/cabinet" element={<Cabinet />} />

      </Routes>
    </BrowserRouter>
    </CartProvider>
  );
}

export default App;
