import { useEffect, useState } from "react";
import { toast } from "react-hot-toast";
import orderService from "../../services/orderService";
import {
  ORDER_STATUS,
  ORDER_STATUS_LABEL,
  ORDER_STATUS_FLOW,
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
      console.log("Error in fetching orders", error)
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
      <div className="min-h-screen flex items-center justify-center">
        <div className="w-10 h-10 border-4 border-gray-300 border-t-black rounded-full animate-spin"></div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto p-6">
      <h1 className="text-2xl font-semibold mb-6">Manage Orders</h1>

      {!loading &&
        orders.map((order) => {
          const currentStatus = getStatusValue(order.orderStatus);
          const isDisabled =
            currentStatus === ORDER_STATUS.CANCELLED ||
            currentStatus === ORDER_STATUS.DELIVERED;

          return (
            <div
              key={order.orderId}
              className="bg-white border rounded-xl p-6 mb-5 shadow-sm hover:shadow-md transition"
            >

              <div className="flex justify-between items-start">
                <div className="space-y-1">
                  <p className="font-medium text-sm">
                    Order #{order.orderId}
                  </p>

                  <p className="text-xs text-gray-500">
                    User:{" "}
                    <span className="font-medium">
                      {order.userName ?? "N/A"}
                    </span>
                  </p>

                  <p className="text-xs text-gray-500">
                    Date:{" "}
                    {new Date(order.orderDate).toLocaleDateString()}
                  </p>

                  <span
                    className={`inline-block mt-2 px-3 py-1 rounded-full text-xs font-medium ${ORDER_STATUS_COLOR[currentStatus]}`}
                  >
                    {ORDER_STATUS_LABEL[currentStatus]}
                  </span>
                </div>

                <select
                  disabled={isDisabled}
                  className="border rounded-lg px-4 py-2 text-sm disabled:opacity-50"
                  defaultValue=""
                  onChange={(e) =>
                    handleStatusChange(
                      order.orderId,
                      currentStatus,
                      Number(e.target.value)
                    )
                  }
                >
                  <option value="" disabled>
                    {isDisabled ? "No Action" : "Update Status"}
                  </option>

                  {ORDER_STATUS_FLOW
                    .filter((status) => status > currentStatus)
                    .map((status) => (
                      <option key={status} value={status}>
                        {ORDER_STATUS_LABEL[status]}
                      </option>
                    ))}
                </select>
              </div>

              <div className="mt-4 space-y-1">
                {order.orderItems?.slice(0, 3).map((item) => (
                  <p
                    key={item.dishId}
                    className="text-sm text-gray-800"
                  >
                    {item.quantity} × {item.dishName}
                  </p>
                ))}

                {order.orderItems?.length > 3 && (
                  <p className="text-xs text-gray-500">
                    +{order.orderItems.length - 3} more items
                  </p>
                )}
              </div>

              <div className="mt-3 flex justify-end">
                <p className="text-sm font-semibold text-gray-900">
                  Total: ₹{order.totalBill}
                </p>
              </div>
            </div>
          );
        })}

      {pagination && (
        <div className="flex justify-center items-center gap-2 mt-8">
          <button
            disabled={!pagination.hasPrevious}
            onClick={() => setPage((p) => p - 1)}
            className="px-3 py-2 border rounded disabled:opacity-40"
          >
            ‹
          </button>

          {Array.from(
            { length: pagination.totalPages },
            (_, i) => i + 1
          ).map((p) => (
            <button
              key={p}
              onClick={() => setPage(p)}
              className={`px-3 py-2 rounded border ${p === page
                  ? "bg-blue-600 text-white"
                  : "hover:bg-gray-100"
                }`}
            >
              {p}
            </button>
          ))}

          <button
            disabled={!pagination.hasNext}
            onClick={() => setPage((p) => p + 1)}
            className="px-3 py-2 border rounded disabled:opacity-40"
          >
            ›
          </button>
        </div>
      )}
    </div>
  );
}

export default ManageOrders;