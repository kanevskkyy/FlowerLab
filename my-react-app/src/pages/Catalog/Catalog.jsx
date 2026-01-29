import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
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
  const { t } = useTranslation();
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
        title={t("catalog.seo_title")}
        description={t("catalog.seo_desc")}
        image="/og-catalog.jpg"
      />
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      <main className="catalog">
        <h1 className="catalog-title">{t("catalog.title")}</h1>

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
                placeholder={t("catalog.search")}
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
            {t("catalog.showing", {
              count: products.length,
              total: totalProducts,
            })}
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
