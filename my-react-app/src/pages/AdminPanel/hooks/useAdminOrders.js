import { useState, useEffect, useCallback } from "react";
import toast from "react-hot-toast";
import { useTranslation } from "react-i18next";
import orderService from "../../../services/orderService";
import { extractErrorMessage } from "../../../utils/errorUtils";
import {
  getLocalizedValue,
  getLocalizedStatus,
} from "../../../utils/localizationUtils";
import { useDebounce } from "../../../hooks/useDebounce";

export function useAdminOrders() {
  const { t, i18n } = useTranslation();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [q, setQ] = useState("");
  const debouncedSearchTerm = useDebounce(q, 500);

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
            SearchTerm: debouncedSearchTerm,
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

        const currentLang = (i18n.language || "ua")
          .toLowerCase()
          .startsWith("u")
          ? "ua"
          : "en";
        const dateLocale = (i18n.language || "ua").toLowerCase().startsWith("u")
          ? "uk-UA"
          : "en-US";

        // Map API response
        const mappedOrders = (data.items || []).map((order) => {
          const firstItem = order.items?.[0];
          const bouquetName =
            getLocalizedValue(firstItem?.bouquetName, i18n.language) ||
            "Unknown Item";

          return {
            id: order.id,
            title:
              order.items?.length > 1
                ? `${bouquetName} + ${order.items.length - 1} ${t("admin.orders.more")}`
                : bouquetName,
            qty:
              order.items?.length > 1
                ? `${order.items.reduce((acc, i) => acc + i.count, 0)} ${t("admin.orders.items_count")}`
                : `${order.items?.[0]?.count || 1} ${t("admin.orders.item_pc")}`,
            customer:
              `${order.userFirstName || order.firstName || ""} ${order.userLastName || order.lastName || ""}`.trim() ||
              t("admin.orders.guest"),
            date: new Date(
              order.status?.createdAt || order.createdAt || Date.now(),
            ).toLocaleString(dateLocale, {
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
          };
        });

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
        toast.error(
          extractErrorMessage(error, t("toasts.admin_orders_load_failed")),
        );
      } finally {
        setLoading(false);
        setDetailLoading(false);
      }
    },
    [
      pagination.pageNumber,
      pagination.pageSize,
      sort,
      debouncedSearchTerm,
      statuses.length,
      i18n.language,
      t,
    ],
  );

  // Initial Fetch (only when sort/language/search changes or first mount)
  useEffect(() => {
    // Reset to page 1 when sort, language, or search changes
    setPagination((p) => ({ ...p, pageNumber: 1 }));
    setOrders([]); // Clear existing
  }, [sort, i18n.language, debouncedSearchTerm]);

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
        toast.error(t("toasts.admin_order_cancelled"));
      } else if (targetStatus.name === "Delivered") {
        toast.success(t("toasts.admin_order_delivered"));
      } else {
        const localizedStatus = getLocalizedStatus(
          targetStatus,
          i18n.language,
          t,
        );
        toast.success(
          t("toasts.admin_status_updated", { status: localizedStatus }),
        );
      }
    } catch (error) {
      console.error("Failed to update status:", error);
      toast.error(
        extractErrorMessage(error, t("toasts.admin_status_update_failed")),
      );
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
    q,
    setQ,
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
