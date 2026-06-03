import { useSelector, useDispatch } from "react-redux";
import { Link } from "react-router-dom";
import cartService from "../../services/cartService";
import {
  updateCartItems,
  removeItemFromCart,
} from "../../features/cart/cartSlice";
import { useNavigate } from "react-router-dom";
import { Button } from "../index";

function Cart() {
  const cartItems = useSelector((state) => state.carts.cartItems);
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const updateQuantity = async (quantity, cartId) => {
    try {
      let response = await cartService.UpdateQuantity(quantity, cartId);
      if (response) {
        if (response.quantity === 0) {
          dispatch(removeItemFromCart(response.cartId));
        } else {
          dispatch(updateCartItems(response));
        }
      }
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <div className="max-w-5xl mx-auto px-4 py-6 md:p-6">
      <h1 className="text-2xl md:text-3xl font-bold mb-6 text-gray-900">
        Your Shopping Cart
      </h1>

      {cartItems?.length === 0 ? (
        <div className="text-center bg-gray-50 p-10 rounded-xl shadow">
          <h2 className="text-lg font-semibold text-gray-700 mb-4">
            No items added yet 🛒
          </h2>

          <p className="text-gray-500 mb-6">
            Looks like your cart is empty. Add some delicious dishes!
          </p>

          <Link
            to="/"
            className="px-6 py-3 bg-blue-600 text-white rounded-lg shadow hover:bg-blue-700 transition"
          >
            Go to Home
          </Link>
        </div>
      ) : (
        <div className="space-y-4">
          {cartItems?.map((item) => (
            <div
              key={item.cartId}
              className="
            bg-white
            rounded-xl
            shadow-sm
            border border-gray-100
            p-4
          "
            >
              <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                <div className="flex items-center gap-3 min-w-0 w-full sm:flex-1">
                  <div className="flex-shrink-0">
                    <img
                      src={item.dish_Image_Path}
                      alt={item.dishName}
                      className="
                    w-20 h-20
                    sm:w-24 sm:h-24
                    rounded-xl
                    object-cover
                  "
                    />
                  </div>

                  <div className="min-w-0 flex-1">
                    <h3
                      className="
                    text-lg sm:text-xl
                    font-semibold
                    text-gray-900
                    break-words
                    leading-tight
                  "
                    >
                      {item.dishName}
                    </h3>

                    <p className="text-gray-500 text-lg mt-1">
                      ₹{item.dishPrice}
                    </p>
                  </div>
                </div>

                <div
                  className="
                flex items-center
                gap-4 sm:gap-6
                flex-shrink-0
              "
                >
                  <div
                    className="
                  flex items-center
                  border border-gray-300
                  rounded-lg
                  overflow-hidden
                "
                  >
                    <button
                      onClick={() => updateQuantity(-1, item.cartId)}
                      className="
                    w-8 h-8
                    flex items-center justify-center
                    bg-gray-100 hover:bg-gray-200
                    text-base font-medium
                  "
                    >
                      –
                    </button>

                    <span
                      className="
                    w-10
                    text-center
                    font-semibold
                    text-base
                  "
                    >
                      {item.quantity}
                    </span>

                    <button
                      onClick={() => updateQuantity(+1, item.cartId)}
                      className="
                    w-8 h-8
                    flex items-center justify-center
                    bg-gray-100 hover:bg-gray-200
                    text-base font-medium
                  "
                    >
                      +
                    </button>
                  </div>

                  <p
                    className="
                  text-lg sm:text-xl
                  font-semibold
                  text-gray-900
                  whitespace-nowrap
                "
                  >
                    ₹{item.dishPrice * item.quantity}
                  </p>
                </div>
              </div>
            </div>
          ))}

          <div className="mt-8 border-t pt-6">
            <div
              className="
            flex flex-col gap-4
            sm:flex-row
            sm:items-center
            sm:justify-between
          "
            >
              <div className="text-lg font-semibold text-gray-900">
                Total Amount:{" "}
                <span className="text-black">
                  ₹
                  {cartItems?.reduce(
                    (acc, item) => acc + item.dishPrice * item.quantity,
                    0,
                  )}
                </span>
              </div>

              <Button
                onClick={() => navigate("/checkout")}
                className="
              w-full md:w-auto
              px-8 py-3
              bg-green-600
              text-white
              font-semibold
              rounded-xl
              hover:bg-green-700
              transition
            "
              >
                Proceed to Checkout
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default Cart;
