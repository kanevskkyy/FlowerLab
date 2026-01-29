import React from "react";
import { useFormContext } from "react-hook-form";
import { useTranslation } from "react-i18next";

const ReceiverInfo = () => {
  const { t } = useTranslation();
  const {
    register,
    watch,
    formState: { errors },
  } = useFormContext();

  const receiverType = watch("receiverType");

  return (
    <section className="form-section receiver-section-wrapper">
      <div className="receiver-section">
        <label>{t("checkout.receiver_title")}</label>
        <div className="radio-group">
          <label className="radio-label">
            <input type="radio" value="self" {...register("receiverType")} />
            <span>{t("checkout.receiver_self")}</span>
          </label>
          <label className="radio-label">
            <input type="radio" value="other" {...register("receiverType")} />
            <span>{t("checkout.receiver_other")}</span>
          </label>
        </div>
      </div>

      {/* Conditional Receiver Fields */}
      {receiverType === "other" && (
        <div className="receiver-fields fade-in-section">
          <div className="form-group">
            <label>{t("checkout.receiver_name")}</label>
            <input
              type="text"
              placeholder={t("checkout.receiver_name")}
              style={
                errors.receiverName
                  ? {
                      border: "1px solid #d32f2f",
                    }
                  : {}
              }
              {...register("receiverName")}
            />
            {errors.receiverName && (
              <p className="error-text">{errors.receiverName.message}</p>
            )}
          </div>
          <div className="form-group">
            <label>{t("checkout.receiver_phone")}</label>
            <input
              type="tel"
              placeholder={t("checkout.receiver_phone")}
              style={
                errors.receiverPhone
                  ? {
                      border: "1px solid #d32f2f",
                    }
                  : {}
              }
              {...register("receiverPhone")}
            />
            {errors.receiverPhone && (
              <p className="error-text">{errors.receiverPhone.message}</p>
            )}
          </div>
        </div>
      )}
    </section>
  );
};

export default ReceiverInfo;
