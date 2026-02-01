const FRIENDLY_ERROR_MESSAGES = {
  STOCK_INSUFFICIENT: "Вибачте, цей букет вже встигли забронювати.",
  GIFT_STOCK_INSUFFICIENT:
    "На жаль, на вибрану кількість подарунків не вистачає на складі.",
  ORDER_ALREADY_PAID: "Це замовлення вже оплачено.",
  INTERNAL_SERVER_ERROR: "Сталася технічна помилка. Спробуйте пізніше.",
  NOT_FOUND: "Замовлення або товар не знайдено.",
  ALREADY_EXISTS: "Такий запис уже існує.",
  INVALID_GUEST_TOKEN: "Помилка авторизації гостьового замовлення.",
  DUPLICATE_GIFTS:
    "Не можна додавати однакові подарунки декілька разів як окремі позиції.",
  CATALOG_CHECK_ERROR:
    "Помилка при перевірці наявності букетів. Спробуйте ще раз.",
  INVALID_PRICE: "Помилка ціноутворення. Зверніться в підтримку.",
  VALIDATION_ERROR: "Будь ласка, перевірте правильність заповнення полів.",
};

export const extractErrorMessage = (
  error,
  defaultMessage = "Something went wrong",
) => {
  if (!error) return defaultMessage;
  if (error.response?.status === 401) return null;

  const data = error.response?.data;

  if (data?.code) {
    if (data.code === "VALIDATION_ERROR" && data.errors) {
    } else if (FRIENDLY_ERROR_MESSAGES[data.code]) {
      return FRIENDLY_ERROR_MESSAGES[data.code];
    } else if (data.message && typeof data.message === "string") {
      return data.message;
    }
  }

  if (data?.message && typeof data.message === "string") {
    return data.message;
  }

  if (data?.error && typeof data.error === "string") {
    return data.error;
  }

  if (data?.detail && typeof data.detail === "string") {
    return data.detail;
  }

  if (data?.title && typeof data.title === "string") {
    if (data.errors && typeof data.errors === "object") {
      const firstKey = Object.keys(data.errors)[0];
      const firstErr = data.errors[firstKey];
      if (Array.isArray(firstErr) && firstErr.length > 0) {
        return firstErr[0];
      }
      if (typeof firstErr === "string" && firstErr.length > 0) {
        return firstErr;
      }
    }
    if (
      !data.title.toLowerCase().includes("validation errors") ||
      !data.errors
    ) {
      return data.title;
    }
  }

  if (typeof data === "string" && data.length > 0) {
    return data;
  }

  if (data?.errors && typeof data.errors === "object") {
    const errorValues = Object.values(data.errors).flat();
    const firstStringErr = errorValues.find((e) => typeof e === "string");
    if (firstStringErr) return firstStringErr;
  }

  if (error.message && typeof error.message === "string") {
    return error.message;
  }

  return defaultMessage;
};
