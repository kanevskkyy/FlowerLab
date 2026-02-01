import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import toast from "react-hot-toast";
import searchIcon from "../../../assets/icons/search.svg";
import editIcon from "../../../assets/icons/edit.svg";

export default function AdminUsersList({
  users = [],
  loading,
  q,
  setQ,
  onUpdateDiscount,
  loadMore,
  hasNextPage,
  isLoadingMore,
}) {
  const { t } = useTranslation();
  const [editUserId, setEditUserId] = useState(null);
  const [editValue, setEditValue] = useState("");

  const handleEditClick = (user) => {
    setEditUserId(user.id);
    setEditValue(user.personalDiscountPercentage || 0);
  };

  const handleSave = async (userId) => {
    const success = await onUpdateDiscount(userId, parseFloat(editValue));
    if (success) {
      toast.success(t("admin.users.edit_discount.success"));
      setEditUserId(null);
    } else {
      toast.error(t("admin.users.edit_discount.error"));
    }
  };

  return (
    <section className="admin-section">
      <h2 className="admin-section-title">{t("admin.users.title")}</h2>

      <div className="admin-toolbar">
        <div className="admin-search">
          <img className="admin-search-ico" src={searchIcon} alt="" />
          <input
            type="text"
            placeholder={t("admin.users.search_placeholder")}
            value={q}
            onChange={(e) => setQ(e.target.value)}
          />
        </div>
      </div>

      <div className="admin-users-list">
        <div className="admin-users-table-header">
          <div className="col-name">{t("admin.users.table.name")}</div>
          <div className="col-email">{t("admin.users.table.email")}</div>
          <div className="col-phone">{t("admin.users.table.phone")}</div>
          <div className="col-discount">{t("admin.users.table.discount")}</div>
          <div className="col-actions">{t("admin.users.table.actions")}</div>
        </div>

        {users.map((user) => (
          <div key={user.id} className="admin-user-row">
            <div className="col-name">
              <div className="admin-user-identity">
                <div className="admin-user-avatar">
                  {user.photoUrl ? (
                    <img src={user.photoUrl} alt="" />
                  ) : (
                    <div className="avatar-placeholder">
                      {user.firstName?.charAt(0) || "U"}
                    </div>
                  )}
                </div>
                <div className="admin-user-names">
                  <span className="name-primary">
                    {user.firstName} {user.lastName}
                  </span>
                </div>
              </div>
            </div>

            <div className="col-email">{user.email}</div>
            <div className="col-phone">{user.phoneNumber || "-"}</div>

            <div className="col-discount">
              {editUserId === user.id ? (
                <div className="discount-edit-box">
                  <input
                    type="number"
                    min="0"
                    max="100"
                    value={editValue}
                    onChange={(e) => setEditValue(e.target.value)}
                    autoFocus
                  />
                  <span>%</span>
                </div>
              ) : (
                <span className="discount-val">
                  {user.personalDiscountPercentage || 0}%
                </span>
              )}
            </div>

            <div className="col-actions">
              {editUserId === user.id ? (
                <div className="admin-actions-confirm">
                  <button
                    className="admin-save-btn-small"
                    onClick={() => handleSave(user.id)}>
                    {t("admin.users.edit_discount.save")}
                  </button>
                  <button
                    className="admin-cancel-link-small"
                    onClick={() => setEditUserId(null)}>
                    {t("admin.users.edit_discount.cancel")}
                  </button>
                </div>
              ) : (
                <button
                  className="admin-mini-btn"
                  onClick={() => handleEditClick(user)}
                  title={t("admin.users.edit_discount.title")}>
                  <img src={editIcon} alt="edit" />
                </button>
              )}
            </div>
          </div>
        ))}
      </div>

      {loading && <div className="admin-loading">{t("admin.loading")}</div>}

      {!loading && users.length === 0 && (
        <div className="admin-no-results">
          <p>{t("admin.users.no_users")}</p>
        </div>
      )}

      {hasNextPage && (
        <div
          className="admin-load-more"
          style={{ marginTop: "30px", textAlign: "center" }}>
          <button
            className="admin-add-btn"
            onClick={loadMore}
            disabled={isLoadingMore}>
            {isLoadingMore ? "..." : t("admin.users.load_more")}
          </button>
        </div>
      )}
    </section>
  );
}
