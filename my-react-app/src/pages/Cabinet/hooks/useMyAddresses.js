import { useState, useEffect } from "react";
import { toast } from "react-hot-toast";
import axiosClient from "../../../api/axiosClient";
import { useConfirm } from "../../../context/ModalProvider";
import { useTranslation } from "react-i18next";
import userService from "../../../services/userService";

export function useMyAddresses(activeTab, TABS) {
  const { t } = useTranslation();
  const [addressList, setAddressList] = useState([]);
  const [newAddress, setNewAddress] = useState("");
  const confirm = useConfirm();

  const fetchAddresses = async () => {
    try {
      const data = await userService.getAddresses();
      setAddressList(data);
    } catch (error) {
      console.error("Failed to fetch addresses:", error);
    }
  };

  useEffect(() => {
    if (activeTab === TABS.ADDRESSES) fetchAddresses();
  }, [activeTab]);

  const handleSaveAddress = async () => {
    if (!newAddress.trim()) return;
    try {
      await userService.addAddress({
        address: newAddress,
        isDefault: false,
      });
      setNewAddress("");
      toast.success(t("toasts.address_added"));
      fetchAddresses();
    } catch (error) {
      console.error("Failed to save address:", error);
      toast.error(t("toasts.address_save_failed"));
    }
  };

  const handleDeleteAddress = (id) => {
    confirm({
      title: t("cabinet.delete_address"),
      message: t("cabinet.delete_address_confirm"),
      confirmText: t("cabinet.delete_confirm_btn"),
      confirmType: "danger",
      onConfirm: async () => {
        try {
          await userService.deleteAddress(id);
          toast.success(t("toasts.address_deleted"));
          fetchAddresses();
        } catch (error) {
          console.error("Failed to delete address:", error);
          toast.error(t("toasts.address_delete_failed"));
        }
      },
    });
  };

  return {
    addressList,
    newAddress,
    setNewAddress,
    handleSaveAddress,
    handleDeleteAddress,
  };
}
