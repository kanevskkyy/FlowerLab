import React from "react";
import { Helmet } from "react-helmet-async";
import { useTranslation, Trans } from "react-i18next";
import "./DeliveryInfo.css";

const DeliveryInfo = () => {
  const { t } = useTranslation();

  return (
    <>
      <Helmet>
        <title>{t("delivery_info.title")} | FlowerLab</title>
        <meta
          name="description"
          content={t("delivery_info.seo_desc")}
        />
      </Helmet>
      <div className="info-page-container">
        <h1>{t("delivery_info.title")}</h1>

        <div className="info-page-section">
          <h2>{t("delivery_info.payment_title")}</h2>
          <ul>
            <li>
              <Trans
                i18nKey="delivery_info.payment_p1"
                components={{ 1: <strong /> }}
              />
            </li>
            <li>{t("delivery_info.payment_p2")}</li>
          </ul>
        </div>

        <div className="info-page-section">
          <h2>{t("delivery_info.delivery_title")}</h2>
          <ul>
            <li>{t("delivery_info.delivery_p1")}</li>
            <li>{t("delivery_info.delivery_p2")}</li>
            <li>{t("delivery_info.delivery_p3")}</li>
            <li>{t("delivery_info.delivery_p4")}</li>
            <li>{t("delivery_info.delivery_p5")}</li>
          </ul>
        </div>
      </div>
    </>
  );
};

export default DeliveryInfo;
