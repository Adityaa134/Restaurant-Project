import { useEffect, useState } from "react";
import { toast } from "react-hot-toast";
import orderService from "../../services/orderService";
import {
  ORDER_STATUS,
  ORDER_STATUS_LABEL,
  ORDER_STATUS_FLOW,
  ORDER_STATUS_COLOR,
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

function ManageOrders() {
  const [orders, setOrders] = useState([]);
  const [page, setPage] = useState(1);
  const [pagination, setPagination] = useState(null);
  const [loading, setLoading] = useState(false);

  const fetchOrders = async () => {
    setLoading(true);
    try {
      const response = await orderService.GetOrders(page);
      setOrders(response.items);
      setPagination(response);
    } catch (error) {
      console.log("Error in fetching orders", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, [page]);

  const handleStatusChange = async (orderId, currentStatus, newStatus) => {
    if (newStatus <= currentStatus) return;
    if (currentStatus === ORDER_STATUS.CANCELLED) return;
    if (currentStatus === ORDER_STATUS.DELIVERED) return;

    try {
      await orderService.UpdateOrderStatus(orderId, newStatus);
      toast.success("Order status updated successfully", {
        duration: 1800,
        style: {
          padding: "16px 22px",
          fontSize: "16px",
          fontWeight: "600",
          borderRadius: "14px",
        },
        iconTheme: {
          primary: "#16a34a",
          secondary: "#ffffff",
        },
      });
      fetchOrders();
    } catch (error) {
      console.error(error);
      toast.error("Failed to update order status");
    }
  };

  if (loading) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-6 md:px-6 animate-pulse">
        <div className="mb-8">
          <div className="h-10 w-64 bg-gray-200 rounded mb-3"></div>
          <div className="h-5 w-80 max-w-full bg-gray-200 rounded"></div>
        </div>

        {[1, 2, 3, 4].map((item) => (
          <div
            key={item}
            className="
          bg-white
          border
          border-gray-200
          rounded-2xl
          shadow-sm
          mb-5
          overflow-hidden
          "
          >
            <div
              className="
            grid
            grid-cols-1
            md:grid-cols-3
            "
            >
              <div className="p-5 md:p-6 border-b md:border-b-0 md:border-r border-gray-100">
                <div className="h-4 w-16 bg-gray-200 rounded mb-3"></div>

                <div className="h-8 w-full bg-gray-200 rounded mb-6"></div>

                <div className="h-4 w-10 bg-gray-200 rounded mb-2"></div>
                <div className="h-5 w-32 bg-gray-200 rounded mb-5"></div>

                <div className="h-4 w-10 bg-gray-200 rounded mb-2"></div>
                <div className="h-5 w-40 bg-gray-200 rounded"></div>
              </div>
              <div className="p-5 md:p-6 border-b md:border-b-0 md:border-r border-gray-100">
                <div className="h-4 w-20 bg-gray-200 rounded mb-4"></div>

                <div className="space-y-3">
                  <div className="h-4 w-48 bg-gray-200 rounded"></div>
                  <div className="h-4 w-40 bg-gray-200 rounded"></div>
                  <div className="h-4 w-36 bg-gray-200 rounded"></div>
                  <div className="h-4 w-24 bg-gray-200 rounded"></div>
                </div>
              </div>

              <div className="p-5 md:p-6">
                <div className="h-4 w-24 bg-gray-200 rounded mb-3"></div>

                <div className="h-10 w-28 bg-gray-200 rounded mb-5"></div>

                <div className="h-8 w-28 bg-gray-200 rounded-full mb-6"></div>

                <div className="h-4 w-24 bg-gray-200 rounded mb-2"></div>

                <div className="h-11 w-full bg-gray-200 rounded-xl"></div>
              </div>
            </div>
          </div>
        ))}

        <div className="flex justify-center items-center gap-3 mt-10">
          <div className="w-11 h-11 rounded-xl bg-gray-200"></div>
          <div className="w-11 h-11 rounded-xl bg-gray-200"></div>
          <div className="w-11 h-11 rounded-xl bg-gray-200"></div>
          <div className="w-11 h-11 rounded-xl bg-gray-200"></div>
          <div className="w-11 h-11 rounded-xl bg-gray-200"></div>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 py-6 md:px-6">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Manage Orders</h1>

        <p className="text-gray-500 mt-1 text-sm">
          Monitor and update customer orders.
        </p>
      </div>

      {!loading &&
        orders.map((order) => {
          const currentStatus = getStatusValue(order.orderStatus);
          const isDisabled =
            currentStatus === ORDER_STATUS.CANCELLED ||
            currentStatus === ORDER_STATUS.DELIVERED;

          return (
            <div
              key={order.orderId}
              className="
  bg-white
  border border-gray-200
  rounded-2xl
  shadow-sm
  mb-5
  overflow-hidden
  "
            >
              <div
                className="
    grid
    grid-cols-1
    md:grid-cols-3
    "
              >
                <div className="p-4 md:p-6 border-b md:border-b-0 md:border-r border-gray-100">
                  <p className="text-xs text-gray-400 font-medium">Order ID</p>

                  <p className="font-bold text-lg text-gray-900 break-words">
                    #{order.orderId}
                  </p>

                  <div className="mt-2 md:mt-4">
                    <p className="text-xs text-gray-400">User</p>

                    <p className="font-medium text-gray-800">
                      {order.userName ?? "N/A"}
                    </p>
                  </div>

                  <div className="mt-2 md:mt-4">
                    <p className="text-xs text-gray-400">Date</p>

                    <p className="text-sm text-gray-600">
                      {new Date(order.orderDate).toLocaleString()}
                    </p>
                  </div>
                </div>

                <div className="p-4 md:p-6 border-b md:border-b-0 md:border-r border-gray-100">
                  <p className="text-xs text-gray-400 font-medium mb-2 md:mb-3">
                    Items ({order.orderItems?.length || 0})
                  </p>

                  {order.orderItems?.slice(0, 3).map((item) => (
                    <p
                      key={item.dishId}
                      className="text-sm text-gray-800 mb-1 md:mb-2"
                    >
                      {item.quantity} × {item.dishName}
                    </p>
                  ))}

                  {order.orderItems?.length > 3 && (
                    <p className="text-sm text-blue-600 font-medium mt-1">
                      +{order.orderItems.length - 3} more items
                    </p>
                  )}
                </div>

                <div className="p-4 md:p-6">
                  <div className="flex items-center justify-between mb-4">
                    <div>
                      <p className="text-xs text-gray-400 font-medium">
                        Total Amount
                      </p>

                      <p className="text-xl md:text-2xl font-bold text-gray-900 mt-1">
                        ₹{order.totalBill}
                      </p>
                    </div>

                    <span
                      className={`inline-flex px-3 py-1 rounded-full text-xs font-medium ${ORDER_STATUS_COLOR[currentStatus]}`}
                    >
                      {ORDER_STATUS_LABEL[currentStatus]}
                    </span>
                  </div>

                  <div className="mt-3 md:mt-5">
                    <p className="text-xs text-gray-500 mb-2">Update Status</p>

                    <select
                      disabled={isDisabled}
                      className="
          w-full
          border
          border-gray-300
          rounded-xl
          px-3
          py-2.5
          text-sm
          bg-white
          focus:outline-none
          focus:ring-2
          focus:ring-blue-100
          disabled:opacity-50
          disabled:cursor-not-allowed
          "
                      defaultValue=""
                      onChange={(e) =>
                        handleStatusChange(
                          order.orderId,
                          currentStatus,
                          Number(e.target.value),
                        )
                      }
                    >
                      <option value="" disabled>
                        {isDisabled ? "No Action" : "Update Status"}
                      </option>

                      {ORDER_STATUS_FLOW.filter(
                        (status) => status > currentStatus,
                      ).map((status) => (
                        <option key={status} value={status}>
                          {ORDER_STATUS_LABEL[status]}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>
              </div>
            </div>
          );
        })}

      {pagination && (
        <div className="flex justify-center items-center gap-3 mt-10 mb-4">
          <button
            disabled={!pagination.hasPrevious}
            onClick={() => setPage((p) => p - 1)}
            className="
            w-11 h-11
            rounded-xl
            border border-gray-200
            bg-white
            shadow-sm
            hover:bg-gray-50
            hover:border-gray-300
            transition-all duration-200
            disabled:opacity-40
            disabled:cursor-not-allowed
            "
          >
            ‹
          </button>

          {Array.from({ length: pagination.totalPages }, (_, i) => i + 1).map(
            (p) => (
              <button
                key={p}
                onClick={() => setPage(p)}
                className={`w-11 h-11
                rounded-xl
                border
                font-medium
                transition-all duration-200
                ${
                  p === page
                    ? "bg-blue-600 text-white border-blue-600 shadow-md"
                    : "bg-white border-gray-200 hover:bg-gray-50 hover:border-gray-300"
                }`}
              >
                {p}
              </button>
            ),
          )}

          <button
            disabled={!pagination.hasNext}
            onClick={() => setPage((p) => p + 1)}
            className="
            w-11 h-11
            rounded-xl
            border border-gray-200
            bg-white
            shadow-sm
            hover:bg-gray-50
            hover:border-gray-300
            transition-all duration-200
            disabled:opacity-40
            disabled:cursor-not-allowed
            "
          >
            ›
          </button>
        </div>
      )}
    </div>
  );
}

export default ManageOrders;
