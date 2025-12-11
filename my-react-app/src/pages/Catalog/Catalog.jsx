import React, { useState, useRef, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import PopupMenu from "../popupMenu/PopupMenu";
import Header from "../../components/Header/Header";    
import { useCart } from "../../context/CartContext";    
import "./Catalog.css";

import FilterIcon from "../../assets/images/FilterIcon.svg";
import PopupFilterMenu from "../PopupFilterMenu/PopupFilterMenu";

import bouquet1 from "../../assets/images/bouquet1.JPG";
import bouquet2 from "../../assets/images/bouquet2.JPG";
import bouquet3 from "../../assets/images/bouquet3.JPG";
import img4 from "../../assets/images/testphoto.jpg";
import img5 from "../../assets/images/testphoto.jpg";
import img6 from "../../assets/images/testphoto.jpg";
import img7 from "../../assets/images/testphoto.jpg";
import img8 from "../../assets/images/testphoto.jpg";
import img9 from "../../assets/images/testphoto.jpg";

const Catalog = () => {
  const navigate = useNavigate();
  const { addToCart } = useCart();
  const [sortOpen, setSortOpen] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const [filterOpen, setFilterOpen] = useState(false);

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

  const products = [
    { id: 1, title: "bouquet 1", price: "1000 ₴", img: bouquet1 },
    { id: 2, title: "bouquet 1", price: "1000 ₴", img: bouquet2 },
    { id: 3, title: "bouquet 1", price: "1000 ₴", img: bouquet3 },
    { id: 4, title: "bouquet 1", price: "1000 ₴", img: img4 },
    { id: 5, title: "bouquet 1", price: "1000 ₴", img: img5 },
    { id: 6, title: "bouquet 1", price: "1000 ₴", img: img6 },
    { id: 7, title: "bouquet 1", price: "1000 ₴", img: img7 },
    { id: 8, title: "bouquet 1", price: "1000 ₴", img: img8 },
    { id: 9, title: "bouquet 1", price: "1000 ₴", img: img9 },
  ];

  return (
    <div className="page-wrapper catalog-page">

      {/* ✅ Глобальний хедер */}
      <Header onMenuOpen={() => setMenuOpen(true)} />

      {/* POPUP MENU */}
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />

      {/* ================= MAIN ================= */}
      <main className="catalog">
        <h1 className="catalog-title">BOUQUETS</h1>

        {/* FILTER + SORT */}
        <div className="catalog-top">
          
          {/* Фільтри */}
          <div className="catalog-filter">
            <button className="icon-btn" onClick={() => setFilterOpen(true)}>
              <img src={FilterIcon} alt="Filter" className="icon" />
            </button>

            <PopupFilterMenu
              isOpen={filterOpen}
              onClose={() => setFilterOpen(false)}
              onApply={(filters) => console.log("APPLIED FILTERS:", filters)}
            />

            <div className="search-wrapper">
              <span className="search-icon"></span>
              <input type="text" placeholder="Search..." />
            </div>
          </div>

          {/* Сортування */}
          <div
            className="catalog-sort"
            onClick={() => setSortOpen((prev) => !prev)}
            ref={sortButtonRef}
          >
            <span>SORT BY</span>

            {sortOpen && (
              <div className="sort-popup" ref={sortRef}>
                <p>Date: New to old</p>
                <p>Date: Old to new</p>
                <p>Price: High to low</p>
                <p>Price: Low to high</p>
                <p>Popularity</p>
              </div>
            )}
          </div>
        </div>

        {/* GRID — PRODUCTS */}
        <div className="catalog-grid">
          {products.map((p) => (
            <div className="catalog-item" key={p.id}>
              
              <div
                className="item-img"
                onClick={() => navigate(`/product/${p.id}`)}
              >
                <img src={p.img} alt={p.title} />
              </div>

              <div className="item-bottom">
                <div className="item-text">
                  <p>{p.title}</p>
                  <p>{p.price}</p>
                </div>

                {/* ORDER → додає в глобальний кошик */}
                <button className="order-btn" onClick={() => addToCart(p)}>
                  ORDER
                </button>
              </div>

            </div>
          ))}
        </div>

        {/* PAGINATION */}
        <div className="pagination">
          <button className="load-more-btn">LOAD MORE</button>

          <div className="page-numbers">
            <span>{"<"}</span>
            <span>1</span>
            <span>2</span>
            <span>3</span>
            <span>…</span>
            <span>7</span>
          </div>
        </div>
      </main>

      {/* FOOTER */}
      <footer className="footer">
        <div className="footer-col">
          <p>м. Київ, вул. Прикладна 7а</p>
          <p>Пн — Пт: 9:00 — 21:00</p>
        </div>

        <div className="footer-col">
          <p>+38 050 555 55 12</p>
          <p>info@example.com</p>
        </div>

        <div className="footer-col">
          <p>@florist_shop</p>
        </div>
      </footer>

    </div>
  );
};

export default Catalog;
