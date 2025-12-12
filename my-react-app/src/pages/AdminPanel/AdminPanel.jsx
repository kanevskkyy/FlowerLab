import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/useAuth";

import { useMemo, useState } from "react";
import "./AdminPanel.css";

import bouquetsIco from "../../assets/icons/flowerr.svg";
import ordersIco from "../../assets/icons/orders.svg";
import catalogIco from "../../assets/icons/list.svg";
import reviewsIco from "../../assets/icons/review.svg";

import bellIco from "../../assets/icons/bell.svg";
import logoutIco from "../../assets/icons/exit.svg";

import searchIco from "../../assets/icons/search.svg";
import editIco from "../../assets/icons/edit.svg";
import trashIco from "../../assets/icons/trash.svg";

// demo images
import bouquet1L from "../../assets/images/bouquet1L.jpg";
import bouquet2L from "../../assets/images/bouquet2L.jpg";
import bouquet3L from "../../assets/images/bouquet3L.jpg";
import testphoto from "../../assets/images/testphoto.jpg";

const NAV = [
  { key: "bouquets", label: "Bouquets", icon: bouquetsIco },
  { key: "orders", label: "Orders", icon: ordersIco },
  { key: "catalog", label: "Catalog", icon: catalogIco },
  { key: "reviews", label: "Reviews", icon: reviewsIco },
];

export default function AdminPanel() {
  const [active, setActive] = useState("bouquets");
  const [q, setQ] = useState("");
  const navigate = useNavigate();
  const { logout } = useAuth();

  const handleSignOut = () => {
    logout(); // очищає user / token / context
    navigate("/login", { replace: true });
  };

  // ========= BOUQUETS =========
  const bouquets = useMemo(
    () => [
      { id: 1, title: "Bouquet Roses", img: testphoto },
      { id: 2, title: "Bouquet Peonies", img: bouquet2L },
      { id: 3, title: "Bouquet Hydrangea", img: bouquet3L },
      { id: 4, title: "Bouquet Orchids", img: bouquet1L },
      { id: 5, title: "Bouquet Ranunculus", img: bouquet2L },
      { id: 6, title: "Bouquet Daisies", img: bouquet3L },
    ],
    []
  );

  const filteredBouquets = useMemo(() => {
    const s = q.trim().toLowerCase();
    if (!s) return bouquets;
    return bouquets.filter((b) => b.title.toLowerCase().includes(s));
  }, [q, bouquets]);

  const splitTitle = (t) => {
    const parts = t.trim().split(" ");
    if (parts.length <= 1) return { a: t, b: "" };
    return { a: parts[0], b: parts.slice(1).join(" ") };
  };

  // ========= ORDERS =========
  const [sort, setSort] = useState("new");

  const orders = useMemo(
    () => [
      {
        id: 1001,
        title: "Bouquet Orchids",
        qty: "1 pc",
        customer: "Name Surname",
        date: "at 10:06:10 25.10.25",
        total: "1000 ₴",
        avatar: testphoto,
      },
      {
        id: 1002,
        title: "Bouquet 101 Roses",
        qty: "1 pc",
        customer: "Name Surname",
        date: "at 10:06:10 25.10.25",
        total: "1000 ₴",
        avatar: bouquet1L,
      },
      {
        id: 1003,
        title: "Bouquet Onyx",
        qty: "1 pc",
        customer: "Name Surname",
        date: "at 10:06:10 25.10.25",
        total: "1000 ₴",
        avatar: bouquet2L,
      },
      {
        id: 1004,
        title: "Bouquet Pomegranate",
        qty: "1 pc",
        customer: "Name Surname",
        date: "at 10:06:10 25.10.25",
        total: "1000 ₴",
        avatar: bouquet3L,
      },
      {
        id: 1005,
        title: "Bouquet Lilies",
        qty: "1 pc",
        customer: "Name Surname",
        date: "at 10:06:10 25.10.25",
        total: "1000 ₴",
        avatar: bouquet1L,
      },
      {
        id: 1006,
        title: "Bouquet Daisies",
        qty: "1 pc",
        customer: "Name Surname",
        date: "at 10:06:10 25.10.25",
        total: "1000 ₴",
        avatar: bouquet2L,
      },
    ],
    []
  );

  const sortedOrders = useMemo(() => {
    if (sort === "old") return [...orders].reverse();
    return orders;
  }, [orders, sort]);

  // ========= CATALOG =========
  const catalogSettings = useMemo(
    () => ({
      events: ["Birthday", "Wedding", "Engagement"],
      forWho: ["Mom", "Wife", "Husband", "Kid", "Teacher", "Co-worker"],
      flowerTypes: [
        "Peony",
        "Rose",
        "Lily",
        "Tulip",
        "Orchid",
        "Hydrangea",
        "Daffodil",
        "Chrysantemum",
      ],
    }),
    []
  );

  // ========= REVIEWS (Pending reviews) =========
  const initialReviews = useMemo(
    () => [
      {
        id: 1,
        name: "Name Surname",
        stars: 5,
        text: "such a pretty bouquet! will by again^^",
        avatar: testphoto,
      },
      {
        id: 2,
        name: "Name Surname",
        stars: 4,
        text: "nice bouquet, quick delivery",
        avatar: bouquet1L,
      },
      {
        id: 3,
        name: "Name Surname",
        stars: 2,
        text:
          "horrible service! first of all, bouquet didn't arrive in time, " +
          "the delivery guy delivered it to the other address and I was waiting for him " +
          "for a whole hour to get my flowers! second of all, the bouquet turned out not " +
          "to be fresh, many flowers are already discoloured. won't order from here again.",
        avatar: bouquet2L,
      },
      {
        id: 4,
        name: "Name Surname",
        stars: 5,
        text: "pretty flowers, pretty packaging, love it!",
        avatar: bouquet3L,
      },
      {
        id: 5,
        name: "Name Surname",
        stars: 5,
        text: "my girlfriend liked the flowers, thanks for quick delivery",
        avatar: testphoto,
      },
      {
        id: 6,
        name: "Name Surname",
        stars: 5,
        text: "amazing bouquet!",
        avatar: bouquet1L,
      },
    ],
    []
  );

  const [pendingReviews, setPendingReviews] = useState(initialReviews);

  const handlePostReview = (id) => {
    // демо: просто прибираємо з pending
    setPendingReviews((prev) => prev.filter((r) => r.id !== id));
  };

  const handleDeleteReview = (id) => {
    setPendingReviews((prev) => prev.filter((r) => r.id !== id));
  };

  const Stars = ({ value }) => {
    const total = 5;
    return (
      <div className="review-stars" aria-label={`${value} of 5`}>
        {Array.from({ length: total }).map((_, i) => (
          <span key={i} className={`star ${i < value ? "on" : "off"}`}>★</span>
        ))}
      </div>
    );
  };

  return (
    <div className="admin-root">
      {/* LEFT SIDEBAR */}
      <aside className="admin-side">
        <div className="admin-brand">
          <div className="admin-brand-top">FLOWER LAB</div>
          <div className="admin-brand-sub">VLADA</div>
          <div className="admin-brand-line" />
        </div>

        <nav className="admin-nav">
          {NAV.map((item) => (
            <button
              key={item.key}
              type="button"
              className={`admin-nav-item ${active === item.key ? "active" : ""}`}
              onClick={() => setActive(item.key)}
            >
              <img className="admin-nav-ico" src={item.icon} alt="" />
              <span>{item.label}</span>
            </button>
          ))}
        </nav>
      </aside>

      {/* RIGHT */}
      <div className="admin-main">
        {/* TOPBAR */}
        <header className="admin-topbar">
          <div className="admin-topbar-center">Admin panel</div>

          <div className="admin-topbar-right">
            <button className="admin-top-ico-btn" type="button" aria-label="Notifications">
              <img src={bellIco} alt="" />
            </button>

           <button className="admin-top-logout" type="button" onClick={handleSignOut}>
  <img src={logoutIco} alt="" />
  <span>Log out</span>
</button>
          </div>
        </header>

        {/* CONTENT */}
        <main className="admin-content">
          {/* ========== BOUQUETS ========== */}
          {active === "bouquets" && (
            <section className="admin-section">
              <h2 className="admin-section-title">Bouquets management</h2>

              <div className="admin-toolbar">
                <div className="admin-search">
                  <img className="admin-search-ico" src={searchIco} alt="" />
                  <input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Search by name" />
                </div>

                <button className="admin-add-btn" type="button">
                  Add a bouquet <span className="admin-plus">+</span>
                </button>
              </div>

              <div className="admin-grid">
                {filteredBouquets.map((b) => {
                  const tt = splitTitle(b.title);
                  return (
                    <div key={b.id} className="admin-card">
                      <div className="admin-card-img">
                        <img src={b.img} alt={b.title} draggable="false" />
                      </div>

                      <div className="admin-card-bottom">
                        <div className="admin-card-title">
                          <span className="t1">{tt.a}</span>
                          {tt.b ? <span className="t2">{tt.b}</span> : null}
                        </div>

                        <div className="admin-card-actions">
                          <button className="admin-mini-btn" type="button" aria-label="Edit">
                            <img src={editIco} alt="" />
                          </button>
                          <button className="admin-mini-btn" type="button" aria-label="Delete">
                            <img src={trashIco} alt="" />
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
                    onChange={(e) => setSort(e.target.value)}
                  >
                    <option value="new">Date: New to old</option>
                    <option value="old">Date: Old to new</option>
                  </select>
                </div>
              </div>

              <div className="admin-orders-list">
                {sortedOrders.map((o) => (
                  <div key={o.id} className="admin-order-card">
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

                    <div className="admin-order-qty">{o.qty}</div>

                    <div className="admin-order-right">
                      <div className="admin-order-total-label">Order Total:</div>
                      <div className="admin-order-total-value">{o.total}</div>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          )}

          {/* ========== CATALOG ========== */}
          {active === "catalog" && (
            <section className="admin-section admin-catalog">
              <div className="admin-catalog-head">
                <h2 className="admin-section-title admin-catalog-title">Catalog settings</h2>

                <button className="admin-catalog-edit" type="button" onClick={() => console.log("edit catalog")}>
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
                          aria-label="Post review"
                          onClick={() => handlePostReview(r.id)}
                        >
                          ✓
                        </button>

                        <button
                          type="button"
                          className="review-icon-btn del"
                          aria-label="Delete review"
                          onClick={() => handleDeleteReview(r.id)}
                        >
                          <img src={trashIco} alt="" />
                        </button>
                      </div>
                    </div>
                  </div>
                ))}

                {pendingReviews.length === 0 && (
                  <div className="admin-placeholder">No pending reviews.</div>
                )}
              </div>
            </section>
          )}
        </main>
      </div>
    </div>
  );
}
