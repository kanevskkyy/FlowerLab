import React, { useState, useRef, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import axiosClient from "../../api/axiosClient";
import toast from "react-hot-toast";
import PopupMenu from "../popupMenu/PopupMenu";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { useCart } from "../../context/CartContext";
import "./Catalog.css";

import FilterIcon from "../../assets/images/FilterIcon.svg";
import PopupFilterMenu from "../PopupFilterMenu/PopupFilterMenu";

// Map image names to imports (Removed - images now come from API)

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

  const [products, setProducts] = useState([]);
  const [totalProducts, setTotalProducts] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);

  const ITEMS_PER_PAGE = 9;

  const sortRef = useRef(null);
  const sortButtonRef = useRef(null);

  // Fetch bouquets from API
  useEffect(() => {
    const fetchBouquets = async () => {
      setLoading(true);
      try {
        // Radical cleanup of params to ensure NO empty strings (especially Name=) are sent
        const params = {};
        if (searchQuery && searchQuery.trim() !== "") {
          params.Name = searchQuery;
        }
        params.SortBy = sortBy;
        params.PageSize = ITEMS_PER_PAGE;
        params.Page = currentPage;

        if (filters) {
          Object.entries(filters).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== "") {
              if (Array.isArray(value)) {
                if (value.length > 0) params[key] = value;
              } else {
                params[key] = value;
              }
            }
          });
        }

        const response = await axiosClient.get("/api/catalog/bouquets", {
          params,
          headers: { Accept: "application/json" },
        });

        // Match the backend PagedResult structure if it exists, otherwise use array
        // Assuming the backend returns an object with Items, TotalCount, TotalPages
        if (response.data.items) {
          setProducts(
            response.data.items.map((p) => ({
              id: p.id,
              title: p.name,
              price: p.price,
              img: p.mainPhotoUrl,
            }))
          );
          setTotalProducts(response.data.totalCount);
          setTotalPages(response.data.totalPages);
        } else {
          // Fallback if it's just an array
          const items = Array.isArray(response.data) ? response.data : [];
          setProducts(
            items.map((p) => ({
              id: p.id,
              title: p.name,
              price: p.price,
              img: p.mainPhotoUrl,
            }))
          );
          setTotalProducts(items.length);
          setTotalPages(1);
        }
      } catch (error) {
        console.error("Failed to fetch bouquets:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchBouquets();
  }, [filters, sortBy, currentPage, searchQuery]);

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
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

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
                <p onClick={() => handleSortChange("date_desc")}>
                  Date: New to old
                </p>
                <p onClick={() => handleSortChange("date_asc")}>
                  Date: Old to new
                </p>
                <p onClick={() => handleSortChange("price_desc")}>
                  Price: High to low
                </p>
                <p onClick={() => handleSortChange("price_asc")}>
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
