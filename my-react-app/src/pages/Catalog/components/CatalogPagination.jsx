import React from "react";
import { useTranslation } from "react-i18next";

function CatalogPagination({
  currentPage,
  totalPages,
  onPageChange,
  onLoadMore,
}) {
  const { t } = useTranslation();
  if (totalPages <= 1) return null;

  // Generate page numbers
  const getPageNumbers = () => {
    const pages = [];
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
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
          totalPages,
        );
      } else {
        pages.push(
          1,
          "...",
          currentPage - 1,
          currentPage,
          currentPage + 1,
          "...",
          totalPages,
        );
      }
    }
    return pages;
  };

  return (
    <div className="pagination">
      {currentPage < totalPages && (
        <button className="load-more-btn" onClick={onLoadMore}>
          {t("catalog.load_more")}
        </button>
      )}

      <div className="page-numbers">
        <span
          className={currentPage === 1 ? "disabled" : ""}
          onClick={() => currentPage > 1 && onPageChange(currentPage - 1)}>
          {"<"}
        </span>

        {getPageNumbers().map((page, index) => (
          <span
            key={index}
            className={
              page === currentPage ? "active" : page === "..." ? "dots" : ""
            }
            onClick={() => typeof page === "number" && onPageChange(page)}>
            {page}
          </span>
        ))}

        <span
          className={currentPage === totalPages ? "disabled" : ""}
          onClick={() =>
            currentPage < totalPages && onPageChange(currentPage + 1)
          }>
          {">"}
        </span>
      </div>
    </div>
  );
}

export default CatalogPagination;
