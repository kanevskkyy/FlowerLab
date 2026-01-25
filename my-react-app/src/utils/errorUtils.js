/**
 * Extracts a human-readable error message from an API error response.
 * Handles standard problem details, custom error objects, and validation dictionaries.
 *
 * @param {any} error The error object (usually from axios catch block)
 * @param {string} defaultMessage Fallback message if nothing specific is found
 * @returns {string} The extracted error message
 */
export const extractErrorMessage = (
  error,
  defaultMessage = "Something went wrong",
) => {
  if (!error) return defaultMessage;

  const data = error.response?.data;

  // 1. Explicit message field
  if (data?.message && typeof data.message === "string") {
    return data.message;
  }

  // 2. Problem Detail 'detail' field
  if (data?.detail && typeof data.detail === "string") {
    return data.detail;
  }

  // 3. Problem Detail 'title' field
  if (data?.title && typeof data.title === "string") {
    // If it's a validation error, try to extract specific field error first
    if (data.errors && typeof data.errors === "object") {
      const firstKey = Object.keys(data.errors)[0];
      const firstErr = data.errors[firstKey];
      if (Array.isArray(firstErr) && firstErr.length > 0) {
        return firstErr[0];
      }
      if (typeof firstErr === "string") {
        return firstErr;
      }
    }
    return data.title;
  }

  // 4. Raw string response
  if (typeof data === "string" && data.length > 0) {
    return data;
  }

  // 5. Validation error dictionary (older format or ASP.NET Core default)
  if (data?.errors && typeof data.errors === "object") {
    const firstKey = Object.keys(data.errors)[0];
    const firstErr = data.errors[firstKey];
    if (Array.isArray(firstErr) && firstErr.length > 0) {
      return firstErr[0];
    }
  }

  // 6. Generic Axios error message (e.g. Network Error)
  if (error.message && typeof error.message === "string") {
    return error.message;
  }

  return defaultMessage;
};
