import { useState } from "react";

const StarRating = ({ value, onChange }) => {
  const [hovered, setHovered] = useState(0);

  return (
    <div className="flex gap-1">
      {[1, 2, 3, 4, 5].map((star) => {
        const isActive = star <= (hovered || value);
        return (
          <button
            key={star}
            type="button"
            onClick={() => onChange(star)}
            onMouseEnter={() => setHovered(star)}
            onMouseLeave={() => setHovered(0)}
            aria-label={`Rate ${star} star${star > 1 ? "s" : ""}`}
            className="focus:outline-none border-none bg-transparent p-0 select-none"
            style={{
              fontSize: "clamp(18px, 4vw, 24px)",
              lineHeight: 1,
              cursor: "pointer",
              filter: isActive
                ? "drop-shadow(0 1px 4px rgba(245,158,11,0.45))"
                : "grayscale(1) opacity(0.3)",
              transform: isActive ? "scale(1.12)" : "scale(1)",
              transition: "transform 0.12s ease, filter 0.12s ease",
            }}
          >
            ⭐
          </button>
        );
      })}
    </div>
  );
};

export default StarRating;
