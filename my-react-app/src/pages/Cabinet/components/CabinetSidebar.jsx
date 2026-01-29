import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../../context/useAuth";
import UserInfoIcon from "../../../assets/icons/userinfo.svg";
import OrderIcon from "../../../assets/icons/orders.svg";
import AddressIcon from "../../../assets/icons/address.svg";
import ExitIcon from "../../../assets/icons/exit.svg";
import EditIcon from "../../../assets/icons/edit.svg"; // Using edit icon for Admin Dashboard

export default function CabinetSidebar({
  activeTab,
  setActiveTab,
  TABS,
  onSignOut,
}) {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { user } = useAuth();
  const isAdmin = user?.role === "Admin";

  return (
    <aside className="cabinet-sidebar">
      <button
        className={`cabinet-nav-item ${
          activeTab === TABS.PERSONAL ? "active" : ""
        }`}
        onClick={() => setActiveTab(TABS.PERSONAL)}
        type="button">
        <img src={UserInfoIcon} className="cabinet-nav-icon" alt="" />
        <span>{t("cabinet.personal")}</span>
      </button>

      <button
        className={`cabinet-nav-item ${
          activeTab === TABS.ORDERS ? "active" : ""
        }`}
        onClick={() => setActiveTab(TABS.ORDERS)}
        type="button">
        <img src={OrderIcon} className="cabinet-nav-icon" alt="" />
        <span>{t("cabinet.orders")}</span>
      </button>

      <button
        className={`cabinet-nav-item ${
          activeTab === TABS.ADDRESSES ? "active" : ""
        }`}
        onClick={() => setActiveTab(TABS.ADDRESSES)}
        type="button">
        <img src={AddressIcon} className="cabinet-nav-icon" alt="" />
        <span>{t("cabinet.addresses")}</span>
      </button>

      <div className="cabinet-sidebar-spacer" />

      {isAdmin && (
        <button
          className="cabinet-nav-item"
          onClick={() => navigate("/admin")}
          type="button">
          <img src={EditIcon} className="cabinet-nav-icon" alt="" />
          <span>{t("admin.title")}</span>
        </button>
      )}

      <button className="cabinet-signout" onClick={onSignOut} type="button">
        <img src={ExitIcon} className="cabinet-nav-icon" alt="" />
        <span>{t("cabinet.sign_out")}</span>
      </button>
    </aside>
  );
}
