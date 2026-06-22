import { useState } from "react";
import { toast } from "react-hot-toast";
import orderService from "../../services/orderService";
import { StarRating } from "../index";

const DishRatingPanel = ({
  item,
  orderId,
  userId,
  existingRating,
  existingComment,
}) => {
  const [open, setOpen] = useState(false);
  const [rating, setRating] = useState(0);
  const [comment, setComment] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [rated, setRated] = useState(() => !!existingRating);
  const [submittedRating, setSubmittedRating] = useState(() =>
    existingRating ? Number(existingRating) : 0,
  );

  const handleSubmit = async () => {
    if (rating === 0) {
      toast.error("Please select a star rating");
      return;
    }
    try {
      setSubmitting(true);
      await orderService.SubmitRating({
        userId,
        dishId: item.dishId,
        orderId,
        rate: rating,
        comment: comment.trim() || null,
      });
      setRated(true);
      setSubmittedRating(rating);
      setOpen(false);
      toast.success(`Rating submitted for ${item.dishName}`, {
        duration: 1800,
        style: {
          padding: "16px 20px",
          fontSize: "15px",
          fontWeight: "600",
          borderRadius: "12px",
        },
      });
    } catch (err) {
      console.error(err);
      toast.error("Failed to submit rating. Please try again.");
    } finally {
      setSubmitting(false);
    }
  };

  const handleCancel = () => {
    setOpen(false);
    setRating(0);
    setComment("");
  };

  if (rated) {
    return (
      <div className="mt-2 ml-20 sm:ml-20 flex items-center gap-1.5">
        <span className="text-sm text-gray-500">You rated</span>
        <span className="text-sm font-semibold text-gray-800">
          {submittedRating}
        </span>
        <span className="text-base leading-none">⭐</span>
      </div>
    );
  }

  return (
    <div className="mt-2 ml-20 sm:ml-20">
      {!open ? (
        <button
          onClick={() => setOpen(true)}
          className="inline-flex items-center gap-1.5 text-xs px-3 py-1.5 rounded-full border border-gray-200 text-gray-500 hover:border-gray-300 hover:text-gray-700 transition-colors bg-white"
        >
          <span
            className="text-sm leading-none"
            style={{ filter: "grayscale(1) opacity(0.4)" }}
          >
            ⭐
          </span>
          Write a review
        </button>
      ) : (
        <div className="p-3 sm:p-4 bg-gray-50 rounded-xl border border-gray-200">
          <p className="text-xs text-gray-500 mb-3">
            How was{" "}
            <span className="font-medium text-gray-700">{item.dishName}</span>?{" "}
            <span className="text-gray-400">Tap to rate</span>
          </p>

          <StarRating value={rating} onChange={setRating} />

          <textarea
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            placeholder="Share what you liked or didn't like (optional)..."
            maxLength={500}
            rows={2}
            className="mt-3 w-full text-sm px-3 py-2 rounded-lg border border-gray-200 bg-white text-gray-800 resize-none focus:outline-none focus:ring-2 focus:ring-gray-300 placeholder-gray-400"
          />

          <div className="flex justify-end gap-2 mt-2">
            <button
              onClick={handleCancel}
              className="text-xs px-3 py-2 rounded-lg border border-gray-200 text-gray-500 hover:bg-gray-100 transition-colors"
            >
              Cancel
            </button>
            <button
              onClick={handleSubmit}
              disabled={submitting || rating === 0}
              className="text-xs px-4 py-2 rounded-lg bg-gray-900 text-white hover:bg-gray-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {submitting ? "Submitting..." : "Submit review"}
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default DishRatingPanel;
