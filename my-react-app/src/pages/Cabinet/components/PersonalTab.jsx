import React from "react";
import { useTranslation } from "react-i18next";
import { usePersonalSettings } from "../hooks/usePersonalSettings";

// Icons
import MessageIcon from "../../../assets/icons/message.svg";
import LockIcon from "../../../assets/icons/lock.svg";
import TrashIcon from "../../../assets/icons/trash-icon.svg";

export default function PersonalTab() {
  const { t } = useTranslation();
  const {
    form,
    formErrors,
    photoPreview,
    onChange,
    handleFileChange,
    handleProfileUpdate,
    isPasswordModalOpen,
    setIsPasswordModalOpen,
    passwordForm,
    isDeleteModalOpen,
    setIsDeleteModalOpen,
    handleAccountDelete,
    handlePasswordChange,
    onPasswordChange,
    passwordErrors,
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
        <div
          className={`cabinet-field ${formErrors.firstName ? "has-error" : ""}`}>
          <label>{t("auth.first_name")}</label>
          <input
            value={form.firstName}
            onChange={onChange("firstName")}
            placeholder="Name"
          />
          {formErrors.firstName && (
            <span className="cabinet-error-text">{formErrors.firstName}</span>
          )}
        </div>

        <div
          className={`cabinet-field ${formErrors.lastName ? "has-error" : ""}`}>
          <label>{t("auth.last_name")}</label>
          <input
            value={form.lastName}
            onChange={onChange("lastName")}
            placeholder="Name"
          />
          {formErrors.lastName && (
            <span className="cabinet-error-text">{formErrors.lastName}</span>
          )}
        </div>
      </div>

      <div className="cabinet-grid-2 cabinet-grid-mobile-single">
        <div className={`cabinet-field ${formErrors.phone ? "has-error" : ""}`}>
          <label>{t("auth.phone")}</label>
          <input
            value={form.phone}
            onChange={onChange("phone")}
            placeholder="+38 066 000 03 01"
          />
          {formErrors.phone && (
            <span className="cabinet-error-text">{formErrors.phone}</span>
          )}
        </div>

        {form.discount > 0 && (
          <div className="cabinet-field cabinet-discount-field">
            <label>{t("cabinet.personal_discount")}</label>
            <div className="cabinet-discount-badge">
              <span className="discount-value">{form.discount}%</span>
            </div>
          </div>
        )}
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

      <div className="cabinet-danger-zone">
        <button
          className="cabinet-account-delete-btn"
          type="button"
          onClick={() => setIsDeleteModalOpen(true)}>
          <img src={TrashIcon} className="cabinet-account-delete-icon" alt="" />
          <span>{t("cabinet.delete_account_btn")}</span>
        </button>
      </div>

      {/* ===== PASSWORD MODAL ===== */}
      {isPasswordModalOpen && (
        // ... (existing password modal code remains the same)
        <div className="modal-overlay">
          <div className="modal-content cabinet-password-modal">
            <h2 className="modal-title">{t("cabinet.change_password")}</h2>
            <form onSubmit={handlePasswordChange}>
              <div
                className={`cabinet-field ${passwordErrors.oldPassword ? "has-error" : ""}`}>
                <label>{t("cabinet.old_password")}</label>
                <input
                  type="password"
                  value={passwordForm.oldPassword}
                  onChange={onPasswordChange("oldPassword")}
                  placeholder="********"
                  autoComplete="current-password"
                />
                {passwordErrors.oldPassword && (
                  <span className="cabinet-error-text">
                    {passwordErrors.oldPassword}
                  </span>
                )}
              </div>

              <div
                className={`cabinet-field ${passwordErrors.newPassword ? "has-error" : ""}`}>
                <label>{t("cabinet.new_password")}</label>
                <input
                  type="password"
                  value={passwordForm.newPassword}
                  onChange={onPasswordChange("newPassword")}
                  placeholder="********"
                  autoComplete="new-password"
                />
                {passwordErrors.newPassword && (
                  <span className="cabinet-error-text">
                    {passwordErrors.newPassword}
                  </span>
                )}
              </div>

              <div
                className={`cabinet-field ${passwordErrors.confirmPassword ? "has-error" : ""}`}>
                <label>{t("cabinet.confirm_new_password")}</label>
                <input
                  type="password"
                  value={passwordForm.confirmPassword}
                  onChange={onPasswordChange("confirmPassword")}
                  placeholder="********"
                  autoComplete="new-password"
                />
                {passwordErrors.confirmPassword && (
                  <span className="cabinet-error-text">
                    {passwordErrors.confirmPassword}
                  </span>
                )}
              </div>

              <div className="modal-actions">
                <button
                  className="modal-btn-cancel"
                  type="button"
                  onClick={() => {
                    setIsPasswordModalOpen(false);
                  }}>
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

      {/* ===== DELETE ACCOUNT MODAL ===== */}
      {isDeleteModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content cabinet-delete-modal">
            <h2 className="modal-title">{t("cabinet.delete_account_title")}</h2>
            <p className="modal-text">{t("cabinet.delete_account_confirm")}</p>
            <div className="modal-actions">
              <button
                className="modal-btn-cancel"
                type="button"
                onClick={() => setIsDeleteModalOpen(false)}>
                {t("cabinet.cancel")}
              </button>
              <button
                className="modal-btn-delete"
                type="button"
                onClick={handleAccountDelete}>
                {t("cabinet.delete_account_confirm_btn")}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
