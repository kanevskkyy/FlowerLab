import React, { useState, useEffect } from "react";
import toast from "react-hot-toast";
import { useCart } from "../../context/CartContext";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";
import "./Gifts.css"; // Локальні стилі (копія каталогу)

// Імпорт зображень
import gift1 from "../../assets/images/gift1.jpg";
import gift2 from "../../assets/images/gift2.jpg";
import gift3 from "../../assets/images/gift3.png";

const Gifts = () => {
  const { addToCart } = useCart();
  const [menuOpen, setMenuOpen] = useState(false);

  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  const giftsItems = [
    { id: 101, title: "Teddy Bear", price: "1000 ₴", img: gift1 },
    { id: 102, title: "Star Balloon", price: "250 ₴", img: gift2 },
    { id: 103, title: "Box of Chocolates", price: "500 ₴", img: gift3 },
    { id: 104, title: "Big Teddy XL", price: "1500 ₴", img: gift1 },
    { id: 105, title: "Heart Balloon Set", price: "450 ₴", img: gift2 },
    { id: 106, title: "Premium Sweets Box", price: "800 ₴", img: gift3 },
  ];

  const handleAddToCart = (item) => {
    addToCart({
      id: item.id,
      title: item.title,
      price: item.price, // Переконайтеся, що тут строка з валютою або число
      img: item.img,
      qty: 1,
      isGift: true,
    });
    toast.success(`${item.title} added to cart!`);
  };

  return (
    <div className="page-wrapper gifts-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="catalog">
        <h1 className="catalog-title">GIFTS & EXTRAS</h1>

        {/* СІТКА (Структура 1 в 1 як у Catalog.jsx) */}
        <div className="catalog-grid">
          {giftsItems.map((item) => (
            <div className="catalog-item" key={item.id}>
              {/* ФОТО */}
              <div className="item-img">
                <img src={item.img} alt={item.title} />
              </div>

              {/* НИЖНЯ ЧАСТИНА */}
              <div className="item-bottom">
                <div className="item-text">
                  <p>{item.title}</p>
                  <p>{item.price}</p>
                </div>

                <button
                  className="order-btn"
                  onClick={() => handleAddToCart(item)}>
                  ORDER
                </button>
              </div>
            </div>
          ))}
        </div>
      </main>

      <Footer />
    </div>
  );
};

export default Gifts;
