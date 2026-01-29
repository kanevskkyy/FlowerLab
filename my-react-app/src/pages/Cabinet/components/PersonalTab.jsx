import React from "react";
import { useTranslation } from "react-i18next";
import { usePersonalSettings } from "../hooks/usePersonalSettings";

// Icons
import MessageIcon from "../../../assets/icons/message.svg";
import LockIcon from "../../../assets/icons/lock.svg";

export default function PersonalTab() {
  const { t } = useTranslation();
  const {
    form,
    photoPreview,
    onChange,
    handleFileChange,
    handleProfileUpdate,
    isPasswordModalOpen,
    setIsPasswordModalOpen,
    passwordForm,
    onPasswordChange,
    handlePasswordChange,
  } = usePersonalSettings();

  return (
    <div className="cabinet-panel-inner">
      <h1 className="cabinet-title">{t("cabinet.personal_title")}</h1>

      <div className="cabinet-avatar-section">
        <div
          className="cabinet-avatar-wrapper"
          onClick={() => document.getElementById("avatar-input").click()}>
          {photoPreview ? (
            <img
              src={photoPreview}
              alt="Avatar"
              className="cabinet-avatar-img"
            />
          ) : (
            <div className="cabinet-avatar-placeholder">
              {form.firstName?.charAt(0)}
              {form.lastName?.charAt(0)}
            </div>
          )}
          <div className="cabinet-avatar-overlay">
            <span>{t("cabinet.change_photo")}</span>
          </div>
        </div>
        <input
          id="avatar-input"
          type="file"
          accept="image/*"
          onChange={handleFileChange}
          style={{ display: "none" }}
        />
        <div className="cabinet-avatar-info">
          <h3>{t("cabinet.profile_photo")}</h3>
          <p>{t("cabinet.update_avatar")}</p>
        </div>
      </div>

      <div className="cabinet-grid-2">
        <div className="cabinet-field">
          <label>{t("auth.first_name")}</label>
          <input
            value={form.firstName}
            onChange={onChange("firstName")}
            placeholder="Name"
          />
        </div>

        <div className="cabinet-field">
          <label>{t("auth.last_name")}</label>
          <input
            value={form.lastName}
            onChange={onChange("lastName")}
            placeholder="Name"
          />
        </div>
      </div>

      <div className="cabinet-grid-1 cabinet-grid-single-left">
        <div className="cabinet-field">
          <label>{t("auth.phone")}</label>
          <input
            value={form.phone}
            onChange={onChange("phone")}
            placeholder="+38 066 000 03 01"
          />
        </div>
      </div>

      <h2 className="cabinet-subtitle">{t("cabinet.account_title")}</h2>

      <div className="cabinet-grid-2">
        <div className="cabinet-pill">
          <div className="cabinet-pill-left">
            <img src={MessageIcon} className="cabinet-pill-icon" alt="" />
            <span className="cabinet-pill-text">{form.email}</span>
          </div>
        </div>

        <div className="cabinet-pill">
          <div className="cabinet-pill-left">
            <img src={LockIcon} className="cabinet-pill-icon" alt="" />
            <span className="cabinet-pill-text">{t("auth.password")}</span>
          </div>
          <button
            className="cabinet-pill-btn"
            type="button"
            onClick={() => setIsPasswordModalOpen(true)}>
            {t("cabinet.change")}
          </button>
        </div>
      </div>

      <button
        className="cabinet-save"
        type="button"
        onClick={handleProfileUpdate}>
        {t("cabinet.save")}
      </button>

      {/* ===== PASSWORD MODAL ===== */}
      {isPasswordModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content cabinet-password-modal">
            <h2 className="modal-title">{t("cabinet.change_password")}</h2>
            <form onSubmit={handlePasswordChange}>
              <div className="cabinet-field">
                <label>{t("cabinet.old_password")}</label>
                <input
                  type="password"
                  value={passwordForm.oldPassword}
                  onChange={onPasswordChange("oldPassword")}
                  placeholder="********"
                  required
                />
              </div>

              <div className="cabinet-field">
                <label>{t("cabinet.new_password")}</label>
                <input
                  type="password"
                  value={passwordForm.newPassword}
                  onChange={onPasswordChange("newPassword")}
                  placeholder="********"
                  required
                />
              </div>

              <div className="cabinet-field">
                <label>{t("cabinet.confirm_new_password")}</label>
                <input
                  type="password"
                  value={passwordForm.confirmPassword}
                  onChange={onPasswordChange("confirmPassword")}
                  placeholder="********"
                  required
                />
              </div>

              <div className="modal-actions">
                <button
                  className="modal-btn-cancel"
                  type="button"
                  onClick={() => setIsPasswordModalOpen(false)}>
                  {t("cabinet.cancel")}
                </button>
                <button className="modal-btn-save" type="submit">
                  {t("cabinet.update_password_btn")}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
