import { useState, useEffect, useCallback } from "react";
import { useDebounce } from "../../../hooks/useDebounce";
import adminService from "../../../services/adminService";

export function useAdminUsers(activeTab) {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [q, setQ] = useState("");
  const [page, setPage] = useState(1);
  const [hasNextPage, setHasNextPage] = useState(false);
  const [isLoadingMore, setIsLoadingMore] = useState(false);

  const debouncedSearchTerm = useDebounce(q, 500);

  const fetchUsers = useCallback(
    async (pageNum = 1, isLoadMore = false) => {
      console.log("fetchUsers called", {
        pageNum,
        isLoadMore,
        q: debouncedSearchTerm,
        activeTab,
      });
      if (activeTab !== "users") return;

      if (isLoadMore) setIsLoadingMore(true);
      else setLoading(true);

      try {
        const params = {
          SearchTerm: debouncedSearchTerm,
          PageNumber: pageNum,
          PageSize: 10,
        };
        console.log("Requesting users with params:", params);
        const data = await adminService.getUsers(params);
        console.log("Received users data:", data);

        let newItems = [];
        let hasNext = false;

        if (Array.isArray(data)) {
          newItems = data;
          hasNext = false; // Array return means no paging from backend
        } else if (data && Array.isArray(data.items)) {
          newItems = data.items;
          hasNext = data.hasNextPage || false;
        }

        if (isLoadMore) {
          setUsers((prev) => [...prev, ...newItems]);
        } else {
          setUsers(newItems);
        }

        setHasNextPage(hasNext);
        setPage(pageNum);
      } catch (error) {
        console.error("Error fetching users:", error);
      } finally {
        setLoading(false);
        setIsLoadingMore(false);
      }
    },
    [debouncedSearchTerm, activeTab],
  );

  useEffect(() => {
    if (activeTab === "users") {
      fetchUsers(1, false);
    }
  }, [fetchUsers, activeTab]);

  const loadMore = () => {
    if (hasNextPage && !isLoadingMore) {
      fetchUsers(page + 1, true);
    }
  };

  const handleUpdateDiscount = async (userId, discount) => {
    try {
      await adminService.updateUserDiscount(userId, discount);
      setUsers((prev) =>
        prev.map((u) =>
          u.id === userId ? { ...u, personalDiscountPercentage: discount } : u,
        ),
      );
      return true;
    } catch (error) {
      console.error("Error updating discount:", error);
      return false;
    }
  };

  return {
    users,
    loading,
    q,
    setQ,
    loadMore,
    hasNextPage,
    isLoadingMore,
    handleUpdateDiscount,
  };
}
