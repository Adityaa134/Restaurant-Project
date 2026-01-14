import { useNavigate } from "react-router-dom";
import {
  ORDER_STATUS,
  ORDER_STATUS_COLOR
} from "../../constants/orderStatus";

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
  const navigate = useNavigate();

  const currentStatus = getStatusValue(order.orderStatus);

  return (
    <div
      onClick={() => navigate(`/orders/${order.orderId}`)}
      className="
        bg-white rounded-xl border shadow-sm
        hover:shadow-md hover:border-gray-300
        transition cursor-pointer
        p-5 space-y-4
      "
    >
      <div className="flex justify-between items-start">
        <div>
          <p className="text-sm text-gray-600 font-medium">
            Order #{order.orderId.slice(0, 8)}
          </p>
        </div>

        <span
          className={`px-3 py-1 rounded-full text-xs font-medium ${
            ORDER_STATUS_COLOR[currentStatus]
          }`}
        >
          {order.orderStatus}
        </span>
      </div>

      <div className="space-y-1">
        {order.orderItems.slice(0, 3).map((item) => (
          <p key={item.dishId} className="text-sm text-gray-800">
            {item.quantity} × {item.dishName}
          </p>
        ))}

        {order.orderItems.length > 3 && (
          <p className="text-xs text-gray-500">
            +{order.orderItems.length - 3} more items
          </p>
        )}
      </div>

      <hr className="border-gray-300 border-t-[1.5px]" />

      <div className="flex justify-between items-start">
        <div>
          <p className="text-xs text-gray-500">Order placed on</p>
          <p className="text-sm text-gray-700">
            {new Date(order.orderDate).toLocaleDateString()}
          </p>
        </div>

        <p className="text-base font-semibold text-gray-900">
          ₹{order.totalBill}
        </p>
      </div>
    </div>
  );
};

export default OrderCard;