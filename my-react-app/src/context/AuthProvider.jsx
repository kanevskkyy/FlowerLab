import { useMemo, useState, useCallback, useEffect } from "react";
import { AuthContext } from "./authContext";
import { jwtDecode } from "jwt-decode";
import userService from "../services/userService";
import axiosClient from "../api/axiosClient";

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
        photoUrl: decoded.PhotoUrl || "",
        discount: parseInt(decoded.Discount) || 0,
      };
    } catch (e) {
      console.error("Invalid token:", e);
      return null;
    }
  };

  // Читаємо токен при старті - ТЕПЕР ПУСТО, чекаємо API
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // Експортуємо setAuth як login, щоб не ламати твій код в Login.jsx
  const setAuth = useCallback(
    async (token) => {
      // Ми більше не зберігаємо токен у localStorage вручну (він у Cookies)
      const userData = token ? decodeToken(token) : null;
      setUser(userData);

      if (!token) return;

      // Одразу підтягуємо повний профіль зі знижкою
      try {
        const fullProfile = await userService.getProfile();
        setUser((prev) => ({
          ...prev,
          ...userData, // Впевнюємося що маємо ID та Email
          name: fullProfile.firstName,
          lastName: fullProfile.lastName,
          phone: fullProfile.phoneNumber,
          photoUrl: fullProfile.photoUrl,
          discount: fullProfile.personalDiscountPercentage || 0,
          role: Array.isArray(fullProfile.roles)
            ? fullProfile.roles[0] || ""
            : fullProfile.roles || "",
        }));
      } catch (e) {
        console.error("Sync profile error (non-fatal):", e);
      }
    },
    [decodeToken],
  );

  const logout = useCallback(async () => {
    try {
      // Кличемо бекенд щоб він затер кукі
      await axiosClient.post("/api/auth/logout");
    } catch (e) {
      console.error("Logout error:", e);
    }

    setUser(null);
    window.location.href = "/login";
  }, []);

  // Синхронізація при старті
  useEffect(() => {
    const checkAuth = async () => {
      setLoading(true);
      try {
        // Ми додаємо skipAuthRedirect: true, щоб axios не викидав гостя на сторінку логіну
        const fp = await userService.getProfile({ skipAuthRedirect: true });
        setUser({
          id: fp.id,
          email: fp.email,
          name: fp.firstName,
          lastName: fp.lastName,
          phone: fp.phoneNumber,
          photoUrl: fp.photoUrl,
          discount: fp.personalDiscountPercentage || 0,
          role: Array.isArray(fp.roles) ? fp.roles[0] || "" : fp.roles || "",
        });
      } catch (e) {
        setUser(null);
      } finally {
        setLoading(false);
      }
    };
    checkAuth();
  }, []);

  const value = useMemo(
    () => ({ user, login: setAuth, setAuth, logout, loading }),
    [user, setAuth, logout, loading],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
