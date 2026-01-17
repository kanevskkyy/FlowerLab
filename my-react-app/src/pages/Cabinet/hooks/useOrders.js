import { useState, useEffect } from "react";
import axiosClient from "../../../api/axiosClient";

export function useOrders(activeTab, TABS) {
  const [orders, setOrders] = useState([]);
  const [ordersLoading, setOrdersLoading] = useState(false);

  const fetchOrders = async () => {
    setOrdersLoading(true);
    try {
      const { data } = await axiosClient.get("/api/orders/my?pageNumber=1&pageSize=100");
      
      const mappedOrders = data.items.map(order => ({
        id: `№${order.id.substring(0, 8).toUpperCase()}`,
        date: new Date(order.createdAt).toLocaleString('uk-UA', { 
           hour: '2-digit', minute: '2-digit', day: 'numeric', month: 'numeric', year: 'numeric' 
        }), 
        type: order.items.length > 1 ? "multi" : "single",
        status: order.status.name,
        total: order.totalPrice,
        currency: "₴",
        items: order.items.map(item => ({
           title: item.bouquetName,
           qty: `${item.count} pc`,
           img: item.bouquetImage,
           price: item.price
        }))
      }));
      setOrders(mappedOrders);
    } catch (error) {
      console.error("Failed to fetch orders:", error);
    } finally {
      setOrdersLoading(false);
    }
  };

  useEffect(() => {
    if (activeTab === TABS.ORDERS) fetchOrders();
  }, [activeTab]);

  return {
    orders,
    ordersLoading
  };
}
