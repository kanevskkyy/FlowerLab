import React, { useRef, useEffect } from "react";

function CatalogSort({ isOpen, setIsOpen, onSortChange }) {
  const sortRef = useRef(null);
  const sortButtonRef = useRef(null);

  // Close sort on outside click
  useEffect(() => {
    function handleClickOutside(event) {
      if (
        sortRef.current &&
        !sortRef.current.contains(event.target) &&
        !sortButtonRef.current.contains(event.target)
      ) {
        setIsOpen(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [setIsOpen]);

  return (
    <div
      className="catalog-sort"
      onClick={() => setIsOpen((prev) => !prev)}
      ref={sortButtonRef}
    >
      <span>SORT BY</span>

      {isOpen && (
        <div className="sort-popup" ref={sortRef}>
          <p onClick={() => onSortChange("date_desc")}>Date: New to old</p>
          <p onClick={() => onSortChange("date_asc")}>Date: Old to new</p>
          <p onClick={() => onSortChange("price_desc")}>Price: High to low</p>
          <p onClick={() => onSortChange("price_asc")}>Price: Low to high</p>
          <p onClick={() => onSortChange("popularity")}>Popularity</p>
        </div>
      )}
    </div>
  );
}

export default CatalogSort;
