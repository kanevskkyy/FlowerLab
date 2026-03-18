import React from "react";
import { Helmet } from "react-helmet-async";
import { useTranslation, Trans } from "react-i18next";
import "./RefundPolicy.css";

const RefundPolicy = () => {
  const { t } = useTranslation();

  return (
    <>
      <Helmet>
        <title>{t("refund_policy.title")} | FlowerLab</title>
        <meta
          name="description"
          content={t("refund_policy.seo_desc")}
        />
      </Helmet>
      <div className="info-page-container">
        <h1>{t("refund_policy.title")}</h1>

        <div className="info-page-section">
          <h2>{t("refund_policy.general_title")}</h2>
          <p>{t("refund_policy.general_p1")}</p>
          <p>{t("refund_policy.general_p2")}</p>
          <ul>
            <li>{t("refund_policy.general_l1")}</li>
            <li>{t("refund_policy.general_l2")}</li>
            <li>{t("refund_policy.general_l3")}</li>
          </ul>
        </div>

        <div className="info-page-section">
          <h2>{t("refund_policy.claims_title")}</h2>
          <ul>
            <li>
              <Trans
                i18nKey="refund_policy.claims_l1"
                components={{ 1: <strong /> }}
              />
            </li>
            <li>
              <Trans
                i18nKey="refund_policy.claims_l2"
                components={{ 1: <strong /> }}
              />
            </li>
          </ul>
        </div>

        <div className="info-page-section">
          <h2>{t("refund_policy.refund_title")}</h2>
          <p>{t("refund_policy.refund_p1")}</p>
          <p>
            <Trans
              i18nKey="refund_policy.refund_p2"
              components={{ 1: <strong /> }}
            />
          </p>
          <p>{t("refund_policy.refund_p3")}</p>
        </div>
      </div>
    </>
  );
};

export default RefundPolicy;
