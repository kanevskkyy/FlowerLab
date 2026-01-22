import React, { useState, useEffect } from "react";
import toast from "react-hot-toast";
import { useCart } from "../../context/CartContext";
import { useGifts } from "./hooks/useGifts";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../popupMenu/PopupMenu";
import CatalogPagination from "../Catalog/components/CatalogPagination";
import ProductSkeleton from "../../components/ProductSkeleton/ProductSkeleton";
import "./Gifts.css"; // Локальні стилі (копія каталогу)

const Gifts = () => {
  const { addToCart } = useCart();
  const {
    gifts,
    loading,
    currentPage,
    totalPages,
    handlePageChange,
    handleLoadMore,
  } = useGifts();
  const [menuOpen, setMenuOpen] = useState(false);

  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  const handleAddToCart = (item) => {
    addToCart({
      id: item.id,
      title: item.name, // API returns 'name'
      price: `${item.price} ₴`, // API returns number
      img: item.imageUrl, // API returns 'imageUrl'
      qty: 1,
      isGift: true,
    });
    toast.success(`${item.name} added to cart!`);
  };

  return (
    <div className="page-wrapper gifts-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="catalog">
        <h1 className="catalog-title">GIFTS & EXTRAS</h1>

        {loading ? (
          <div className="catalog-grid">
            <ProductSkeleton count={6} />
          </div>
        ) : (
          <>
            <div className="catalog-grid">
              {gifts.length > 0 ? (
                gifts.map((item) => (
                  <div className="catalog-item" key={item.id}>
                    {/* PHOTO */}
                    <div className="item-img">
                      <img
                        src={item.imageUrl || "/placeholder.png"}
                        alt={item.name}
                      />
                    </div>

                    {/* BOTTOM */}
                    <div className="item-bottom">
                      <div className="item-text">
                        <p>{item.name}</p>
                        <p>{item.price} ₴</p>
                      </div>

                      <button
                        className="order-btn"
                        onClick={() => handleAddToCart(item)}>
                        ORDER
                      </button>
                    </div>
                  </div>
                ))
              ) : (
                <div
                  style={{
                    gridColumn: "1/-1",
                    textAlign: "center",
                    padding: "2rem",
                    color: "#999",
                  }}>
                  No gifts found.
                </div>
              )}
            </div>

            {/* PAGINATION */}
            {gifts.length > 0 && (
              <CatalogPagination
                currentPage={currentPage}
                totalPages={totalPages}
                onPageChange={handlePageChange}
                onLoadMore={handleLoadMore}
              />
            )}
          </>
        )}
      </main>

      <Footer />
    </div>
  );
};

export default Gifts;
