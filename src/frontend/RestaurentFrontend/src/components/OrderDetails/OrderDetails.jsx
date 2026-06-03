import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { toast } from "react-hot-toast";
import orderService from "../../services/orderService";
import { updateOrder } from "../../features/orders/orderSlice";
import { setCartItems } from "../../features/cart/cartSlice";
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

const OrderDetails = () => {
  const { orderId } = useParams();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const reduxOrder = useSelector((state) =>
    state.orders.orders.find((o) => o.orderId === orderId),
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
      "Are you sure you want to cancel this order?",
    );

    if (!confirmCancel) return;

    try {
      setCancelLoading(true);
      const response = await orderService.CancelOrder(
        order.orderId,
        "Cancelled",
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

  const handleReorder = () => {
    const reorderedItems = order.orderItems.map((item) => ({
      dishId: item.dishId,
      dishName: item.dishName,
      quantity: item.quantity,
      dishPrice: item.unitPrice,
      dish_Image_Path: item.dishImagePath,
    }));

    dispatch(setCartItems(reorderedItems));
    navigate("/checkout");
  };

  if (loading) {
    return (
      <div className="bg-gray-50 pt-6 pb-4 sm:pb-6 px-4 animate-pulse">
        <div className="max-w-2xl mx-auto px-4 py-4 sm:py-6">
          <div className="bg-white rounded-3xl shadow-sm border border-gray-200 overflow-hidden">
            <div className="p-5 sm:p-7 border-b border-gray-200">
              <div className="flex items-start justify-between gap-4">
                <div className="space-y-3">
                  <div className="h-8 w-52 bg-gray-200 rounded-lg"></div>
                  <div className="h-4 w-40 bg-gray-200 rounded"></div>
                </div>

                <div className="h-12 w-36 bg-gray-200 rounded-full"></div>
              </div>
            </div>

            <div className="px-5 sm:px-7 py-5 border-b border-gray-200">
              <div className="h-6 w-48 bg-gray-200 rounded mb-4"></div>

              <div className="space-y-2">
                <div className="h-4 w-full bg-gray-200 rounded"></div>
                <div className="h-4 w-3/4 bg-gray-200 rounded"></div>
              </div>
            </div>

            <div className="px-5 sm:px-7 py-5 border-b border-gray-200">
              <div className="h-6 w-24 bg-gray-200 rounded mb-6"></div>

              <div className="flex items-center gap-4">
                <div className="w-16 h-16 rounded-xl bg-gray-200 flex-shrink-0"></div>

                <div className="flex-1 space-y-3">
                  <div className="h-5 w-40 bg-gray-200 rounded"></div>
                  <div className="h-4 w-20 bg-gray-200 rounded"></div>
                </div>

                <div className="h-5 w-16 bg-gray-200 rounded"></div>
              </div>
            </div>

            <div className="px-5 sm:px-7 py-5 border-b border-gray-200 space-y-4">
              <div className="flex justify-between">
                <div className="h-4 w-24 bg-gray-200 rounded"></div>
                <div className="h-4 w-16 bg-gray-200 rounded"></div>
              </div>

              <div className="flex justify-between">
                <div className="h-4 w-28 bg-gray-200 rounded"></div>
                <div className="h-4 w-14 bg-gray-200 rounded"></div>
              </div>

              <div className="border-t border-gray-200 pt-4 flex justify-between">
                <div className="h-6 w-20 bg-gray-200 rounded"></div>
                <div className="h-6 w-16 bg-gray-200 rounded"></div>
              </div>
            </div>

            <div className="p-5 sm:p-7">
              <div className="h-14 w-full bg-gray-200 rounded-2xl"></div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (!order) {
    return (
      <div className="flex flex-col items-center justify-center mt-20 space-y-3">
        <span className="text-5xl">📦</span>
        <h2 className="text-lg font-semibold text-gray-700">Order not found</h2>
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
    <div className="bg-gray-50 pt-6 pb-4 sm:pb-6 px-4">
      <div className="max-w-2xl mx-auto px-4 py-4 sm:py-6">
        <div
          className="
      bg-white
      rounded-3xl
      shadow-sm
      border border-gray-200
      overflow-hidden
    "
        >
          <div className="p-5 sm:p-7 border-b border-gray-200">
            <div className="flex items-start justify-between gap-4">
              <div>
                <h1 className="text-2xl sm:text-3xl font-bold text-gray-900">
                  Order #{order.orderId.slice(0, 8)}
                </h1>

                <p className="text-sm sm:text-base text-gray-500 mt-1">
                  {new Date(order.orderDate).toLocaleString()}
                </p>
              </div>

              <span
                className={`
              inline-flex items-center gap-2
              px-4 py-2 rounded-full
              text-sm font-semibold
              shadow-sm
              ${ORDER_STATUS_COLOR[currentStatus]}
              `}
              >
                <span className="w-2 h-2 rounded-full bg-current opacity-70"></span>
                {order.orderStatus}
              </span>
            </div>
          </div>

          <div className="px-5 sm:px-7 py-5 border-b border-gray-200">
            <h2 className="text-lg font-semibold text-gray-900 mb-3">
              Delivery Address
            </h2>

            <p className="text-gray-600 leading-relaxed">
              {order.deliveryAddress?.addressLine}

              {order.deliveryAddress?.area && `, ${order.deliveryAddress.area}`}

              {order.deliveryAddress?.city && `, ${order.deliveryAddress.city}`}

              {order.deliveryAddress?.landmark &&
                ` (${order.deliveryAddress.landmark})`}
            </p>
          </div>

          <div className="px-5 sm:px-7 py-5 border-b border-gray-200">
            <h2 className="text-lg font-semibold text-gray-900 mb-5">Items</h2>

            <div className="space-y-5">
              {order.orderItems.map((item) => (
                <div key={item.dishId} className="flex items-center gap-4">
                  <img
                    src={item.dishImagePath}
                    alt={item.dishName}
                    className="
                  w-16 h-16
                  rounded-xl
                  object-cover
                  border border-gray-200
                  flex-shrink-0
                "
                  />
                  <div className="flex-1 min-w-0">
                    <h3 className="font-medium text-gray-900 truncate">
                      {item.dishName}
                    </h3>

                    <p className="text-sm text-gray-500 mt-1">
                      Qty: {item.quantity}
                    </p>
                  </div>

                  <p className="font-semibold text-gray-900">
                    ₹{item.totalPrice}
                  </p>
                </div>
              ))}
            </div>
          </div>
          <div className="px-5 sm:px-7 py-5 border-b border-gray-200">
            <div className="space-y-3">
              <div className="flex justify-between text-gray-600">
                <span>Subtotal</span>
                <span>₹{order.totalBill}</span>
              </div>

              <div className="flex justify-between text-gray-600">
                <span>Delivery Fee</span>
                <span>Free</span>
              </div>

              <div className="border-t border-gray-200 pt-3 flex justify-between">
                <span className="text-lg font-bold text-gray-900">Total</span>

                <span className="text-xl font-bold text-gray-900">
                  ₹{order.totalBill}
                </span>
              </div>
            </div>
          </div>

          <div className="p-5 sm:p-7">
            {order.orderStatus === "Delivered" && (
              <button
                onClick={handleReorder}
                className="
                w-full
                bg-blue-600
                hover:bg-blue-700
                text-white
                font-semibold
                py-3 rounded-2xl
                transition
                flex items-center justify-center gap-2
              "
              >
                ↻ Reorder
              </button>
            )}

            {(order.orderStatus === "Pending" ||
              order.orderStatus === "Preparing") && (
              <button
                onClick={handleCancelOrder}
                className="
              w-full
              bg-red-600
              hover:bg-red-700
              text-white
              font-semibold
              py-3 rounded-2xl
              transition
            "
              >
                Cancel Order
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default OrderDetails;
