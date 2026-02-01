import { useState, useEffect } from "react";
import { toast } from "react-hot-toast";
import axiosClient from "../../../api/axiosClient";
import { useAuth } from "../../../context/useAuth";
import { useTranslation } from "react-i18next";
import userService from "../../../services/userService";

export function usePersonalSettings() {
  const { t } = useTranslation();
  const { user, login } = useAuth();

  const [form, setForm] = useState({
    firstName: user?.name || "",
    lastName: user?.lastName || "",
    phone: user?.phone || "",
    email: user?.email || "youremail@gmail.com",
    photoUrl: user?.photoUrl || "",
    discount: user?.discount || 0,
  });

  const [selectedFile, setSelectedFile] = useState(null);
  const [photoPreview, setPhotoPreview] = useState(user?.photoUrl || "");
  const [formErrors, setFormErrors] = useState({});

  // Sync with context if user data changes (e.g. after background refetch in AuthProvider)
  useEffect(() => {
    if (user) {
      setForm((prev) => ({
        ...prev,
        firstName: user.name || "",
        lastName: user.lastName || "",
        phone: user.phone || "",
        email: user.email || prev.email,
        photoUrl: user.photoUrl || "",
        discount: user.discount || 0,
      }));
      setPhotoPreview(user.photoUrl || "");
    }
  }, [user]);

  const validateProfile = (data) => {
    const errs = {};
    if (!data.firstName?.trim())
      errs.firstName = t("cabinet.error_first_name_required");
    if (!data.lastName?.trim())
      errs.lastName = t("cabinet.error_last_name_required");

    const cleanPhone = data.phone?.replace(/[\s-]/g, "") || "";
    if (!cleanPhone) {
      errs.phone = t("cabinet.error_phone_required");
    } else if (!/^\+380\d{9}$/.test(cleanPhone)) {
      errs.phone = t("cabinet.error_phone_invalid");
    }

    setFormErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [passwordForm, setPasswordForm] = useState({
    oldPassword: "",
    newPassword: "",
    confirmPassword: "",
  });
  const [passwordErrors, setPasswordErrors] = useState({});

  const { logout } = useAuth();

  const handleAccountDelete = async () => {
    try {
      await userService.deleteAccount();
      toast.success(t("toasts.account_deleted") || "Акаунт видалено");
      logout();
    } catch (error) {
      console.error("Failed to delete account:", error);
      toast.error(
        t("toasts.account_delete_failed") || "Помилка при видаленні акаунту",
      );
    }
  };

  const validatePassword = (form) => {
    const errs = {};
    if (!form.oldPassword)
      errs.oldPassword = t("cabinet.error_old_password_required");
    if (!form.newPassword) {
      errs.newPassword = t("cabinet.error_new_password_required");
    } else if (form.newPassword.length < 8) {
      errs.newPassword = t("cabinet.error_password_min_length");
    }
    if (form.confirmPassword !== form.newPassword) {
      errs.confirmPassword = t("cabinet.error_passwords_mismatch");
    }
    setPasswordErrors(errs);
    return Object.keys(errs).length === 0;
  };

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
    if (!validateProfile(form)) {
      return;
    }

    try {
      const formData = new FormData();
      formData.append("FirstName", form.firstName);
      formData.append("LastName", form.lastName);

      const cleanPhone = form.phone.replace(/[\s-]/g, "");
      formData.append("PhoneNumber", cleanPhone);

      if (selectedFile) {
        formData.append("Photo", selectedFile);
      }

      const response = await userService.updateProfile(formData, true);

      const newToken = response.token || response.accessToken;
      if (newToken) {
        await login(newToken);
      }

      toast.success(t("toasts.profile_updated"));
    } catch (error) {
      console.error("Failed to update profile:", error);
      const responseData = error.response?.data;
      if (responseData?.errors) {
        if (Array.isArray(responseData.errors)) {
          const firstErr = responseData.errors[0];
          toast.error(
            firstErr?.error || firstErr?.Error || t("toasts.validation_error"),
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
            t("toasts.profile_update_failed"),
        );
      }
    }
  };

  const onPasswordChange = (key) => (e) =>
    setPasswordForm((p) => ({ ...p, [key]: e.target.value }));

  const handlePasswordChange = async (e) => {
    e.preventDefault();
    if (!validatePassword(passwordForm)) {
      return;
    }

    try {
      await userService.changePassword({
        oldPassword: passwordForm.oldPassword,
        newPassword: passwordForm.newPassword,
        confirmPassword: passwordForm.confirmPassword,
      });

      toast.success(t("toasts.password_changed"));
      setIsPasswordModalOpen(false);
      setPasswordForm({
        oldPassword: "",
        newPassword: "",
        confirmPassword: "",
      });
      setPasswordErrors({});
    } catch (error) {
      console.error("Failed to change password:", error);
      const msg =
        error.response?.data?.message || t("toasts.password_change_failed");
      toast.error(msg);

      // If it's a validation error from server, we could potentially map it,
      // but for now, the toast is sufficient for the "Old password incorrect" case.
    }
  };

  return {
    form,
    formErrors,
    photoPreview,
    selectedFile,
    onChange,
    handleFileChange,
    handleProfileUpdate,

    // Password
    isPasswordModalOpen,
    setIsPasswordModalOpen,
    passwordForm,
    passwordErrors,
    onPasswordChange,
    handlePasswordChange,

    // Delete Account
    isDeleteModalOpen,
    setIsDeleteModalOpen,
    handleAccountDelete,
  };
}
