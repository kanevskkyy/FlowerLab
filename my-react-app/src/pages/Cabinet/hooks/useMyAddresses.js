import { useState, useEffect } from "react";
import { toast } from "react-hot-toast";
import axiosClient from "../../../api/axiosClient";
import { useConfirm } from "../../../context/ModalProvider";
import { useTranslation } from "react-i18next";

export function useMyAddresses(activeTab, TABS) {
  const { t } = useTranslation();
  const [addressList, setAddressList] = useState([]);
  const [newAddress, setNewAddress] = useState("");
  const confirm = useConfirm();

  const fetchAddresses = async () => {
    try {
      const { data } = await axiosClient.get("/api/users/me/addresses");
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
      await axiosClient.post("/api/users/me/addresses", {
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
      title: "Delete address?",
      message: "Are you sure you want to delete this address?",
      confirmText: "Delete",
      confirmType: "danger",
      onConfirm: async () => {
        try {
          await axiosClient.delete(`/api/users/me/addresses/${id}`);
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
