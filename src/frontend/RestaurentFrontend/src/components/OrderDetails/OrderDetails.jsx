import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { toast } from "react-hot-toast";
import orderService from "../../services/orderService";
import { updateOrder } from "../../features/orders/orderSlice";
import {
  ORDER_STATUS,
  ORDER_STATUS_LABEL,
  ORDER_STATUS_FLOW,
  ORDER_STATUS_COLOR
} from "../../constants/orderStatus";

const getStatusValue = (status) => {
  if (typeof status === "number") return status;

  switch (status) {
    case "Pending": return ORDER_STATUS.PENDING;
    case "Preparing": return ORDER_STATUS.PREPARING;
    case "Confirmed": return ORDER_STATUS.CONFIRMED;
    case "Cancelled": return ORDER_STATUS.CANCELLED;
    case "Delivered": return ORDER_STATUS.DELIVERED;
    default: return null;
  }
};

const OrderDetails = () => {
  const { orderId } = useParams();
  const dispatch = useDispatch();

  const reduxOrder = useSelector((state) =>
    state.orders.orders.find((o) => o.orderId === orderId)
  );

  const [order, setOrder] = useState(reduxOrder || null);
  const [loading, setLoading] = useState(!reduxOrder);
  const [cancelLoading, setCancelLoading] = useState(false);

  useEffect(() => {
    if (!reduxOrder) {
      const fetchOrder = async () => {
        try {
          const response = await orderService.GetOrderById(orderId);
          setOrder(response);
          dispatch(updateOrder(response));
        } catch (err) {
          console.error(err);
        } finally {
          setLoading(false);
        }
      };

      fetchOrder();
    }
  }, [orderId, reduxOrder]);

  const handleCancelOrder = async () => {
    const confirmCancel = window.confirm(
      "Are you sure you want to cancel this order?"
    );

    if (!confirmCancel) return;

    try {
      setCancelLoading(true);
      const response = await orderService.CancelOrder(
        order.orderId,
        "Cancelled"
      );
      setOrder(response);
      dispatch(updateOrder(response));

      toast.success("Order cancelled successfully", {
        duration: 1800,
        style: {
          padding: "16px 20px",
          fontSize: "15px",
          fontWeight: "600",
          borderRadius: "12px",
        },
      });
    } catch (error) {
      console.error(error);
      toast.error("Unable to cancel order");
    } finally {
      setCancelLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center mt-20 space-y-4">
        <div className="w-10 h-10 border-4 border-gray-300 border-t-gray-600 rounded-full animate-spin"></div>
        <p className="text-gray-600 text-sm">Loading order details…</p>
      </div>
    );
  }

  if (!order) {
    return (
      <div className="flex flex-col items-center justify-center mt-20 space-y-3">
        <span className="text-5xl">📦</span>
        <h2 className="text-lg font-semibold text-gray-700">
          Order not found
        </h2>
        <p className="text-sm text-gray-500 text-center max-w-sm">
          The order you’re looking for doesn’t exist.
        </p>
      </div>
    );
  }

  const currentStatus = getStatusValue(order.orderStatus);
  const canCancel =
    order.orderStatus === "Pending" || order.orderStatus === "Preparing";

  return (
    <div className="bg-gray-50 pt-6 pb-6 px-4">
      <div className="max-w-3xl mx-auto">
        <div className="bg-white rounded-xl shadow-md border p-6 space-y-6">

          <div className="flex justify-between items-start">
            <div>
              <h2 className="text-xl font-semibold text-gray-900">
                Order #{order.orderId.slice(0, 8)}
              </h2>
              <p className="text-sm text-gray-500 mt-1">
                {new Date(order.orderDate).toLocaleString()}
              </p>
            </div>

            <span
              className={`px-3 py-1 rounded-full text-sm font-medium ${ORDER_STATUS_COLOR[currentStatus]}`}
            >
              {order.orderStatus}
            </span>
          </div>

          {currentStatus !== ORDER_STATUS.CANCELLED && (
            <div className="flex items-center gap-3">
              {ORDER_STATUS_FLOW.map((status, index) => {
                const isCurrent = status === currentStatus;
                const isCompleted = status < currentStatus;
                const isLast = index === ORDER_STATUS_FLOW.length - 1;

                let pillClass = "bg-gray-100 text-gray-400"; 

                if (isCurrent) {
                  pillClass = ORDER_STATUS_COLOR[status];
                } else if (isCompleted) {
                  pillClass = "bg-gray-200 text-gray-600"; 
                }

                return (
                  <div key={status} className="flex items-center">
                    <span
                      className={`px-3 py-1 rounded-full text-xs font-medium ${pillClass}`}
                    >
                      {ORDER_STATUS_LABEL[status]}
                    </span>

                    {!isLast && (
                      <div
                        className={`w-8 h-[2px] ${isCompleted || isCurrent
                            ? "bg-gray-400"
                            : "bg-gray-200"
                          }`}
                      />
                    )}
                  </div>
                );
              })}
            </div>
          )}

          <hr className="border-gray-300" />

          <div>
            <h3 className="text-sm font-semibold text-gray-700 mb-3">
              Items
            </h3>

            <div className="space-y-2">
              {order.orderItems.map((item) => (
                <div
                  key={item.dishId}
                  className="flex justify-between text-sm text-gray-800"
                >
                  <span>
                    {item.quantity} × {item.dishName}
                  </span>
                  <span className="font-medium">
                    ₹{item.totalPrice}
                  </span>
                </div>
              ))}
            </div>
          </div>

          <hr className="border-gray-300" />

          <div className="flex justify-between items-center">
            <span className="text-base font-semibold text-gray-900">
              Total Amount
            </span>
            <span className="text-lg font-bold text-gray-900">
              ₹{order.totalBill}
            </span>
          </div>

          {order.orderStatus !== "Cancelled" && order.deliveryAddress && (
            <>
              <hr className="border-gray-300" />
              <div>
                <h3 className="text-sm font-semibold text-gray-700 mb-2">
                  Delivery Address
                </h3>
                <p className="text-sm text-gray-600 leading-relaxed">
                  {order.deliveryAddress.addressLine}
                  {order.deliveryAddress.area && `, ${order.deliveryAddress.area}`}
                  {order.deliveryAddress.city && `, ${order.deliveryAddress.city}`}
                  {order.deliveryAddress.landmark &&
                    ` (${order.deliveryAddress.landmark})`}
                </p>
              </div>
            </>
          )}

          {canCancel && (
            <>
              <hr className="border-gray-300" />
              <div className="flex justify-end">
                <button
                  onClick={handleCancelOrder}
                  disabled={cancelLoading}
                  className="
                    px-4 py-2
                    border border-red-500
                    text-red-600
                    rounded-md
                    text-sm font-medium
                    hover:bg-red-50
                    disabled:opacity-50
                    disabled:cursor-not-allowed
                  "
                >
                  {cancelLoading ? "Cancelling..." : "Cancel Order"}
                </button>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default OrderDetails;