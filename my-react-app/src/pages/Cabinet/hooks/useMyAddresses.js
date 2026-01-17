import { useState, useEffect } from "react";
import { toast } from "react-hot-toast";
import axiosClient from "../../../api/axiosClient";

export function useMyAddresses(activeTab, TABS) {
  const [addressList, setAddressList] = useState([]);
  const [newAddress, setNewAddress] = useState("");

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
      toast.success("Address added successfully!");
      fetchAddresses();
    } catch (error) {
      console.error("Failed to save address:", error);
      toast.error("Failed to save address.");
    }
  };

  const handleDeleteAddress = async (id) => {
    if (!window.confirm("Are you sure you want to delete this address?")) return;
    try {
      await axiosClient.delete(`/api/users/me/addresses/${id}`);
      toast.success("Address deleted!");
      fetchAddresses();
    } catch (error) {
      console.error("Failed to delete address:", error);
      toast.error("Failed to delete address.");
    }
  };

  return {
    addressList,
    newAddress,
    setNewAddress,
    handleSaveAddress,
    handleDeleteAddress
  };
}
