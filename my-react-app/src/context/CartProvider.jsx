import { useState, useEffect, useCallback, useMemo, useRef } from "react";
import { CartContext } from "./CartContext";
import { useAuth } from "./useAuth";

export const CartProvider = ({ children }) => {
  const { user } = useAuth();
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

  const addToCart = (product, openCart = true) => {
    setCartItems((prev) => [...prev, product]);
    if (openCart) {
      setCartOpen(true);
    }
  };

  const increaseQty = (id) => {
    setCartItems((prev) =>
      prev.map((item) =>
        item.id === id ? { ...item, qty: (item.qty || 1) + 1 } : item,
      ),
    );
  };

  const decreaseQty = (id) => {
    setCartItems((prev) =>
      prev
        .map((item) =>
          item.id === id
            ? { ...item, qty: Math.max(1, (item.qty || 1) - 1) }
            : item,
        )
        .filter((item) => item.qty > 0),
    );
  };

  const removeItem = (id) => {
    setCartItems((prev) => prev.filter((item) => item.id !== id));
  };

  const clearCart = useCallback(() => {
    setCartItems([]);
  }, []);

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
