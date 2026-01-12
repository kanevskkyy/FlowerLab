import React, { useState, useRef, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import PopupMenu from "../popupMenu/PopupMenu";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { useCart } from "../../context/CartContext";
import "./Catalog.css";

import FilterIcon from "../../assets/images/FilterIcon.svg";
import PopupFilterMenu from "../PopupFilterMenu/PopupFilterMenu";

// Import all images
import bouquet1S from "../../assets/images/bouquet1S.jpg";
import bouquet1M from "../../assets/images/bouquet1M.jpg";
import bouquet1L from "../../assets/images/bouquet1L.jpg";
import bouquet1XL from "../../assets/images/bouquet1XL.jpg";
import bouquet2S from "../../assets/images/bouquet2S.jpg";
import bouquet2M from "../../assets/images/bouquet2M.jpg";
import bouquet2L from "../../assets/images/bouquet2L.jpg";
import bouquet2XL from "../../assets/images/bouquet2XL.jpg";
import bouquet3S from "../../assets/images/bouquet3S.jpg";
import bouquet3M from "../../assets/images/bouquet3M.JPG";
import bouquet3L from "../../assets/images/bouquet3L.jpg";
import bouquet3XL from "../../assets/images/bouquet3XL.png";
import bouquet4S from "../../assets/images/bouquet4S.jpg";
import bouquet4M from "../../assets/images/bouquet4M.JPG";
import bouquet4L from "../../assets/images/bouquet4L.JPG";
import bouquet4XL from "../../assets/images/bouquet4XL.jpg";
import bouquet5S from "../../assets/images/bouquet5S.jpg";
import bouquet5M from "../../assets/images/bouquet5M.jpg";
import bouquet5L from "../../assets/images/bouquet5L.jpg";
import bouquet5XL from "../../assets/images/bouquet5XL.jpg";
import bouquet6S from "../../assets/images/bouquet6S.jpg";
import bouquet6M from "../../assets/images/bouquet6M.png";
import bouquet6L from "../../assets/images/bouquet6L.png";
import bouquet6XL from "../../assets/images/bouquet6XL.png";
import bouquet7S from "../../assets/images/bouquet7S.jpg";
import bouquet7M from "../../assets/images/bouquet7M.jpg";
import bouquet7L from "../../assets/images/bouquet7L.png";
import bouquet7XL from "../../assets/images/bouquet7XL.png";
import bouquet8S from "../../assets/images/bouquet8S.jpg";
import bouquet8M from "../../assets/images/bouquet8M.jpg";
import bouquet8L from "../../assets/images/bouquet8L.jpg";
import bouquet8XL from "../../assets/images/bouquet8XL.jpg";
import bouquet9S from "../../assets/images/bouquet9S.jpg";
import bouquet9M from "../../assets/images/bouquet9M.jpg";
import bouquet9L from "../../assets/images/bouquet9L.png";
import bouquet9XL from "../../assets/images/bouquet9XL.jpg";
import bouquet10S from "../../assets/images/bouquet10S.jpg";
import bouquet10M from "../../assets/images/bouquet10M.jpg";
import bouquet10L from "../../assets/images/bouquet10L.jpg";
import bouquet10XL from "../../assets/images/bouquet10XL.jpg";
import bouquet11S from "../../assets/images/bouquet11S.jpg";
import bouquet11M from "../../assets/images/bouquet11M.jpg";
import bouquet11L from "../../assets/images/bouquet11L.jpg";
import bouquet11XL from "../../assets/images/bouquet11XL.jpg";
import bouquet12S from "../../assets/images/bouquet12S.jpg";
import bouquet12M from "../../assets/images/bouquet12M.png";
import bouquet12L from "../../assets/images/bouquet12L.png";
import bouquet12XL from "../../assets/images/bouquet12XL.png";
import bouquet13S from "../../assets/images/bouquet13S.jpg";
import bouquet13M from "../../assets/images/bouquet13M.jpg";
import bouquet13L from "../../assets/images/bouquet13L.jpg";
import bouquet13XL from "../../assets/images/bouquet13XL.jpg";
import bouquet14S from "../../assets/images/bouquet14S.jpg";
import bouquet14M from "../../assets/images/bouquet14M.jpg";
import bouquet14L from "../../assets/images/bouquet14L.jpg";
import bouquet14XL from "../../assets/images/bouquet14XL.jpg";
import bouquet15S from "../../assets/images/bouquet15S.jpg";
import bouquet15M from "../../assets/images/bouquet15M.jpg";
import bouquet15L from "../../assets/images/bouquet15L.jpg";
import bouquet15XL from "../../assets/images/bouquet15XL.jpg";

// Import products data
import { productsData } from "../../data/productsData";

// Map image names to imports
const imageMap = {
  bouquet1S,
  bouquet1M,
  bouquet1L,
  bouquet1XL,
  bouquet2S,
  bouquet2M,
  bouquet2L,
  bouquet2XL,
  bouquet3S,
  bouquet3M,
  bouquet3L,
  bouquet3XL,
  bouquet4S,
  bouquet4M,
  bouquet4L,
  bouquet4XL,
  bouquet5S,
  bouquet5M,
  bouquet5L,
  bouquet5XL,
  bouquet6S,
  bouquet6M,
  bouquet6L,
  bouquet6XL,
  bouquet7S,
  bouquet7M,
  bouquet7L,
  bouquet7XL,
  bouquet8S,
  bouquet8M,
  bouquet8L,
  bouquet8XL,
  bouquet9S,
  bouquet9M,
  bouquet9L,
  bouquet9XL,
  bouquet10S,
  bouquet10M,
  bouquet10L,
  bouquet10XL,
  bouquet11S,
  bouquet11M,
  bouquet11L,
  bouquet11XL,
  bouquet12S,
  bouquet12M,
  bouquet12L,
  bouquet12XL,
  bouquet13S,
  bouquet13M,
  bouquet13L,
  bouquet13XL,
  bouquet14S,
  bouquet14M,
  bouquet14L,
  bouquet14XL,
  bouquet15S,
  bouquet15M,
  bouquet15L,
  bouquet15XL,
};

const Catalog = () => {
  const navigate = useNavigate();
  const { addToCart } = useCart();

  const [sortOpen, setSortOpen] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const [filterOpen, setFilterOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [sortBy, setSortBy] = useState("popularity");
  const [currentPage, setCurrentPage] = useState(1);
  const [filters, setFilters] = useState(null);

  const ITEMS_PER_PAGE = 9;

  const sortRef = useRef(null);
  const sortButtonRef = useRef(null);

  // Закриття сортування при кліку поза межами
  useEffect(() => {
    function handleClickOutside(event) {
      if (
        sortRef.current &&
        !sortRef.current.contains(event.target) &&
        !sortButtonRef.current.contains(event.target)
      ) {
        setSortOpen(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  // Застосування фільтрів
  const applyFilters = (filterData) => {
    setFilters(filterData);
    setCurrentPage(1);
    // Скрол вгору при застосуванні нових фільтрів
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  // === ОНОВЛЕНА ЛОГІКА ФІЛЬТРАЦІЇ ===
  const getFilteredProducts = () => {
    let filtered = productsData.map((product) => ({
      ...product,
      img: imageMap[product.mainImage],
    }));

    // Пошук
    if (searchQuery) {
      const query = searchQuery.toLowerCase();
      filtered = filtered.filter(
        (p) =>
          (p.title && p.title.toLowerCase().includes(query)) ||
          (p.composition && p.composition.toLowerCase().includes(query))
      );
    }

    // Фільтри
    if (filters) {
      // Price (Max limit)
      if (filters.price) {
        filtered = filtered.filter(
          (p) => Number(p.price) <= Number(filters.price)
        );
      }

      // Size
      if (filters.size) {
        filtered = filtered.filter((p) => p.size === filters.size);
      }

      // Quantity (Min limit)
      if (filters.quantity) {
        filtered = filtered.filter(
          (p) => p.quantity >= parseInt(filters.quantity)
        );
      }

      // Events (Safe check with || [])
      if (filters.events && filters.events.length > 0) {
        filtered = filtered.filter((p) =>
          filters.events.some((event) => (p.events || []).includes(event))
        );
      }

      // For Who (Safe check with || [])
      if (filters.forWho && filters.forWho.length > 0) {
        filtered = filtered.filter((p) =>
          filters.forWho.some((who) => (p.forWho || []).includes(who))
        );
      }

      // Flower Type (Safe check with || [])
      if (filters.flowerType && filters.flowerType.length > 0) {
        filtered = filtered.filter((p) =>
          filters.flowerType.some((type) => (p.flowerType || []).includes(type))
        );
      }
    }

    return filtered;
  };

  // Сортування продуктів
  const getSortedProducts = (products) => {
    const sorted = [...products];

    switch (sortBy) {
      case "date-new":
        return sorted.sort(
          (a, b) => new Date(b.dateAdded) - new Date(a.dateAdded)
        );
      case "date-old":
        return sorted.sort(
          (a, b) => new Date(a.dateAdded) - new Date(b.dateAdded)
        );
      case "price-high":
        return sorted.sort((a, b) => b.price - a.price);
      case "price-low":
        return sorted.sort((a, b) => a.price - b.price);
      case "popularity":
      default:
        return sorted.sort((a, b) => b.popularity - a.popularity);
    }
  };

  // Отримати продукти для поточної сторінки
  const getCurrentPageProducts = () => {
    const filtered = getFilteredProducts();
    const sorted = getSortedProducts(filtered);

    const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
    const endIndex = startIndex + ITEMS_PER_PAGE;

    return {
      products: sorted.slice(startIndex, endIndex),
      totalProducts: sorted.length,
      totalPages: Math.ceil(sorted.length / ITEMS_PER_PAGE),
    };
  };

  const { products, totalProducts, totalPages } = getCurrentPageProducts();

  // Функція для додавання товару в кошик
  const handleAddToCart = (product) => {
    addToCart({ ...product, qty: 1 });
  };

  // Обробка зміни сортування
  const handleSortChange = (sortOption) => {
    setSortBy(sortOption);
    setSortOpen(false);
    setCurrentPage(1);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  // Завантажити більше (Наступна сторінка)
  const handleLoadMore = () => {
    if (currentPage < totalPages) {
      setCurrentPage((prev) => prev + 1);
      // Скрол нагору при переході на наступну сторінку
      window.scrollTo({ top: 0, behavior: "smooth" });
    }
  };

  // Перехід на конкретну сторінку
  const goToPage = (page) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  // Генерація номерів сторінок для пагінації
  const getPageNumbers = () => {
    const pages = [];

    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      if (currentPage <= 3) {
        pages.push(1, 2, 3, 4, "...", totalPages);
      } else if (currentPage >= totalPages - 2) {
        pages.push(
          1,
          "...",
          totalPages - 3,
          totalPages - 2,
          totalPages - 1,
          totalPages
        );
      } else {
        pages.push(
          1,
          "...",
          currentPage - 1,
          currentPage,
          currentPage + 1,
          "...",
          totalPages
        );
      }
    }

    return pages;
  };

  return (
    <div className="page-wrapper catalog-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="catalog">
        <h1 className="catalog-title">BOUQUETS</h1>

        {/* FILTER + SORT */}
        <div className="catalog-top">
          {/* Фільтри */}
          <div className="catalog-filter">
            <button className="filter-btn" onClick={() => setFilterOpen(true)}>
              <img src={FilterIcon} alt="Filter" className="filter-icon" />
            </button>

            <PopupFilterMenu
              isOpen={filterOpen}
              onClose={() => setFilterOpen(false)}
              onApply={applyFilters}
            />

            <div className="search-wrapper">
              <span className="search-icon"></span>
              <input
                type="text"
                placeholder="Search..."
                value={searchQuery}
                onChange={(e) => {
                  setSearchQuery(e.target.value);
                  setCurrentPage(1);
                }}
              />
            </div>
          </div>

          {/* Сортування */}
          <div
            className="catalog-sort"
            onClick={() => setSortOpen((prev) => !prev)}
            ref={sortButtonRef}>
            <span>SORT BY</span>

            {sortOpen && (
              <div className="sort-popup" ref={sortRef}>
                <p onClick={() => handleSortChange("date-new")}>
                  Date: New to old
                </p>
                <p onClick={() => handleSortChange("date-old")}>
                  Date: Old to new
                </p>
                <p onClick={() => handleSortChange("price-high")}>
                  Price: High to low
                </p>
                <p onClick={() => handleSortChange("price-low")}>
                  Price: Low to high
                </p>
                <p onClick={() => handleSortChange("popularity")}>Popularity</p>
              </div>
            )}
          </div>
        </div>

        {/* Results info */}
        <div className="catalog-info">
          <p>
            Showing {products.length} of {totalProducts} products
          </p>
        </div>

        {/* GRID — PRODUCTS */}
        <div className="catalog-grid">
          {products.length > 0 ? (
            products.map((p) => (
              <div className="catalog-item" key={p.id}>
                <div
                  className="item-img"
                  onClick={() => navigate(`/product/${p.id}`)}>
                  <img src={p.img} alt={p.title} />
                </div>

                <div className="item-bottom">
                  <div className="item-text">
                    <p>{p.title}</p>
                    <p>{p.price} ₴</p>
                  </div>

                  <button
                    className="order-btn"
                    onClick={() => handleAddToCart(p)}>
                    ORDER
                  </button>
                </div>
              </div>
            ))
          ) : (
            <div className="no-results">
              <p>No products found matching your criteria</p>
            </div>
          )}
        </div>

        {/* PAGINATION */}
        {totalPages > 1 && (
          <div className="pagination">
            {currentPage < totalPages && (
              <button className="load-more-btn" onClick={handleLoadMore}>
                LOAD MORE
              </button>
            )}

            <div className="page-numbers">
              <span
                className={currentPage === 1 ? "disabled" : ""}
                onClick={() => currentPage > 1 && goToPage(currentPage - 1)}>
                {"<"}
              </span>

              {getPageNumbers().map((page, index) => (
                <span
                  key={index}
                  className={
                    page === currentPage
                      ? "active"
                      : page === "..."
                      ? "dots"
                      : ""
                  }
                  onClick={() => typeof page === "number" && goToPage(page)}>
                  {page}
                </span>
              ))}

              <span
                className={currentPage === totalPages ? "disabled" : ""}
                onClick={() =>
                  currentPage < totalPages && goToPage(currentPage + 1)
                }>
                {">"}
              </span>
            </div>
          </div>
        )}
      </main>

      <Footer />
    </div>
  );
};

export default Catalog;
