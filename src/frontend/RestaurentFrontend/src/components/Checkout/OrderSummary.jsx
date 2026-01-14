import { useSelector, useDispatch } from "react-redux";
import { toast } from "react-hot-toast";
import orderService from "../../services/orderService";
import { addOrder } from "../../features/orders/orderSlice";
import { removeItemsFromCart } from "../../features/cart/cartSlice";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "../index";

const OrderSummary = ({ selectedAddress, isAddressSelected }) => {
  const cartItems = useSelector(state => state.carts.cartItems);
  const [placingOrder, setPlacingOrder] = useState(false);
  const userId = useSelector((state) => state.auth.userData?.userId);
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const total = cartItems.reduce(
    (acc, item) => acc + item.dishPrice * item.quantity,
    0
  );

  const handlePlaceOrder = async () => {
    if (!isAddressSelected || placingOrder) return;

    try {
      setPlacingOrder(true);

      const orderItems = cartItems.map(item => ({
        dishId: item.dishId,
        unitPrice: item.dishPrice,
        quantity: item.quantity
      }));

      const payload = {
        userId,
        orderDate: new Date().toISOString(),
        orderItems,
        deliveryAddressId: selectedAddress.addressId
      };

      const response = await orderService.CreateOrder(payload);

      dispatch(addOrder(response));
      dispatch(removeItemsFromCart());

      toast.success("Order placed successfully 🎉", {
        duration: 1800,
        style: {
          padding: "16px 20px",
          fontSize: "16px",
          fontWeight: "600",
          borderRadius: "12px",
        },
        iconTheme: {
          primary: "#16a34a",
          secondary: "#fff",
        },
      });
      navigate(`/orders/${response.orderId}`);
    } catch (error) {
      console.log(error.message);
    } finally {
      setPlacingOrder(false);
    }
  };

  return (
    <div className="relative">
      <div
        className={`
          bg-white rounded-xl shadow p-5 space-y-4 transition
          ${!isAddressSelected ? "opacity-50 blur-[1px] pointer-events-none" : ""}
        `}
      >
        <h2 className="text-lg font-semibold">Order Summary</h2>

        {cartItems.map(item => (
          <div key={item.cartId} className="flex justify-between text-sm">
            <span>{item.quantity} × {item.dishName}</span>
            <span>₹{item.dishPrice * item.quantity}</span>
          </div>
        ))}

        <hr />

        <div className="flex justify-between font-semibold text-lg">
          <span>Total</span>
          <span>₹{total}</span>
        </div>

        <Button
          type="button"
          disabled={!selectedAddress || placingOrder}
          onClick={handlePlaceOrder}
          className={`
            w-full mt-6 py-3 rounded-lg font-semibold transition
            ${selectedAddress
              ? "bg-green-600 text-white hover:bg-green-700"
              : "bg-gray-300 text-gray-500 cursor-not-allowed"}
          `}
        >
          {placingOrder ? "Placing Order..." : "Place Order"}
        </Button>
      </div>

      {!isAddressSelected && (
        <div
          className="
            absolute inset-0
            flex items-center justify-center
            text-sm font-medium text-gray-600
            bg-white/60 rounded-xl
          "
        >
          Select a delivery address to continue
        </div>
      )}
    </div>
  );
};

export default OrderSummary;