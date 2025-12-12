import { useMemo, useState, useCallback } from "react";
import { AuthContext } from "./authContext";

const STORAGE_KEY = "flowerlab_auth_user";

function readStoredUser() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    // ignore invalid JSON or access errors
    return null;
  }
}

export default function AuthProvider({ children }) {
  const [user, setUser] = useState(() => readStoredUser());

  const login = useCallback((payload) => {
    setUser(payload);
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(payload));
    } catch {
      // ignore storage write errors
    }
  }, []);

  const logout = useCallback(() => {
    setUser(null);
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch {
      // ignore storage remove errors
    }
  }, []);

  const value = useMemo(
    () => ({ user, login, logout }),
    [user, login, logout]
  );

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}
