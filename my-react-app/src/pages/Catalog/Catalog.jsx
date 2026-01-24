import { useNavigate } from "react-router-dom";
import PopupMenu from "../../components/PopupMenu/PopupMenu";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import "./Catalog.css";

import FilterIcon from "../../assets/icons/FilterIcon.svg";
import PopupFilterMenu from "../../components/PopupFilterMenu/PopupFilterMenu";

// Sub-components
import CatalogSort from "./components/CatalogSort";
import CatalogGrid from "./components/CatalogGrid";
import CatalogPagination from "./components/CatalogPagination";

// Hooks
import { useCatalog } from "./hooks/useCatalog";

import SEO from "../../components/SEO/SEO";

const Catalog = () => {
  const navigate = useNavigate();

  const {
    products,
    totalProducts,
    totalPages,
    currentPage,
    loading,
    sortOpen,
    menuOpen,
    filterOpen,
    searchQuery,
    filters,

    setSortOpen,
    setMenuOpen,
    setFilterOpen,
    setSearchQuery,
    setCurrentPage,

    applyFilters,
    handleSortChange,
    handleLoadMore,
    goToPage,
    // handleAddToCart, // passed to grid if needed, or grid can use context/navigate
  } = useCatalog();

  return (
    <div className="page-wrapper catalog-page">
      <SEO
        title="Catalog | FlowerLab"
        description="Browse our wide selection of fresh bouquets and floral arrangements. Filter by price, size, and occasion."
        image="/og-catalog.jpg"
      />
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="catalog">
        <h1 className="catalog-title">BOUQUETS</h1>

        {/* FILTER + SORT */}
        <div className="catalog-top">
          {/* Filters */}
          <div className="catalog-filter">
            <button className="filter-btn" onClick={() => setFilterOpen(true)}>
              <img src={FilterIcon} alt="Filter" className="filter-icon" />
            </button>

            <PopupFilterMenu
              isOpen={filterOpen}
              onClose={() => setFilterOpen(false)}
              onApply={applyFilters}
              currentFilters={filters}
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

          {/* Sort */}
          <CatalogSort
            isOpen={sortOpen}
            setIsOpen={setSortOpen}
            onSortChange={handleSortChange}
          />
        </div>

        {/* Results info */}
        <div className="catalog-info">
          <p>
            Showing {products.length} of {totalProducts} products
          </p>
        </div>

        {/* GRID — PRODUCTS */}
        <CatalogGrid
          products={products}
          loading={loading}
          onOrder={(p) => navigate(`/product/${p.id}`)}
        />

        {/* PAGINATION */}
        <CatalogPagination
          currentPage={currentPage}
          totalPages={totalPages}
          onPageChange={goToPage}
          onLoadMore={handleLoadMore}
        />
      </main>

      <Footer />
    </div>
  );
};

export default Catalog;
