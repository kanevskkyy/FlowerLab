const FRIENDLY_ERROR_MESSAGES = {
  STOCK_INSUFFICIENT: "toasts.backend_errors.STOCK_INSUFFICIENT",
  GIFT_STOCK_INSUFFICIENT: "toasts.backend_errors.GIFT_STOCK_INSUFFICIENT",
  ORDER_ALREADY_PAID: "toasts.backend_errors.ORDER_ALREADY_PAID",
  INTERNAL_SERVER_ERROR: "toasts.backend_errors.INTERNAL_SERVER_ERROR",
  NOT_FOUND: "toasts.backend_errors.NOT_FOUND",
  ALREADY_EXISTS: "toasts.backend_errors.ALREADY_EXISTS",
  INVALID_GUEST_TOKEN: "toasts.backend_errors.INVALID_GUEST_TOKEN",
  DUPLICATE_GIFTS: "toasts.backend_errors.DUPLICATE_GIFTS",
  CATALOG_CHECK_ERROR: "toasts.backend_errors.CATALOG_CHECK_ERROR",
  INVALID_PRICE: "toasts.backend_errors.INVALID_PRICE",
  VALIDATION_ERROR: "toasts.backend_errors.VALIDATION_ERROR",
  INVALID_CREDENTIALS: "toasts.backend_errors.INVALID_CREDENTIALS",
  EMAIL_ALREADY_EXISTS: "toasts.backend_errors.EMAIL_ALREADY_EXISTS",
  EMAIL_NOT_CONFIRMED: "toasts.backend_errors.EMAIL_NOT_CONFIRMED",
  ACCOUNT_LOCKED: "toasts.backend_errors.ACCOUNT_LOCKED",
  USER_NOT_FOUND: "toasts.backend_errors.USER_NOT_FOUND",
};

/**
 * Tries to map a raw string or code to a known translation key.
 */
const tryMapToKey = (input) => {
  if (!input || typeof input !== "string") return null;

  // 1. Check direct code match
  if (FRIENDLY_ERROR_MESSAGES[input]) {
    return FRIENDLY_ERROR_MESSAGES[input];
  }

  // 2. Fuzzy string matching
  const lower = input.toLowerCase();
  if (lower.includes("invalid password or email"))
    return FRIENDLY_ERROR_MESSAGES.INVALID_CREDENTIALS;
  if (lower.includes("account is temporarily locked"))
    return FRIENDLY_ERROR_MESSAGES.ACCOUNT_LOCKED;
  if (lower.includes("does not exist"))
    return FRIENDLY_ERROR_MESSAGES.USER_NOT_FOUND;
  if (lower.includes("email is already taken"))
    return FRIENDLY_ERROR_MESSAGES.EMAIL_ALREADY_EXISTS;
  if (lower.includes("email not confirmed"))
    return FRIENDLY_ERROR_MESSAGES.EMAIL_NOT_CONFIRMED;

  return null;
};

export const extractErrorMessage = (
  error,
  defaultMessage = "Something went wrong",
) => {
  if (!error) return defaultMessage;

  const data = error.response?.data;

  // 1. Try mapping the code if it exists
  if (data?.code) {
    const mappedCode = tryMapToKey(data.code);
    if (mappedCode) return mappedCode;

    // Special case for validation errors
    if (data.code === "VALIDATION_ERROR" && data.errors) {
      // Logic for validation errors can go here if needed
    }
  }

  // 2. Try mapping message, error, detail, or title fields
  const potentialMessages = [
    data?.message,
    data?.error,
    data?.detail,
    data?.title,
  ];

  for (const msg of potentialMessages) {
    const mapped = tryMapToKey(msg);
    if (mapped) return mapped;
  }

  // 3. If no map found, return the first available raw string
  if (data?.message && typeof data.message === "string") return data.message;
  if (data?.error && typeof data.error === "string") return data.error;
  if (data?.detail && typeof data.detail === "string") return data.detail;
  if (data?.title && typeof data.title === "string") {
    // Fallback logic for complex title/errors object
    if (data.errors && typeof data.errors === "object") {
      const firstKey = Object.keys(data.errors)[0];
      const firstErr = data.errors[firstKey];
      if (Array.isArray(firstErr) && firstErr.length > 0) return firstErr[0];
      if (typeof firstErr === "string") return firstErr;
    }
    return data.title;
  }

  if (typeof data === "string" && data.length > 0) return data;

  if (error.message && typeof error.message === "string") return error.message;

  return defaultMessage;
};
