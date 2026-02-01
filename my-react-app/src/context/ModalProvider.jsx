import React, { createContext, useContext, useState, useCallback } from "react";
import ConfirmModal from "../components/ConfirmModal/ConfirmModal";

const ModalContext = createContext();

export const useConfirm = () => useContext(ModalContext);

export default function ModalProvider({ children }) {
  const [modalState, setModalState] = useState({
    isOpen: false,
    title: "",
    message: "",
    confirmText: "Delete",
    cancelText: "Cancel",
    confirmType: "danger",
    onConfirm: () => {},
    onCancel: () => {},
  });

  const confirm = useCallback(
    ({
      title = "Are you sure?",
      message = "This action cannot be undone.",
      confirmText = "Delete",
      cancelText = "Cancel",
      confirmType = "danger",
      onConfirm,
      onCancel,
    }) => {
      setModalState({
        isOpen: true,
        title,
        message,
        confirmText,
        cancelText,
        confirmType,
        onConfirm: () => {
          if (onConfirm) onConfirm();
          close();
        },
        onCancel: () => {
          if (onCancel) onCancel();
          close();
        },
      });
    },
    [],
  );

  const close = useCallback(() => {
    setModalState((prev) => ({ ...prev, isOpen: false }));
  }, []);

  return (
    <ModalContext.Provider value={confirm}>
      {children}
      <ConfirmModal
        isOpen={modalState.isOpen}
        title={modalState.title}
        message={modalState.message}
        confirmText={modalState.confirmText}
        cancelText={modalState.cancelText}
        confirmType={modalState.confirmType}
        onConfirm={modalState.onConfirm}
        onCancel={modalState.onCancel}
      />
    </ModalContext.Provider>
  );
}
