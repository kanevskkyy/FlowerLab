import React from "react";
import { useFormContext } from "react-hook-form";
import { useTranslation } from "react-i18next";

const ContactInfo = () => {
  const { t } = useTranslation();
  const {
    register,
    formState: { errors },
  } = useFormContext();

  return (
    <section className="form-section">
      <h2>{t("checkout.contact_info_title")}</h2>

      <div className="form-group">
        <label>{t("auth.first_name")}</label>
        <input
          type="text"
          placeholder={t("auth.first_name")}
          style={errors.firstName ? { border: "1px solid #d32f2f" } : {}}
          {...register("firstName")}
        />
        {errors.firstName && (
          <p className="error-text">{errors.firstName.message}</p>
        )}
      </div>

      <div className="form-group">
        <label>{t("auth.last_name")}</label>
        <input
          type="text"
          placeholder={t("auth.last_name")}
          style={errors.lastName ? { border: "1px solid #d32f2f" } : {}}
          {...register("lastName")}
        />
        {errors.lastName && (
          <p className="error-text">{errors.lastName.message}</p>
        )}
      </div>

      <div className="form-group">
        <label>{t("auth.phone")}</label>
        <input
          type="tel"
          placeholder="+38 066 001 02 03"
          style={errors.phone ? { border: "1px solid #d32f2f" } : {}}
          {...register("phone")}
        />
        {errors.phone && <p className="error-text">{errors.phone.message}</p>}
      </div>
    </section>
  );
};

export default ContactInfo;
