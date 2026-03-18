import React from "react";
import { Helmet } from "react-helmet-async";
import { useTranslation } from "react-i18next";
import "./PublicOffer.css";

const PublicOffer = () => {
  const { t } = useTranslation();

  return (
    <>
      <Helmet>
        <title>{t("public_offer_page.title")} | FlowerLab</title>
        <meta
          name="description"
          content={t("public_offer_page.seo_desc")}
        />
      </Helmet>
      <div className="info-page-container">
        <h1>{t("public_offer_page.title")}</h1>

        <div className="info-page-section">
          <h2>{t("public_offer_page.s1_title")}</h2>
          <p>{t("public_offer_page.s1_1")}</p>
          <p>{t("public_offer_page.s1_2")}</p>
          <p>{t("public_offer_page.s1_3")}</p>
          <p>{t("public_offer_page.s1_4")}</p>
        </div>

        <div className="info-page-section">
          <h2>{t("public_offer_page.s2_title")}</h2>
          <p>{t("public_offer_page.s2_1")}</p>
          <p>{t("public_offer_page.s2_2")}</p>
          <p>{t("public_offer_page.s2_3")}</p>
        </div>

        <div className="info-page-section">
          <h2>{t("public_offer_page.s3_title")}</h2>
          <p>{t("public_offer_page.s3_1")}</p>
          <p>{t("public_offer_page.s3_2")}</p>
          <p>{t("public_offer_page.s3_3")}</p>
        </div>

        <div className="info-page-section">
          <h2>{t("public_offer_page.s4_title")}</h2>
          <p>{t("public_offer_page.s4_1")}</p>
          <p>{t("public_offer_page.s4_2")}</p>
        </div>

        <div className="info-page-section">
          <h2>{t("public_offer_page.s5_title")}</h2>
          <p>{t("public_offer_page.s5_1")}</p>
          <p>{t("public_offer_page.s5_2")}</p>
          <p>{t("public_offer_page.s5_3")}</p>
          <p>{t("public_offer_page.s5_4")}</p>
          <p>{t("public_offer_page.s5_5")}</p>
        </div>

        <div className="info-page-section">
          <h2>{t("public_offer_page.s6_title")}</h2>
          <p>{t("public_offer_page.s6_1")}</p>
          <p>{t("public_offer_page.s6_2")}</p>
          <p>{t("public_offer_page.s6_3")}</p>
        </div>

        <div className="info-page-section">
          <h2>{t("public_offer_page.s7_title")}</h2>
          <p>{t("public_offer_page.s7_1")}</p>
          <p>{t("public_offer_page.s7_2")}</p>
          <p>{t("public_offer_page.s7_3")}</p>
        </div>

        <div className="info-page-section">
          <h2>{t("public_offer_page.s8_title")}</h2>
          <p>{t("public_offer_page.s8_1")}</p>
          <p>{t("public_offer_page.s8_2")}</p>
        </div>

        <div className="info-page-section">
          <h2>{t("public_offer_page.s9_title")}</h2>
          <p>{t("public_offer_page.s9_1")}</p>
          <p>{t("public_offer_page.s9_2")}</p>
          <p>{t("public_offer_page.s9_3")}</p>
        </div>
      </div>
    </>
  );
};

export default PublicOffer;
