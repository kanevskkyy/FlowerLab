import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/useAuth";
import { useMemo, useState, useEffect } from "react";
import toast from "react-hot-toast";
import "./AdminPanel.css"; // ✅ Стилі підвантажуються звідси

import bouquetsIco from "../../assets/icons/flowerr.svg";
import ordersIco from "../../assets/icons/orders.svg";
import reviewsIco from "../../assets/icons/review.svg";
import bellIco from "../../assets/icons/bell.svg";
import logoutIco from "../../assets/icons/exit.svg";
import searchIco from "../../assets/icons/search.svg";
import editIco from "../../assets/icons/edit.svg";
import trashIco from "../../assets/icons/trash.svg";

// Images
import testphoto from "../../assets/images/testphoto.jpg";
import bouquet1L from "../../assets/images/bouquet1L.jpg";
import bouquet2L from "../../assets/images/bouquet2L.jpg";
import bouquet3L from "../../assets/images/bouquet3L.jpg";
import gift1 from "../../assets/images/gift1.jpg";
import gift2 from "../../assets/images/gift2.jpg";

// Залишаємо тут тільки ті пункти, які НЕ в каталозі
const NAV = [
  { key: "orders", label: "Orders", icon: ordersIco },
  { key: "reviews", label: "Reviews", icon: reviewsIco },
];

const ORDER_STATUSES = [
  "New",
  "Processing",
  "Shipped",
  "Delivered",
  "Cancelled",
];

export default function AdminPanel() {
  const navigate = useNavigate();
  const { logout } = useAuth();

  // --- STATE ---
  const [active, setActive] = useState(() => {
    return localStorage.getItem("adminActiveTab") || "bouquets";
  });

  const [isCatalogOpen, setIsCatalogOpen] = useState(() => {
    // ✅ Логіка відкриття вже тут, тому useEffect не потрібен для цього
    const current = localStorage.getItem("adminActiveTab");
    return (
      current === "bouquets" || current === "gifts" || current === "catalog"
    );
  });

  const [q, setQ] = useState("");

  useEffect(() => {
    localStorage.setItem("adminActiveTab", active);
  }, [active]);

  const handleSignOut = () => {
    logout();
    toast.success("Successfully logged out");
    navigate("/login", { replace: true });
    localStorage.removeItem("adminActiveTab");
  };

  // ========= PRODUCTS (Bouquets + Gifts) =========
  const [products, setProducts] = useState([
    {
      id: 1,
      title: "Bouquet Roses",
      img: testphoto,
      price: "1000 ₴",
      category: "Bouquets",
    },
    {
      id: 2,
      title: "Bouquet Peonies",
      img: bouquet2L,
      price: "1200 ₴",
      category: "Bouquets",
    },
    {
      id: 3,
      title: "Bouquet Hydrangea",
      img: bouquet3L,
      price: "900 ₴",
      category: "Bouquets",
    },
    {
      id: 4,
      title: "Bouquet Orchids",
      img: bouquet1L,
      price: "1500 ₴",
      category: "Bouquets",
    },
    {
      id: 5,
      title: "Bouquet Ranunculus",
      img: bouquet2L,
      price: "1100 ₴",
      category: "Bouquets",
    },
    {
      id: 6,
      title: "Bouquet Daisies",
      img: bouquet3L,
      price: "800 ₴",
      category: "Bouquets",
    },
    // Подарунки
    {
      id: 101,
      title: "Teddy Bear",
      img: gift1 || testphoto,
      price: "850 ₴",
      category: "Gifts",
    },
    {
      id: 102,
      title: "Star Balloon",
      img: gift2 || testphoto,
      price: "250 ₴",
      category: "Gifts",
    },
  ]);

  const handleDeleteProduct = (id) => {
    if (window.confirm("Are you sure you want to delete this item?")) {
      setProducts((prev) => prev.filter((b) => b.id !== id));
      toast.success("Item deleted successfully");
    }
  };

  const filteredProducts = useMemo(() => {
    let data = products;

    if (active === "bouquets") {
      data = products.filter((p) => p.category === "Bouquets");
    } else if (active === "gifts") {
      data = products.filter((p) => p.category === "Gifts");
    }

    const s = q.trim().toLowerCase();
    if (!s) return data;
    return data.filter((b) => b.title.toLowerCase().includes(s));
  }, [q, products, active]);

  const splitTitle = (t) => {
    if (!t) return { a: "", b: "" };
    const parts = t.trim().split(" ");
    if (parts.length <= 1) return { a: t, b: "" };
    return { a: parts[0], b: parts.slice(1).join(" ") };
  };

  // ========= ORDERS =========
  const [orders, setOrders] = useState([
    {
      id: 1001,
      title: "Bouquet Orchids",
      qty: "1 pc",
      customer: "Oleh Vynnyk",
      date: "25.10.25 10:06",
      total: "1000 ₴",
      avatar: testphoto,
      status: "New",
    },
    {
      id: 1002,
      title: "Bouquet 101 Roses",
      qty: "1 pc",
      customer: "Tina Karol",
      date: "24.10.25 14:30",
      total: "5500 ₴",
      avatar: bouquet1L,
      status: "Processing",
    },
  ]);

  const [sort, setSort] = useState("new");

  const handleStatusChange = (id, newStatus) => {
    setOrders((prev) =>
      prev.map((order) =>
        order.id === id ? { ...order, status: newStatus } : order
      )
    );
    if (newStatus === "Cancelled") {
      toast.error(`Order #${id} marked as Cancelled`);
    } else if (newStatus === "Delivered") {
      toast.success(`Order #${id} delivered! 🎉`);
    } else {
      toast.success(`Order #${id} is now ${newStatus}`);
    }
  };

  const sortedOrders = useMemo(() => {
    const sorted = [...orders];
    if (sort === "old") {
      return sorted.reverse();
    }
    return sorted;
  }, [orders, sort]);

  // ========= CATALOG SETTINGS =========
  const catalogSettings = useMemo(
    () => ({
      events: ["Birthday", "Wedding", "Engagement"],
      forWho: ["Mom", "Wife", "Husband", "Kid", "Teacher", "Co-worker"],
      flowerTypes: ["Peony", "Rose", "Lily", "Tulip", "Orchid", "Hydrangea"],
    }),
    []
  );

  // ========= REVIEWS =========
  const [pendingReviews, setPendingReviews] = useState([
    {
      id: 1,
      name: "Anna Shevchenko",
      stars: 5,
      text: "Such a pretty bouquet! Will buy again ^^",
      avatar: testphoto,
    },
  ]);

  const handlePostReview = (id) => {
    setPendingReviews((prev) => prev.filter((r) => r.id !== id));
    toast.success("Review posted successfully!");
  };

  const handleDeleteReview = (id) => {
    setPendingReviews((prev) => prev.filter((r) => r.id !== id));
    toast.success("Review deleted");
  };

  const Stars = ({ value }) => {
    return (
      <div className="review-stars">
        {Array.from({ length: 5 }).map((_, i) => (
          <span key={i} className={`star ${i < value ? "on" : "off"}`}>
            ★
          </span>
        ))}
      </div>
    );
  };

  return (
    <div className="admin-root">
      {/* HEADER */}
      <header className="admin-topbar">
        <div className="admin-brand">
          <div className="admin-brand-top">FLOWER LAB</div>
          <div className="admin-brand-sub">VLADA</div>
        </div>

        <div className="admin-topbar-center">Admin panel</div>

        <div className="admin-topbar-right">
          <button className="admin-top-ico-btn" type="button">
            <img src={bellIco} alt="Notifications" />
          </button>
          <button
            className="admin-top-logout"
            type="button"
            onClick={handleSignOut}>
            <img src={logoutIco} alt="Logout" />
            <span>Log out</span>
          </button>
        </div>
      </header>

      <div className="admin-body">
        {/* SIDEBAR */}
        <aside className="admin-side">
          <nav className="admin-nav">
            {/* CATALOG DROPDOWN */}
            <div className="nav-group">
              <button
                className={`admin-nav-item ${
                  active === "bouquets" ||
                  active === "gifts" ||
                  active === "catalog"
                    ? "active-parent"
                    : ""
                }`}
                onClick={() => setIsCatalogOpen(!isCatalogOpen)}>
                <img className="admin-nav-ico" src={bouquetsIco} alt="" />
                <span style={{ flex: 1 }}>CATALOG</span>
                {/* Стрілочка (клас у CSS) */}
                <span className={`nav-arrow ${isCatalogOpen ? "open" : ""}`}>
                  ›
                </span>
              </button>

              {isCatalogOpen && (
                <div className="admin-submenu">
                  <button
                    className={`admin-sub-item ${
                      active === "bouquets" ? "active" : ""
                    }`}
                    onClick={() => {
                      setActive("bouquets");
                      setQ("");
                    }}>
                    Bouquets
                  </button>
                  <button
                    className={`admin-sub-item ${
                      active === "gifts" ? "active" : ""
                    }`}
                    onClick={() => {
                      setActive("gifts");
                      setQ("");
                    }}>
                    Gifts
                  </button>
                  {/* 👇 Повернув назву "Catalog" */}
                  <button
                    className={`admin-sub-item ${
                      active === "catalog" ? "active" : ""
                    }`}
                    onClick={() => {
                      setActive("catalog");
                      setQ("");
                    }}>
                    Catalog Settings
                  </button>
                </div>
              )}
            </div>

            {/* OTHER ITEMS */}
            {NAV.map((item) => (
              <button
                key={item.key}
                type="button"
                className={`admin-nav-item ${
                  active === item.key ? "active" : ""
                }`}
                onClick={() => setActive(item.key)}>
                <img className="admin-nav-ico" src={item.icon} alt="" />
                <span>{item.label}</span>
              </button>
            ))}
          </nav>
        </aside>

        {/* CONTENT */}
        <main className="admin-content">
          {/* ========== BOUQUETS & GIFTS MANAGEMENT ========== */}
          {(active === "bouquets" || active === "gifts") && (
            <section className="admin-section">
              <h2 className="admin-section-title">
                {active === "bouquets"
                  ? "Bouquets management"
                  : "Gifts management"}
              </h2>
              <div className="admin-toolbar">
                <div className="admin-search">
                  <img className="admin-search-ico" src={searchIco} alt="" />
                  <input
                    value={q}
                    onChange={(e) => setQ(e.target.value)}
                    placeholder="Search by name"
                  />
                </div>

                <button
                  className="admin-add-btn"
                  type="button"
                  onClick={() => {
                    if (active === "gifts") navigate("/admin/gifts/new");
                    else navigate("/admin/bouquets/new");
                  }}>
                  Add {active === "bouquets" ? "bouquet" : "gift"}{" "}
                  <span className="admin-plus">+</span>
                </button>
              </div>

              <div className="admin-grid">
                {filteredProducts.map((p) => {
                  const tt = splitTitle(p.title);
                  return (
                    <div key={p.id} className="admin-card">
                      <div className="admin-card-img">
                        <img src={p.img} alt={p.title} draggable="false" />
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
                            onClick={() => {
                              if (p.category === "Gifts")
                                navigate(`/admin/gifts/edit/${p.id}`);
                              else navigate(`/admin/bouquets/edit/${p.id}`);
                            }}>
                            <img src={editIco} alt="Edit" />
                          </button>

                          <button
                            className="admin-mini-btn"
                            type="button"
                            onClick={() => handleDeleteProduct(p.id)}>
                            <img src={trashIco} alt="Delete" />
                          </button>
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            </section>
          )}

          {/* ========== ORDERS ========== */}
          {active === "orders" && (
            <section className="admin-section admin-orders">
              <h2 className="admin-section-title admin-orders-title">Orders</h2>
              <div className="admin-orders-top">
                <div />
                <div className="admin-orders-sort">
                  <div className="admin-orders-sort-label">SORT BY</div>
                  <select
                    className="admin-orders-sort-select"
                    value={sort}
                    onChange={(e) => setSort(e.target.value)}>
                    <option value="new">Date: New to old</option>
                    <option value="old">Date: Old to new</option>
                  </select>
                </div>
              </div>
              <div className="admin-orders-list">
                {sortedOrders.map((o) => (
                  <div
                    key={o.id}
                    className="admin-order-card"
                    onClick={() => navigate(`/admin/orders/${o.id}`)}>
                    <div className="admin-order-left">
                      <div className="admin-order-avatar">
                        <img src={o.avatar} alt="" />
                      </div>
                      <div className="admin-order-mid">
                        <div className="admin-order-title">{o.title}</div>
                        <div className="admin-order-sub">
                          <span className="admin-order-name">{o.customer}</span>
                          <span className="admin-order-date">{o.date}</span>
                        </div>
                      </div>
                    </div>
                    <div className="admin-order-status">
                      <select
                        className={`status-select status-${o.status.toLowerCase()}`}
                        value={o.status}
                        onClick={(e) => e.stopPropagation()}
                        onChange={(e) =>
                          handleStatusChange(o.id, e.target.value)
                        }>
                        {ORDER_STATUSES.map((s) => (
                          <option key={s} value={s}>
                            {s}
                          </option>
                        ))}
                      </select>
                    </div>
                    <div className="admin-order-right">
                      <div className="admin-order-total-value">{o.total}</div>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          )}

          {/* ========== CATALOG SETTINGS (active === 'catalog') ========== */}
          {active === "catalog" && (
            <section className="admin-section admin-catalog">
              <div className="admin-catalog-head">
                <h2 className="admin-section-title admin-catalog-title">
                  Catalog settings
                </h2>
                <button
                  className="admin-catalog-edit"
                  onClick={() => navigate("/admin/catalog/edit")}
                  type="button">
                  <span>Edit</span>
                  <img src={editIco} alt="" />
                </button>
              </div>
              <div className="admin-catalog-groups">
                <div className="admin-catalog-group">
                  <div className="admin-catalog-pill">Events</div>
                  <ul className="admin-catalog-list">
                    {catalogSettings.events.map((x) => (
                      <li key={x}>{x}</li>
                    ))}
                  </ul>
                </div>
                <div className="admin-catalog-group">
                  <div className="admin-catalog-pill">For who</div>
                  <ul className="admin-catalog-list">
                    {catalogSettings.forWho.map((x) => (
                      <li key={x}>{x}</li>
                    ))}
                  </ul>
                </div>
                <div className="admin-catalog-group">
                  <div className="admin-catalog-pill">Flower types</div>
                  <ul className="admin-catalog-list">
                    {catalogSettings.flowerTypes.map((x) => (
                      <li key={x}>{x}</li>
                    ))}
                  </ul>
                </div>
              </div>
            </section>
          )}

          {/* ========== REVIEWS ========== */}
          {active === "reviews" && (
            <section className="admin-section admin-reviews">
              <h2 className="admin-section-title">Pending reviews</h2>
              <div className="admin-reviews-list">
                {pendingReviews.map((r) => (
                  <div key={r.id} className="review-card">
                    <div className="review-left">
                      <div className="review-avatar">
                        <img src={r.avatar} alt="" />
                      </div>
                      <div className="review-body">
                        <div className="review-name">{r.name}</div>
                        <Stars value={r.stars} />
                        <div className="review-text">{r.text}</div>
                      </div>
                    </div>
                    <div className="review-actions">
                      <div className="review-actions-head">
                        <div className="review-action-label">Post</div>
                        <div className="review-action-label">Delete</div>
                      </div>
                      <div className="review-actions-btns">
                        <button
                          type="button"
                          className="review-icon-btn ok"
                          onClick={() => handlePostReview(r.id)}>
                          ✓
                        </button>
                        <button
                          type="button"
                          className="review-icon-btn del"
                          onClick={() => handleDeleteReview(r.id)}>
                          <img src={trashIco} alt="" />
                        </button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          )}
        </main>
      </div>
    </div>
  );
}
