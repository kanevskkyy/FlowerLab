import React, { useState, useEffect } from "react";
import toast from "react-hot-toast";
import { useTranslation } from "react-i18next";
import { useCart } from "../../context/CartContext";
import { useGifts } from "./hooks/useGifts";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import PopupMenu from "../../components/PopupMenu/PopupMenu";
import CatalogPagination from "../Catalog/components/CatalogPagination";
import ProductSkeleton from "../../components/ProductSkeleton/ProductSkeleton";
import "./Gifts.css"; // Локальні стилі (копія каталогу)
import SEO from "../../components/SEO/SEO";

const Gifts = () => {
  const { t } = useTranslation();
  const { addToCart } = useCart();
  const {
    gifts,
    loading,
    error,
    currentPage,
    totalPages,
    handlePageChange,
    handleLoadMore,
  } = useGifts();
  const [menuOpen, setMenuOpen] = useState(false);

  const handleAddToCart = (item) => {
    addToCart({
      id: item.id,
      title: item.name, // API returns 'name'
      price: `${item.price} ₴`, // API returns number
      img: item.imageUrl, // API returns 'imageUrl'
      qty: 1,
      maxStock: item.availableCount,
      isGift: true,
    });
    toast.success(t("gifts.added", { name: item.name }));
  };

  const optimizeCloudinaryUrl = (url) => {
    if (!url || !url.includes("cloudinary.com")) return url;
    if (url.includes("/w_")) return url;
    const parts = url.split("upload/");
    if (parts.length < 2) return url;
    return `${parts[0]}upload/w_400,h_400,c_fill,q_auto,f_auto/${parts[1]}`;
  };

  return (
    <div className="page-wrapper gifts-page">
      <SEO
        title={t("gifts.seo_title")}
        description={t("gifts.seo_desc")}
        image="/og-gifts.jpg"
      />
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="catalog">
        <h1 className="catalog-title">{t("gifts.title")}</h1>

        {loading ? (
          <div className="catalog-grid">
            <ProductSkeleton count={6} />
          </div>
        ) : error ? (
          <div
            style={{
              textAlign: "center",
              padding: "4rem",
              color: "#666",
              fontSize: "1.1rem",
            }}>
            {t("gifts.error")}
          </div>
        ) : (
          <>
            <div className="catalog-grid">
              {gifts.length > 0 ? (
                gifts.map((item) => (
                  <div
                    className={`catalog-item ${item.availableCount === 0 ? "out-of-stock" : ""}`}
                    key={item.id}>
                    {/* PHOTO */}
                    <div className="item-img">
                      <img
                        src={
                          optimizeCloudinaryUrl(item.imageUrl) ||
                          "/placeholder.png"
                        }
                        alt={item.name}
                        loading="lazy"
                      />

                      {/* STOCK BADGES */}
                      {item.availableCount === 0 ? (
                        <div className="stock-badge oos">{t("gifts.oos")}</div>
                      ) : item.availableCount < 5 ? (
                        <div className="stock-badge low">
                          {t("gifts.only_left", { count: item.availableCount })}
                        </div>
                      ) : null}
                    </div>

                    {/* BOTTOM */}
                    <div className="item-bottom">
                      <div className="item-text">
                        <p>{item.name}</p>
                        <p>{item.price} ₴</p>
                      </div>

                      <button
                        className="order-btn"
                        onClick={() => handleAddToCart(item)}
                        disabled={item.availableCount === 0}>
                        {item.availableCount === 0
                          ? t("gifts.oos")
                          : t("gifts.order")}
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
                  {t("gifts.no_gifts")}
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
