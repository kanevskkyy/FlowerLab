import {
  useState,
  useEffect,
  useCallback,
  useMemo,
  useRef,
  useContext,
} from "react";
import { useTranslation } from "react-i18next";
import { toast } from "react-hot-toast";
import { CartContext } from "./CartContext";
import { AuthContext } from "./authContext";
import catalogService from "../services/catalogService";

export const CartProvider = ({ children }) => {
  const { t } = useTranslation();
  const { user } = useContext(AuthContext) || {};
  const [cartOpen, setCartOpen] = useState(false);

  // Helper to generate storage key
  const getCartKey = (u) => (u ? `cart_${u.id}` : "cart_guest");

  // Initial load function
  const loadCart = (u) => {
    try {
      const key = getCartKey(u);
      const saved = localStorage.getItem(key);
      return saved ? JSON.parse(saved) : [];
    } catch (error) {
      console.error("Failed to load cart from local storage", error);
      return [];
    }
  };

  const [cartItems, setCartItems] = useState(() => loadCart(user));

  // Track the current key to prevent overwriting new user's cart with old user's items during transition
  const currentKeyRef = useRef(getCartKey(user));

  // 1. Load cart when user changes
  useEffect(() => {
    const newItems = loadCart(user);
    setCartItems(newItems);
    currentKeyRef.current = getCartKey(user);
  }, [user]);

  // 2. Save cart when items change
  useEffect(() => {
    const key = getCartKey(user);

    // Safety check: ensure we are saving to the key matching the current user context
    // This prevents race conditions where old cart items might be saved to the new user's key
    if (currentKeyRef.current !== key) {
      currentKeyRef.current = key;
    }

    try {
      localStorage.setItem(key, JSON.stringify(cartItems));
    } catch (error) {
      console.error("Failed to save cart to local storage", error);
    }
  }, [cartItems, user]);

  // 3. Validate Cart Stock on Load
  useEffect(() => {
    const validateStock = async () => {
      if (cartItems.length === 0) return;

      let updatedItems = [...cartItems];
      let hasChanges = false;
      const messages = [];

      // We use a mapping to avoid fetching the same bouquet multiple times if multiple sizes are in cart
      // Though usually users buy 1 type. Parallel fetching is fine for now.

      await Promise.all(
        updatedItems.map(async (item, index) => {
          try {
            let maxStock = 0;
            let itemName = item.title;

            if (item.isGift || (!item.bouquetId && item.id)) {
              // It's a Gift
              // item.id is the giftId
              const giftId = item.id;
              const giftData = await catalogService.getGiftById(giftId);
              // Assuming backend returns object with availableCount or similar
              // In OrderService we saw AvailableCount. Let's assume standard casing from JSON: availableCount
              maxStock = giftData.availableCount;
              itemName = giftData.name;
            } else {
              // It's a Bouquet
              const bouquetId = item.bouquetId;
              const bouquetData =
                await catalogService.getBouquetById(bouquetId);

              const sizeObj = bouquetData.sizes.find(
                (s) => s.sizeId === item.sizeId || s.sizeName === item.sizeName,
              );

              if (sizeObj) {
                maxStock = sizeObj.maxAssemblableCount;
              } else {
                // Size no longer exists?
                maxStock = 0;
              }
              itemName = bouquetData.name;
            }

            // Update local maxStock knowledge
            if (updatedItems[index].maxStock !== maxStock) {
              updatedItems[index] = { ...updatedItems[index], maxStock };
              hasChanges = true; // Even if qty doesn't change, we want to update maxStock for future + button clicks
            }

            // Validate Quantity
            if (item.qty > maxStock) {
              if (maxStock === 0) {
                // Mark for deletion
                updatedItems[index] = null; // Will filter later
                messages.push(
                  t("toasts.cart_item_removed", { name: itemName }),
                );
              } else {
                updatedItems[index] = {
                  ...updatedItems[index],
                  qty: maxStock,
                  maxStock,
                };
                messages.push(
                  t("toasts.cart_qty_adjusted", {
                    name: itemName,
                    count: maxStock,
                  }),
                );
              }
              hasChanges = true;
            }
          } catch (error) {
            console.error(
              `Failed to validate stock for item ${item.id}`,
              error,
            );
            // If 404, maybe remove? For safety, leave as is or assume 0?
            // If item not found, safest is to assume 0 to prevent broken orders.
            if (error.response && error.response.status === 404) {
              updatedItems[index] = null;
              messages.push(
                t("toasts.cart_item_unavailable", {
                  name: item.title || "item",
                }),
              );
              hasChanges = true;
            }
          }
        }),
      );

      // Race Condition Check:
      // If the cart was cleared (e.g. by successful payment) while we were validating,
      // localStorage will be empty. We should NOT restore the items.
      // We need to check the CURRENT state in localStorage before applying updates.
      const currentKey = getCartKey(user);
      const currentStored = localStorage.getItem(currentKey);
      if (!currentStored || JSON.parse(currentStored).length === 0) {
        return;
      }

      if (hasChanges) {
        const finalItems = updatedItems.filter(Boolean);
        setCartItems(finalItems);
        // Toast messages
        messages.forEach((msg) => toast(msg, { icon: "⚠️" }));
      }
    };

    validateStock();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Run once on mount

  const addToCart = (product, openCart = true) => {
    if (cartItems.some((item) => item.id === product.id)) {
      toast.error(t("toasts.item_already_in_cart"));
      return false;
    }
    setCartItems((prev) => [...prev, product]);
    if (openCart) {
      setCartOpen(true);
    }
    return true;
  };

  const increaseQty = (id) => {
    setCartItems((prev) =>
      prev.map((item) =>
        item.id === id
          ? {
              ...item,
              qty: Math.min(item.maxStock || Infinity, (item.qty || 1) + 1),
            }
          : item,
      ),
    );
  };

  const decreaseQty = (id) => {
    setCartItems((prev) =>
      prev.map((item) =>
        item.id === id
          ? { ...item, qty: Math.max(1, (item.qty || 1) - 1) }
          : item,
      ),
    );
  };

  const removeItem = (id) => {
    setCartItems((prev) => prev.filter((item) => item.id !== id));
  };

  const clearCart = useCallback(() => {
    const key = getCartKey(user);
    // Immediately clear storage to prevent race conditions with loadCart
    localStorage.removeItem(key);
    setCartItems([]);
  }, [user]);

  const value = useMemo(
    () => ({
      cartItems,
      cartOpen,
      setCartOpen,
      addToCart,
      increaseQty,
      decreaseQty,
      removeItem,
      clearCart,
    }),
    [cartItems, cartOpen, clearCart],
  );

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
};
