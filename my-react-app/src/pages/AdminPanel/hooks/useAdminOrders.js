import { useState, useMemo } from "react";
import toast from "react-hot-toast";

// Images (in real app, data comes from API, images usually urls)
import testphoto from "../../../assets/images/testphoto.jpg";
import bouquet1L from "../../../assets/images/bouquet1L.jpg";

export function useAdminOrders() {
  const [orders, setOrders] = useState([
    {
      id: 1001,
      title: "Bouquet Orchids",
      qty: "1 pc",
      customer: "Oleh Vynnyk",
      date: "25.10.25 10:06",
      total: "1000 ₴",
      avatar: testphoto,
      status: "New",
    },
    {
      id: 1002,
      title: "Bouquet 101 Roses",
      qty: "1 pc",
      customer: "Tina Karol",
      date: "24.10.25 14:30",
      total: "5500 ₴",
      avatar: bouquet1L,
      status: "Processing",
    },
  ]);

  const [sort, setSort] = useState("new");

  const handleStatusChange = (id, newStatus) => {
    setOrders((prev) =>
      prev.map((order) =>
        order.id === id ? { ...order, status: newStatus } : order
      )
    );
    if (newStatus === "Cancelled") {
      toast.error(`Order #${id} marked as Cancelled`);
    } else if (newStatus === "Delivered") {
      toast.success(`Order #${id} delivered! 🎉`);
    } else {
      toast.success(`Order #${id} is now ${newStatus}`);
    }
  };

  const sortedOrders = useMemo(() => {
    const sorted = [...orders];
    if (sort === "old") {
      return sorted.reverse();
    }
    return sorted;
  }, [orders, sort]);

  return {
    orders: sortedOrders,
    sort,
    setSort,
    handleStatusChange
  };
}
