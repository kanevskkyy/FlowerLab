import { useState, useEffect, useCallback } from "react";
import toast from "react-hot-toast";
import orderService from "../../../services/orderService";
import { extractErrorMessage } from "../../../utils/errorUtils";

export function useAdminOrders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 1,
  });

  const [sort, setSort] = useState("new"); // "new" (desc date) or "old" (asc date)

  // For Detail Modal (legacy/unused now but good to keep clean)
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [isDetailOpen, setIsDetailOpen] = useState(false);
  const [detailLoading, setDetailLoading] = useState(false);

  // Statuses list
  const [statuses, setStatuses] = useState([]);

  const fetchOrders = useCallback(
    async (isLoadMore = false) => {
      // If loading first page -> full load. If loading more -> background load
      if (isLoadMore) {
        setDetailLoading(true); // Reusing this or add specific state
      } else {
        setLoading(true);
      }

      try {
        let sortParam = "DateDesc";
        // Map sort keys to API params
        switch (sort) {
          case "date-asc":
            sortParam = "DateAsc";
            break;
          case "qty-desc":
            sortParam = "QtyDesc";
            break;
          case "qty-asc":
            sortParam = "QtyAsc";
            break;
          case "name-asc":
            sortParam = "NameAsc";
            break;
          case "name-desc":
            sortParam = "NameDesc";
            break;
          case "date-desc":
          default:
            sortParam = "DateDesc";
            break;
        }

        // Fetch orders AND statuses in parallel if statuses empty
        const promises = [
          orderService.getAll({
            pageNumber: pagination.pageNumber,
            pageSize: pagination.pageSize,
            sort: sortParam,
          }),
        ];

        const shouldFetchStatuses = statuses.length === 0;
        if (shouldFetchStatuses) {
          promises.push(orderService.getStatuses());
        }

        const results = await Promise.all(promises);
        const data = results[0];
        if (shouldFetchStatuses) {
          setStatuses(results[1]);
        }

        // Map API response
        const mappedOrders = (data.items || []).map((order) => ({
          id: order.id,
          title:
            order.items?.length > 1
              ? `${order.items[0].bouquetName} + ${order.items.length - 1} more`
              : order.items?.[0]?.bouquetName || "Unknown Item",
          qty:
            order.items?.length > 1
              ? `${order.items.reduce((acc, i) => acc + i.count, 0)} items`
              : `${order.items?.[0]?.count || 1} pc`,
          customer:
            `${order.userFirstName || order.firstName || ""} ${order.userLastName || order.lastName || ""}`.trim() ||
            "Guest",
          date: new Date(
            order.status?.createdAt || order.createdAt || Date.now(),
          ).toLocaleString("uk-UA", {
            day: "numeric",
            month: "numeric",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit",
          }),
          total: `${order.totalPrice} ₴`,
          avatar:
            order.userPhotoUrl ||
            "https://res.cloudinary.com/dg9clyn4k/image/upload/v1763712578/order-service/gifts/mpfiss97mfebcqwm6elb.jpg",
          status: order.status, // KEEP OBJECT {id, name}
          rawDate: new Date(
            order.status?.createdAt || order.createdAt || Date.now(),
          ),
        }));

        // If LoadMore -> Append, else -> Replace
        if (isLoadMore) {
          setOrders((prev) => [...prev, ...mappedOrders]);
        } else {
          setOrders(mappedOrders);
        }

        setPagination((prev) => ({
          ...prev,
          totalCount: data.totalCount || 0,
          totalPages: Math.ceil((data.totalCount || 0) / prev.pageSize),
        }));
      } catch (error) {
        console.error("Failed to fetch orders:", error);
        toast.error(extractErrorMessage(error, "Failed to load orders"));
      } finally {
        setLoading(false);
        setDetailLoading(false);
      }
    },
    [pagination.pageNumber, pagination.pageSize, sort, statuses.length],
  );

  // Initial Fetch (only when sort changes or first mount)
  useEffect(() => {
    // Reset to page 1 when sort changes
    setPagination((p) => ({ ...p, pageNumber: 1 }));
  }, [sort]);

  // Trigger fetch when pageNumber changes
  useEffect(() => {
    fetchOrders(pagination.pageNumber > 1);
  }, [fetchOrders, pagination.pageNumber]);

  const loadMore = () => {
    if (
      pagination.pageNumber < pagination.totalPages &&
      !loading &&
      !detailLoading
    ) {
      setPagination((prev) => ({ ...prev, pageNumber: prev.pageNumber + 1 }));
    }
  };

  const handleStatusChange = async (id, newStatusId) => {
    const originalOrders = [...orders];
    const targetStatus = statuses.find((s) => s.id === newStatusId);

    if (!targetStatus) return;

    // Optimistic update
    setOrders((prev) =>
      prev.map((order) =>
        order.id === id ? { ...order, status: targetStatus } : order,
      ),
    );

    try {
      await orderService.updateStatus(id, newStatusId);

      if (targetStatus.name === "Cancelled") {
        toast.error(`Order status updated to Cancelled`);
      } else if (targetStatus.name === "Delivered") {
        toast.success(`Order delivered! 🎉`);
      } else {
        toast.success(`Order status updated to ${targetStatus.name}`);
      }
    } catch (error) {
      console.error("Failed to update status:", error);
      toast.error(extractErrorMessage(error, "Failed to update status"));
      // Revert on failure
      setOrders(originalOrders);
    }
  };

  const handleOrderClick = (id) => {
    // Logic moved to navigation in component
  };

  const closeDetail = () => {};

  return {
    orders,
    statuses, // Expose statuses
    loading,
    sort,
    setSort,
    pagination,
    setPagination,
    handleStatusChange,
    handleOrderClick,

    selectedOrder,
    isDetailOpen,
    detailLoading,
    closeDetail,
    loadMore,
    hasNextPage: pagination.pageNumber < pagination.totalPages,
    isLoadingMore: detailLoading,
  };
}
