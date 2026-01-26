import { useState } from "react";
import { toast } from "react-hot-toast";
import axiosClient from "../../../api/axiosClient";
import { useAuth } from "../../../context/useAuth";

export function usePersonalSettings() {
  const { user, login } = useAuth();

  const [form, setForm] = useState({
    firstName: user?.name || "",
    lastName: user?.lastName || "",
    phone: user?.phone || "",
    email: user?.email || "youremail@gmail.com",
    photoUrl: user?.photoUrl || "",
  });

  const [selectedFile, setSelectedFile] = useState(null);
  const [photoPreview, setPhotoPreview] = useState(user?.photoUrl || "");

  const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
  const [passwordForm, setPasswordForm] = useState({
    oldPassword: "",
    newPassword: "",
    confirmPassword: "",
  });

  const onChange = (key) => (e) =>
    setForm((p) => ({ ...p, [key]: e.target.value }));

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setSelectedFile(file);
      setPhotoPreview(URL.createObjectURL(file));
    }
  };

  const handleProfileUpdate = async () => {
    try {
      const formData = new FormData();
      formData.append("FirstName", form.firstName);
      formData.append("LastName", form.lastName);

      const cleanPhone = form.phone.replace(/[\s-]/g, "");
      formData.append("PhoneNumber", cleanPhone);

      if (selectedFile) {
        formData.append("Photo", selectedFile);
      }

      const response = await axiosClient.put("/api/users/me", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      const newToken = response.data.token || response.data.accessToken;
      if (newToken) {
        await login(newToken);
      }

      toast.success("Профіль успішно оновлено! ✨");
    } catch (error) {
      console.error("Failed to update profile:", error);
      const responseData = error.response?.data;
      if (responseData?.errors) {
        if (Array.isArray(responseData.errors)) {
          const firstErr = responseData.errors[0];
          toast.error(
            firstErr?.error || firstErr?.Error || "Помилка валідації",
          );
        } else if (typeof responseData.errors === "object") {
          const firstKey = Object.keys(responseData.errors)[0];
          const firstMsg = responseData.errors[firstKey][0];
          toast.error(`${firstKey}: ${firstMsg}`);
        }
      } else {
        toast.error(
          responseData?.error ||
            responseData?.message ||
            "Не вдалося оновити профіль",
        );
      }
    }
  };

  const onPasswordChange = (key) => (e) =>
    setPasswordForm((p) => ({ ...p, [key]: e.target.value }));

  const handlePasswordChange = async (e) => {
    e.preventDefault();
    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      toast.error("Нові паролі не співпадають!");
      return;
    }

    try {
      await axiosClient.put("/api/users/me/password", {
        oldPassword: passwordForm.oldPassword,
        newPassword: passwordForm.newPassword,
        confirmPassword: passwordForm.confirmPassword,
      });

      toast.success("Пароль успішно змінено! 🔐");
      setIsPasswordModalOpen(false);
      setPasswordForm({
        oldPassword: "",
        newPassword: "",
        confirmPassword: "",
      });
    } catch (error) {
      console.error("Failed to change password:", error);
      toast.error(error.response?.data?.message || "Не вдалося змінити пароль");
    }
  };

  return {
    form,
    photoPreview,
    selectedFile,
    onChange,
    handleFileChange,
    handleProfileUpdate,

    // Password
    isPasswordModalOpen,
    setIsPasswordModalOpen,
    passwordForm,
    onPasswordChange,
    handlePasswordChange,
  };
}
