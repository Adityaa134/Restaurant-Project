import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { ORDER_STATUS, ORDER_STATUS_COLOR } from "../../constants/orderStatus";

const getStatusValue = (status) => {
  if (typeof status === "number") return status;
  switch (status) {
    case "Pending":
      return ORDER_STATUS.PENDING;
    case "Preparing":
      return ORDER_STATUS.PREPARING;
    case "Confirmed":
      return ORDER_STATUS.CONFIRMED;
    case "Cancelled":
      return ORDER_STATUS.CANCELLED;
    case "Delivered":
      return ORDER_STATUS.DELIVERED;
    default:
      return null;
  }
};

const OrderCard = ({ order }) => {
  const [open, setOpen] = useState(false);
  const navigate = useNavigate();
  const currentStatus = getStatusValue(order.orderStatus);

  const preview = order.orderItems
    .slice(0, 2)
    .map((i) => i.dishName)
    .join(" · ");
  const extra =
    order.orderItems.length > 2
      ? ` · +${order.orderItems.length - 2} more`
      : "";

  return (
    <div className="order-card-acc bg-white rounded-2xl border border-gray-100 overflow-hidden transition-all duration-200 hover:border-gray-200 hover:shadow-sm">
      <button
        onClick={() => setOpen((p) => !p)}
        className="w-full flex items-center gap-3 px-4 py-4 sm:px-5 sm:py-4 text-left focus:outline-none group"
        aria-expanded={open}
      >
        <svg
          className={`w-4 h-4 flex-shrink-0 text-gray-400 transition-transform duration-200 ${open ? "rotate-180" : ""}`}
          viewBox="0 0 16 16"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M4 6l4 4 4-4"
            stroke="currentColor"
            strokeWidth="1.5"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>

        <div className="flex-1 min-w-0">
          <p className="text-[13px] font-medium text-gray-900 leading-tight">
            #{order.orderId.slice(0, 8)}
          </p>
          <p className="text-[12px] text-gray-400 mt-0.5 truncate">
            {preview}
            {extra}
          </p>
        </div>

        <div className="flex flex-col items-end gap-1 flex-shrink-0">
          <span className="text-[14px] font-semibold text-gray-900">
            ₹{order.totalBill}
          </span>
          <span
            className={`text-[11px] font-medium px-2 py-0.5 rounded-full ${ORDER_STATUS_COLOR[currentStatus]}`}
          >
            {order.orderStatus}
          </span>
        </div>
      </button>

      <div
        className={`transition-all duration-200 ease-in-out overflow-hidden ${
          open ? "max-h-[500px] opacity-100" : "max-h-0 opacity-0"
        }`}
      >
        <div className="border-t border-gray-100 px-4 pt-3 pb-4 sm:px-5">
          <div className="space-y-0 mb-3">
            {order.orderItems.map((item, idx) => (
              <div
                key={item.dishId}
                className={`flex items-center justify-between py-2.5 text-[13px] ${
                  idx !== order.orderItems.length - 1
                    ? "border-b border-gray-50"
                    : ""
                }`}
              >
                <div className="flex items-center gap-2">
                  <span className="text-[11px] font-medium bg-gray-100 text-gray-600 rounded px-1.5 py-0.5 min-w-[24px] text-center">
                    ×{item.quantity}
                  </span>
                  <span className="text-gray-800">{item.dishName}</span>
                </div>
              </div>
            ))}
          </div>

          <div className="flex items-center justify-between pt-3 border-t border-gray-100">
            <div className="flex items-center gap-1.5 text-[12px] text-gray-400">
              <svg
                className="w-3.5 h-3.5"
                viewBox="0 0 16 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <rect
                  x="2"
                  y="3"
                  width="12"
                  height="11"
                  rx="1.5"
                  stroke="currentColor"
                  strokeWidth="1.3"
                />
                <path
                  d="M5 1.5v3M11 1.5v3M2 7h12"
                  stroke="currentColor"
                  strokeWidth="1.3"
                  strokeLinecap="round"
                />
              </svg>
              {new Date(order.orderDate).toLocaleDateString("en-IN", {
                day: "numeric",
                month: "short",
                year: "numeric",
              })}
            </div>

            <button
              onClick={(e) => {
                e.stopPropagation();
                navigate(`/orders/${order.orderId}`);
              }}
              className="flex items-center gap-1.5 text-[12px] text-gray-500 border border-gray-200 rounded-lg px-3 py-1.5 hover:bg-gray-50 transition-colors"
            >
              <svg
                className="w-3.5 h-3.5"
                viewBox="0 0 16 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M8 2.5A5.5 5.5 0 1 1 2.5 8"
                  stroke="currentColor"
                  strokeWidth="1.4"
                  strokeLinecap="round"
                />
                <path
                  d="M8 2.5L5.5 5M8 2.5L10.5 5"
                  stroke="currentColor"
                  strokeWidth="1.4"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
              View details
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OrderCard;
