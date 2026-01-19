import React, { useRef, useEffect, useState } from "react";

function AdminOrdersSort({ sort, onSortChange }) {
  const [isOpen, setIsOpen] = useState(false);
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

  const handleSelect = (value) => {
    onSortChange(value);
    setIsOpen(false);
  };

  return (
    <div
      className="admin-orders-sort-custom"
      onClick={() => setIsOpen((prev) => !prev)}
      ref={sortButtonRef}
    >
      <span>SORT BY</span>

      {isOpen && (
        <div className="admin-sort-popup" ref={sortRef}>
          <p onClick={() => handleSelect("date-desc")} className={sort === "date-desc" || sort === "new" ? "active" : ""}>Date: New to old</p>
          <p onClick={() => handleSelect("date-asc")} className={sort === "date-asc" || sort === "old" ? "active" : ""}>Date: Old to new</p>
          <p onClick={() => handleSelect("qty-desc")} className={sort === "qty-desc" ? "active" : ""}>Quantity: High to Low</p>
          <p onClick={() => handleSelect("qty-asc")} className={sort === "qty-asc" ? "active" : ""}>Quantity: Low to High</p>
          <p onClick={() => handleSelect("name-asc")} className={sort === "name-asc" ? "active" : ""}>Name: A to Z</p>
          <p onClick={() => handleSelect("name-desc")} className={sort === "name-desc" ? "active" : ""}>Name: Z to A</p>
        </div>
      )}
    </div>
  );
}

export default AdminOrdersSort;
