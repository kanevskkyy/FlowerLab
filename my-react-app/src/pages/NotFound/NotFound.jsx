import React from "react";
import { useNavigate } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import "./NotFound.css";

const NotFound = () => {
  const navigate = useNavigate();

  return (
    <div className="page-wrapper not-found-page">
      <Header />

      <main className="not-found-content">
        <h1 className="not-found-code">404</h1>
        <h2 className="not-found-title">Oops! Page not found</h2>
        <p className="not-found-text">
          The page you are looking for might have been removed, had its name
          changed, or is temporarily unavailable.
        </p>
        <button className="not-found-btn" onClick={() => navigate("/")}>
          BACK TO HOMEPAGE
        </button>
      </main>

      <Footer />
    </div>
  );
};

export default NotFound;
