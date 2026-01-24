import React, { useEffect, useRef } from "react";
import searchIco from "../../../assets/icons/search.svg";
import editIco from "../../../assets/icons/edit.svg";
import trashIco from "../../../assets/icons/trash.svg";

function AdminProductList({
  active,
  title,
  products,
  q,
  setQ,
  onAdd,
  onEdit,
  onDelete,
  loadMore,
  hasNextPage,
  isLoadingMore,
}) {
  const sentinelRef = useRef(null);

  useEffect(() => {
    if (!hasNextPage) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          loadMore();
        }
      },
      { rootMargin: "100px" },
    );

    if (sentinelRef.current) {
      observer.observe(sentinelRef.current);
    }

    return () => {
      if (sentinelRef.current) {
        observer.unobserve(sentinelRef.current);
      }
    };
  }, [loadMore, hasNextPage]);

  const splitTitle = (t) => {
    if (!t) return { a: "", b: "" };
    const parts = t.trim().split(" ");
    if (parts.length <= 1) return { a: t, b: "" };
    return { a: parts[0], b: parts.slice(1).join(" ") };
  };

  return (
    <section className="admin-section">
      <h2 className="admin-section-title">{title} management</h2>
      <div className="admin-toolbar">
        <div className="admin-search">
          <img className="admin-search-ico" src={searchIco} alt="" />
          <input
            value={q}
            onChange={(e) => setQ(e.target.value)}
            placeholder="Search by name"
          />
        </div>

        <button className="admin-add-btn" type="button" onClick={onAdd}>
          Add {title === "Bouquets" ? "bouquet" : "gift"}{" "}
          <span className="admin-plus">+</span>
        </button>
      </div>

      <div className="admin-grid">
        {products.map((p) => {
          const tt = splitTitle(p.title);
          return (
            <div key={p.id} className="admin-card">
              <div className="admin-card-img">
                <img
                  src={p.img}
                  alt={p.title}
                  draggable="false"
                  loading="lazy"
                />
                {p.stock !== undefined && (
                  <div
                    className={`admin-stock-badge ${p.stock === 0 ? "oos" : p.stock < 5 ? "low" : ""}`}>
                    Stock: {p.stock}
                  </div>
                )}
              </div>
              <div className="admin-card-bottom">
                <div className="admin-card-title">
                  <span className="t1">{tt.a}</span>
                  {tt.b ? <span className="t2">{tt.b}</span> : null}
                </div>
                <div className="admin-card-actions">
                  <button
                    className="admin-mini-btn"
                    type="button"
                    onClick={() => onEdit(p)}>
                    <img src={editIco} alt="Edit" />
                  </button>

                  <button
                    className="admin-mini-btn"
                    type="button"
                    onClick={() => onDelete(p.id)}>
                    <img src={trashIco} alt="Delete" />
                  </button>
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Sentinel for Infinite Scroll */}
      {hasNextPage && (
        <div
          ref={sentinelRef}
          style={{
            height: "20px",
            textAlign: "center",
            color: "#666",
            marginTop: "20px",
            width: "100%",
          }}>
          {isLoadingMore ? "Loading more..." : ""}
        </div>
      )}
    </section>
  );
}

export default AdminProductList;
