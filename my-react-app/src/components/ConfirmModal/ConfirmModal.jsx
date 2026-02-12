import React, { useEffect } from "react";
import "./ConfirmModal.css";

export default function ConfirmModal({
  isOpen,
  title = "Are you sure?",
  message = "This action cannot be undone.",
  onConfirm,
  onCancel,
  confirmText = "Delete",
  cancelText = "Cancel",
  confirmType = "danger", // danger | nice
}) {
  // Close on Escape - FIX: Must be called before conditional return
  useEffect(() => {
    if (!isOpen) return;
    const handleEsc = (e) => {
      if (e.key === "Escape") onCancel();
    };
    window.addEventListener("keydown", handleEsc);
    return () => window.removeEventListener("keydown", handleEsc);
  }, [isOpen, onCancel]);

  if (!isOpen) return null;

  return (
    <div className="cm-overlay" onClick={onCancel}>
      <div className="cm-modal" onClick={(e) => e.stopPropagation()}>
        <h3 className="cm-title">{title}</h3>
        <p className="cm-text">{message}</p>
        <div className="cm-actions">
          <button className="cm-btn cm-btn-cancel" onClick={onCancel}>
            {cancelText}
          </button>
          <button
            className={`cm-btn cm-btn-${confirmType}`}
            onClick={onConfirm}>
            {confirmText}
          </button>
        </div>
      </div>
    </div>
  );
}
