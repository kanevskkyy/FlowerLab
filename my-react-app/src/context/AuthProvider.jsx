import { useMemo, useState, useCallback } from "react";
import { AuthContext } from "./authContext";
import { jwtDecode } from "jwt-decode";

export default function AuthProvider({ children }) {
  // Допоміжна функція
  const decodeToken = (token) => {
    try {
      const decoded = jwtDecode(token);
      return {
        email:
          decoded.email ||
          decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
          ],
        id:
          decoded.sub ||
          decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
          ],
        role:
          decoded.role ||
          decoded[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
          ],
        // New fields
        name:
          decoded.given_name ||
          decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"
          ] ||
          "",
        lastName:
          decoded.family_name ||
          decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"
          ] ||
          "",
        phone:
          decoded.phone_number ||
          decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone"
          ] ||
          "",
      };
    } catch (e) {
      console.error("Invalid token:", e);
      return null;
    }
  };

  // Читаємо токен при старті
  const [user, setUser] = useState(() => {
    const token = localStorage.getItem("accessToken");
    return token ? decodeToken(token) : null;
  });

  const [loading, setLoading] = useState(false);

  // Цю функцію викликає Login.jsx після успішного запиту
  const setAuth = useCallback((token) => {
    const userData = decodeToken(token);
    setUser(userData);
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    setUser(null);
    window.location.href = "/login";
  }, []);

  // Експортуємо setAuth як login, щоб не ламати твій код в Login.jsx
  const value = useMemo(
    () => ({ user, login: setAuth, setAuth, logout, loading }),
    [user, setAuth, logout, loading]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
