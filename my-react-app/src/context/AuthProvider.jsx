import { useMemo, useState, useCallback, useEffect } from "react";
import { AuthContext } from "./authContext";
import { jwtDecode } from "jwt-decode";
import userService from "../services/userService";

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
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
      return null;
    }
  };

  // Читаємо токен при старті
  const [user, setUser] = useState(() => {
    const token = localStorage.getItem("accessToken");
    return token ? decodeToken(token) : null;
  });

  const [loading, setLoading] = useState(false);

  // Експортуємо setAuth як login, щоб не ламати твій код в Login.jsx
  const setAuth = useCallback(async (token) => {
    localStorage.setItem("accessToken", token);
    const userData = decodeToken(token);
    setUser(userData);

    // Одразу підтягуємо повний профіль зі знижкою
    try {
      const fullProfile = await userService.getProfile();
      setUser((prev) => ({
        ...prev,
        name: fullProfile.firstName,
        lastName: fullProfile.lastName,
        phone: fullProfile.phoneNumber,
        photoUrl: fullProfile.photoUrl,
        discount: fullProfile.personalDiscountPercentage || 0,
      }));
    } catch (e) {
      console.error("Sync profile error:", e);
    }
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    setUser(null);
    window.location.href = "/login";
  }, []);

  // Синхронізація при старті та зміні мови/сесії
  useEffect(() => {
    const token = localStorage.getItem("accessToken");
    if (token) {
      const syncProfile = async () => {
        try {
          const fp = await userService.getProfile();
          setUser((prev) => ({
            ...prev,
            name: fp.firstName,
            lastName: fp.lastName,
            phone: fp.phoneNumber,
            photoUrl: fp.photoUrl,
            discount: fp.personalDiscountPercentage || 0,
          }));
        } catch (e) {
          console.error("Initial profile sync failed:", e);
        }
      };
      syncProfile();
    }
  }, []);

  const value = useMemo(
    () => ({ user, login: setAuth, setAuth, logout, loading }),
    [user, setAuth, logout, loading],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
