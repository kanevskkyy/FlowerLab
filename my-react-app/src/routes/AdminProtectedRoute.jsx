import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../context/useAuth";

export default function AdminProtectedRoute({ children }) {
  const { user, loading } = useAuth();
  const location = useLocation();

  if (loading) {
    return (
      <div
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "100vh",
        }}>
        Loading...
      </div>
    );
  }

  // 1. Not logged in -> Redirect to Login
  if (!user) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // 2. Check Role (Assuming role is stored in user.role)
  // Adapt this check depending on how your backend returns roles (string vs array)
  const role = user.role || "";
  const isAdmin =
    role === "Admin" || (Array.isArray(role) && role.includes("Admin"));

  // 3. Not Admin -> Redirect to Home
  if (!isAdmin) {
    return <Navigate to="/" replace />;
  }

  // 4. Authorized -> Render content
  return children;
}
