import { useEffect, useState } from "react";
import orderService from "../../services/orderService";
import OrderCard from "../OrderCard/OrderCard";
import { useSelector, useDispatch } from "react-redux";
import { setOrders } from "../../features/orders/orderSlice";
import { ORDER_STATUS } from "../../constants/orderStatus";

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

const FILTERS = [
  { label: "All orders", value: "all" },
  { label: "Delivered", value: "delivered" },
  { label: "Cancelled", value: "cancelled" },
  { label: "In progress", value: "In progress" },
];

const StatCard = ({ value, label, color = "text-gray-900" }) => (
  <div className="bg-white rounded-xl border border-gray-100 px-5 py-4">
    <p className={`text-2xl font-semibold ${color}`}>{value}</p>
    <p className="text-xs text-gray-400 mt-1">{label}</p>
  </div>
);

const getMostOrdered = (orders) => {
  const freq = {};
  orders.forEach((o) =>
    o.orderItems?.forEach((i) => {
      freq[i.dishName] = (freq[i.dishName] || 0) + 1;
    }),
  );
  const entries = Object.entries(freq).sort((a, b) => b[1] - a[1]);
  return entries.length > 0
    ? { name: entries[0][0], count: entries[0][1] }
    : null;
};

const UserOrders = () => {
  const userId = useSelector((state) => state.auth.userData?.userId);
  const orders = useSelector((state) => state.orders.orders);
  const [loading, setLoading] = useState(true);
  const [activeFilter, setActiveFilter] = useState("all");
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        setLoading(true);
        const response = await orderService.GetOrdersByUserId(userId);
        dispatch(setOrders(Array.isArray(response) ? response : []));
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };
    fetchOrders();
  }, [userId]);

  const delivered = orders.filter(
    (o) => getStatusValue(o.orderStatus) === ORDER_STATUS.DELIVERED,
  ).length;
  const cancelled = orders.filter(
    (o) => getStatusValue(o.orderStatus) === ORDER_STATUS.CANCELLED,
  ).length;
  const pending = orders.filter((o) => {
    const s = getStatusValue(o.orderStatus);
    return (
      s === ORDER_STATUS.PENDING ||
      s === ORDER_STATUS.PREPARING ||
      s === ORDER_STATUS.CONFIRMED
    );
  }).length;
  const totalSpent = orders
    .filter((o) => getStatusValue(o.orderStatus) === ORDER_STATUS.DELIVERED)
    .reduce((sum, o) => sum + (o.totalBill || 0), 0);

  const mostOrdered = getMostOrdered(orders);

  const filteredOrders = orders.filter((o) => {
    const s = getStatusValue(o.orderStatus);
    if (activeFilter === "delivered") return s === ORDER_STATUS.DELIVERED;
    if (activeFilter === "cancelled") return s === ORDER_STATUS.CANCELLED;
    if (activeFilter === "In progress")
      return (
        s === ORDER_STATUS.PENDING ||
        s === ORDER_STATUS.PREPARING ||
        s === ORDER_STATUS.CONFIRMED
      );
    return true;
  });

  if (loading) {
    return (
      <div className="max-w-6xl mx-auto px-4 py-6 sm:px-6 lg:px-10 lg:py-10 animate-pulse">
        <div className="mb-8">
          <div className="h-7 w-36 bg-gray-200 rounded-lg" />
          <div className="h-4 w-52 bg-gray-100 rounded mt-2" />
        </div>

        <div className="flex flex-col lg:flex-row lg:gap-8 lg:items-start">
          <div className="flex-1 min-w-0">
            <div className="lg:hidden mb-6 space-y-3">
              <div className="grid grid-cols-2 gap-2.5">
                {[...Array(4)].map((_, i) => (
                  <div
                    key={i}
                    className="bg-white rounded-xl border border-gray-100 px-4 py-3"
                  >
                    <div className="h-6 w-10 bg-gray-200 rounded mb-2" />
                    <div className="h-3 w-20 bg-gray-100 rounded" />
                  </div>
                ))}
              </div>

              <div className="bg-gray-900 rounded-xl px-4 py-3.5 flex items-center justify-between">
                <div>
                  <div className="h-3 w-20 bg-gray-700 rounded mb-2" />
                  <div className="h-6 w-24 bg-gray-700 rounded" />
                </div>
                <div className="h-3 w-32 bg-gray-700 rounded" />
              </div>

              <div className="bg-white rounded-xl border border-gray-100 px-4 py-3 flex items-center justify-between">
                <div>
                  <div className="h-3 w-20 bg-gray-100 rounded mb-2" />
                  <div className="h-4 w-32 bg-gray-200 rounded" />
                </div>
                <div className="h-3 w-16 bg-gray-100 rounded" />
              </div>
            </div>

            <div className="flex gap-2 mb-5">
              {[80, 72, 76, 64].map((w, i) => (
                <div
                  key={i}
                  className={`h-8 w-${w === 80 ? "20" : w === 72 ? "20" : w === 76 ? "24" : "16"} bg-gray-200 rounded-full flex-shrink-0`}
                  style={{ width: `${w}px` }}
                />
              ))}
            </div>

            <div className="flex flex-col gap-2.5">
              {[...Array(6)].map((_, i) => (
                <div
                  key={i}
                  className="bg-white rounded-2xl border border-gray-100 px-4 py-4 sm:px-5 flex items-center gap-3"
                >
                  <div className="w-4 h-4 bg-gray-200 rounded flex-shrink-0" />

                  <div className="flex-1 min-w-0 space-y-2">
                    <div className="h-3.5 w-28 bg-gray-200 rounded" />
                    <div className="h-3 w-48 bg-gray-100 rounded" />
                  </div>

                  <div className="flex flex-col items-end gap-2 flex-shrink-0">
                    <div className="h-4 w-14 bg-gray-200 rounded" />
                    <div
                      className="h-5 w-18 bg-gray-100 rounded-full"
                      style={{ width: "72px" }}
                    />
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="hidden lg:flex flex-col w-72 flex-shrink-0 space-y-4">
            <div className="h-3 w-20 bg-gray-200 rounded px-1" />

            {[...Array(4)].map((_, i) => (
              <div
                key={i}
                className="bg-white rounded-xl border border-gray-100 px-5 py-4"
              >
                <div className="h-7 w-12 bg-gray-200 rounded mb-2" />
                <div className="h-3 w-24 bg-gray-100 rounded" />
              </div>
            ))}

            <div className="border-t border-gray-100 pt-4">
              <div className="bg-gray-900 rounded-xl px-5 py-4">
                <div className="h-3 w-20 bg-gray-700 rounded mb-2" />
                <div className="h-7 w-28 bg-gray-700 rounded mb-2" />
                <div className="h-3 w-36 bg-gray-700 rounded" />
              </div>
            </div>

            <div className="bg-white rounded-xl border border-gray-100 px-5 py-4">
              <div className="h-3 w-20 bg-gray-100 rounded mb-2" />
              <div className="h-4 w-32 bg-gray-200 rounded mb-1.5" />
              <div className="h-3 w-24 bg-gray-100 rounded" />
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (orders?.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-24 gap-3 px-4">
        <div className="w-14 h-14 bg-gray-50 rounded-2xl flex items-center justify-center text-2xl border border-gray-100">
          🧾
        </div>
        <h2 className="text-base font-semibold text-gray-800">No orders yet</h2>
        <p className="text-sm text-gray-500 text-center max-w-xs leading-relaxed">
          Looks like you haven't placed any orders yet. Start ordering your
          favourite dishes 🍕
        </p>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto px-4 py-6 sm:px-6 lg:px-10 lg:py-10">
      <div className="mb-8">
        <h1 className="text-2xl font-semibold text-gray-900">Your orders</h1>
        <p className="text-sm text-gray-500 mt-1">
          Tap any order to expand details
        </p>
      </div>

      <div className="flex flex-col lg:flex-row lg:gap-8 lg:items-start">
        <div className="flex-1 min-w-0">
          <div className="lg:hidden mb-6 space-y-3">
            <div className="grid grid-cols-2 gap-2.5">
              <div className="bg-white rounded-xl border border-gray-100 px-4 py-3">
                <p className="text-xl font-semibold text-gray-900">
                  {orders.length}
                </p>
                <p className="text-[11px] text-gray-400 mt-0.5">Total orders</p>
              </div>
              <div className="bg-white rounded-xl border border-gray-100 px-4 py-3">
                <p className="text-xl font-semibold text-green-700">
                  {delivered}
                </p>
                <p className="text-[11px] text-gray-400 mt-0.5">Delivered</p>
              </div>
              <div className="bg-white rounded-xl border border-gray-100 px-4 py-3">
                <p className="text-xl font-semibold text-red-600">
                  {cancelled}
                </p>
                <p className="text-[11px] text-gray-400 mt-0.5">Cancelled</p>
              </div>
              <div className="bg-white rounded-xl border border-gray-100 px-4 py-3">
                <p className="text-xl font-semibold text-yellow-600">
                  {pending}
                </p>
                <p className="text-[11px] text-gray-400 mt-0.5">In progress</p>
              </div>
            </div>

            <div className="bg-gray-900 rounded-xl px-4 py-3.5 flex items-center justify-between">
              <div>
                <p className="text-[11px] text-gray-400">Total spent</p>
                <p className="text-xl font-semibold text-white mt-0.5">
                  ₹{totalSpent.toLocaleString("en-IN")}
                </p>
              </div>
              <p className="text-[11px] text-gray-500">
                Across delivered orders
              </p>
            </div>

            {mostOrdered && (
              <div className="bg-white rounded-xl border border-gray-100 px-4 py-3 flex items-center justify-between">
                <div>
                  <p className="text-[11px] text-gray-400 mb-0.5">
                    Most ordered
                  </p>
                  <p className="text-sm font-medium text-gray-800">
                    {mostOrdered.name}
                  </p>
                </div>
                <span className="text-[11px] text-gray-400 flex-shrink-0 ml-2">
                  in {mostOrdered.count}{" "}
                  {mostOrdered.count === 1 ? "order" : "orders"}
                </span>
              </div>
            )}
          </div>

          <div className="flex gap-2 overflow-x-auto pb-1 mb-5 scrollbar-none -mx-4 px-4 sm:mx-0 sm:px-0">
            {FILTERS.map((f) => (
              <button
                key={f.value}
                onClick={() => setActiveFilter(f.value)}
                className={`flex-shrink-0 text-[13px] px-4 py-1.5 rounded-full border transition-colors ${
                  activeFilter === f.value
                    ? "bg-gray-900 text-white border-gray-900"
                    : "bg-white text-gray-500 border-gray-200 hover:border-gray-300 hover:text-gray-700"
                }`}
              >
                {f.label}
              </button>
            ))}
          </div>

          {filteredOrders.length === 0 ? (
            <div className="text-center py-16 text-sm text-gray-400">
              No orders match this filter.
            </div>
          ) : (
            <div className="flex flex-col gap-2.5">
              {filteredOrders.map((order) => (
                <OrderCard key={order.orderId} order={order} />
              ))}
            </div>
          )}
        </div>

        <div className="hidden lg:block w-72 flex-shrink-0 sticky top-6 space-y-4">
          <p className="text-xs font-medium text-gray-400 uppercase tracking-widest px-1">
            Summary
          </p>

          <StatCard value={orders.length} label="Total orders" />
          <StatCard
            value={delivered}
            label="Delivered"
            color="text-green-700"
          />
          <StatCard value={cancelled} label="Cancelled" color="text-red-600" />
          <StatCard
            value={pending}
            label="In progress"
            color="text-yellow-600"
          />

          <div className="border-t border-gray-100 pt-4">
            <div className="bg-gray-900 rounded-xl px-5 py-4">
              <p className="text-xs text-gray-400 mb-1">Total spent</p>
              <p className="text-2xl font-semibold text-white">
                ₹{totalSpent.toLocaleString("en-IN")}
              </p>
              <p className="text-xs text-gray-500 mt-1">
                Across delivered orders
              </p>
            </div>
          </div>

          {mostOrdered && (
            <div className="bg-white rounded-xl border border-gray-100 px-5 py-4">
              <p className="text-xs text-gray-400 mb-1">Most ordered</p>
              <p className="text-sm font-medium text-gray-800 truncate">
                {mostOrdered.name}
              </p>
              <p className="text-xs text-gray-400 mt-0.5">
                in {mostOrdered.count}{" "}
                {mostOrdered.count === 1 ? "order" : "orders"}
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default UserOrders;
