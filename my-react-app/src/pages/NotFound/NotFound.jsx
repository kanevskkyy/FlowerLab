import React from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import "./NotFound.css";

const NotFound = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();

  return (
    <div className="page-wrapper not-found-page">
      <Header />

      <main className="not-found-content">
        <h1 className="not-found-code">404</h1>
        <h2 className="not-found-title">{t("not_found_page.title")}</h2>
        <p className="not-found-text">{t("not_found_page.text")}</p>
        <button className="not-found-btn" onClick={() => navigate("/")}>
          {t("not_found_page.button")}
        </button>
      </main>

      <Footer />
    </div>
  );
};

export default NotFound;
