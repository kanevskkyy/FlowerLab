import { useState, useEffect, useCallback } from "react";
import orderService from "../../../services/orderService";

export function useOrders(activeTab, TABS) {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [sort, setSort] = useState("DateDesc");
  const [isLoadingMore, setIsLoadingMore] = useState(false);
  const [pagination, setPagination] = useState({
    pageNumber: 1,
    pageSize: 5,
    totalCount: 0,
    totalPages: 1,
  });

  const fetchOrders = useCallback(
    async (isLoadMore = false) => {
      const pageToFetch = isLoadMore ? pagination.pageNumber : 1;

      if (isLoadMore) {
        setIsLoadingMore(true);
      } else {
        setLoading(true);
      }

      try {
        const data = await orderService.getMyOrders({
          PageNumber: pageToFetch,
          PageSize: pagination.pageSize,
          Sort: sort,
        });

        const mappedOrders = (data.items || []).map((order) => {
          const bouquets = (order.items || []).map((item) => ({
            id: item.id,
            title: item.bouquetName,
            qty: `${item.count} pc`,
            img: item.bouquetImage || null,
            price: item.price,
            size: item.sizeName,
            type: "bouquet",
          }));

          const gifts = (order.orderGifts || order.gifts || []).map((g) => ({
            id: g.giftId,
            title: g.gift?.name || "Gift",
            qty: `${g.orderedCount || g.count} pc`,
            img: g.gift?.imageUrls?.[0] || g.gift?.imageUrl || null,
            price: g.gift?.price || 0,
            type: "gift",
          }));

          const allItems = [...bouquets, ...gifts];

          return {
            id: `№${order.id.substring(0, 8).toUpperCase()}`,
            rawId: order.id,
            date: new Date(order.createdAt).toLocaleString("uk-UA", {
              hour: "2-digit",
              minute: "2-digit",
              day: "numeric",
              month: "numeric",
              year: "numeric",
            }),
            type: allItems.length > 1 ? "multi" : "single",
            status: order.status.name,
            total: order.totalPrice,
            currency: "₴",
            items: allItems,
            cardText: order.giftMessage,
          };
        });

        if (isLoadMore) {
          setOrders((prev) => {
            const existingIds = new Set(prev.map((o) => o.rawId));
            const newUniqueOrders = mappedOrders.filter(
              (o) => !existingIds.has(o.rawId),
            );
            return [...prev, ...newUniqueOrders];
          });
        } else {
          setOrders(mappedOrders);
        }

        setPagination((prev) => ({
          ...prev,
          totalCount: data.totalCount || 0,
          totalPages: data.totalPages || 1,
        }));
      } catch (error) {
        console.error("Failed to fetch orders:", error);
      } finally {
        setLoading(false);
        setIsLoadingMore(false);
      }
    },
    [pagination.pageNumber, pagination.pageSize, sort],
  );

  // Sync pagination reset when sort changes
  useEffect(() => {
    setPagination((p) => ({ ...p, pageNumber: 1 }));
  }, [sort]);

  useEffect(() => {
    if (activeTab === TABS.ORDERS) {
      fetchOrders(pagination.pageNumber > 1);
    }
  }, [activeTab, fetchOrders, TABS.ORDERS, pagination.pageNumber]);

  const loadMore = useCallback(() => {
    if (
      pagination.pageNumber < pagination.totalPages &&
      !loading &&
      !isLoadingMore
    ) {
      setPagination((prev) => ({ ...prev, pageNumber: prev.pageNumber + 1 }));
    }
  }, [pagination.pageNumber, pagination.totalPages, loading, isLoadingMore]);

  return {
    orders,
    ordersLoading: loading,
    isLoadingMore,
    hasNextPage: pagination.pageNumber < pagination.totalPages,
    loadMore,
    sort,
    setSort,
  };
}
