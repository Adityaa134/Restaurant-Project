import { useEffect, useState } from "react";
import orderService from "../../services/orderService";
import OrderCard from "../OrderCard/OrderCard";
import { useSelector, useDispatch } from "react-redux";
import { setOrders } from "../../features/orders/orderSlice";

const UserOrders = () => {
  const userId = useSelector((state) => state.auth.userData?.userId);
  const orders = useSelector((state) => state.orders.orders);
  const [loading, setLoading] = useState(true);
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


  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center mt-20 space-y-4">
        <div className="w-10 h-10 border-4 border-gray-300 border-t-gray-600 rounded-full animate-spin"></div>
        <p className="text-gray-600 text-sm">Fetching your orders…</p>
      </div>
    );
  }

  if (orders?.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center mt-20 space-y-3">
        <span className="text-5xl">🧾</span>
        <h2 className="text-lg font-semibold text-gray-700">
          No orders yet
        </h2>
        <p className="text-sm text-gray-500 text-center max-w-sm">
          Looks like you haven’t placed any orders yet.
          Start ordering your favourite dishes 🍕🥤
        </p>
      </div>
    );
  }

  return (
    <div className="
        max-w-6xl mx-auto p-4
        grid grid-cols-1
        sm:grid-cols-1
        md:grid-cols-2
        lg:grid-cols-2
        xl:grid-cols-3
        gap-6
      ">
      {orders.map((order) => (
        <OrderCard key={order.orderId} order={order} />
      ))}
    </div>
  );
};

export default UserOrders;