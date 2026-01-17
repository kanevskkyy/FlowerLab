import { useState } from "react";
import toast from "react-hot-toast";
import testphoto from "../../../assets/images/testphoto.jpg";

export function useAdminReviews() {
  const [pendingReviews, setPendingReviews] = useState([
    {
      id: 1,
      name: "Anna Shevchenko",
      stars: 5,
      text: "Such a pretty bouquet! Will buy again ^^",
      avatar: testphoto,
    },
  ]);

  const handlePostReview = (id) => {
    setPendingReviews((prev) => prev.filter((r) => r.id !== id));
    toast.success("Review posted successfully!");
  };

  const handleDeleteReview = (id) => {
    setPendingReviews((prev) => prev.filter((r) => r.id !== id));
    toast.success("Review deleted");
  };

  return {
    pendingReviews,
    handlePostReview,
    handleDeleteReview
  };
}
