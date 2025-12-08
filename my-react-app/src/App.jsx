import { BrowserRouter, Routes, Route } from "react-router-dom";
import Catalog from "./pages/Catalog/Catalog";
import ProductCard from "./pages/ProductCard/ProductCard";
import AboutUs from "./pages/AboutUs/AboutUs";
import OrderRouter from "./pages/OrderRouter/OrderRouter";
import OrderPlacementPickUp from "./pages/OrderPlacementPickUp/OrderPlacementPickUp";
function App() {

  const currentUser = {
    registered: true,      // чи користувач увійшов у систему
    selfReceiver: false,   // чи він сам буде отримувачем
  };
  return (
    
    <BrowserRouter>
      <Routes>

        {/* Каталог */}
        <Route path="/catalog" element={<Catalog />} />

        {/* Сторінка товару з ID */}
        <Route path="/product/:id" element={<ProductCard />} />

        {/* Початкова сторінка → каталог */}
        <Route path="*" element={<Catalog />} />

        <Route path="/about" element={<AboutUs />} />
        
        <Route path="/order" element={<OrderPlacementPickUp />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
