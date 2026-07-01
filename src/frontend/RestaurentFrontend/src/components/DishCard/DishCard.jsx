import { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Button } from "../index";
import cartService from "../../services/cartService";
import { useSelector, useDispatch } from "react-redux";
import { addItemToCart } from "../../features/cart/cartSlice";

function DishCard({ dishId, dishName, price, dish_Image_Path }) {
  const userId = useSelector((state) => state.auth.userData?.userId);
  const cartItems = useSelector((state) => state.carts.cartItems);
  const [isPlacingOrder, setIsPlacingOrder] = useState(false);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const imageUrl = `${dish_Image_Path}`;

  const itemAdded = cartItems.some((cartItem) => cartItem.dishId === dishId)
    ? "View Item"
    : "Order Now";

  const addItem = async () => {
    if (isPlacingOrder) return;
    setIsPlacingOrder(true);
    try {
      if (userId) {
        const response = await cartService.AddItemToCart(userId, dishId);
        if (response) {
          dispatch(addItemToCart(response));
        }
      } else {
        dispatch(
          addItemToCart({
            dishId,
            dishName,
            quantity: 1,
            dishPrice: price,
            dish_Image_Path,
          }),
        );
      }
    } catch (error) {
      console.log(error);
    } finally {
      setIsPlacingOrder(false);
    }
  };

  const handleOrderButton = () => {
    if (itemAdded === "Order Now") {
      addItem();
    } else if (itemAdded === "View Item") {
      navigate("/cart");
    }
  };

  return (
    <div
      className="
            w-full
            max-w-[320px]
            mx-auto
            bg-white
            rounded-xl
            shadow-md
            overflow-hidden
            hover:shadow-lg
            transition-shadow
            duration-300
        "
    >
      <Link to={`/dish/${dishId}`}>
        <img
          className="
                    w-full
                    h-44
                    sm:h-44
                    object-cover
                "
          src={imageUrl}
          alt="Dish Image"
        />
      </Link>

      <div className="p-4 sm:p-4">
        <Link to={`/dish/${dishId}`}>
          <h2
            className="
                        text-xl
                        sm:text-lg
                        font-semibold
                        text-gray-800
                        line-clamp-1
                    "
          >
            {dishName}
          </h2>
        </Link>

        <p
          className="
                    text-blue-600
                    font-bold
                    text-xl
                    sm:text-base
                    mt-2
                "
        >
          ₹{price}
        </p>

        <Button
          onClick={handleOrderButton}
          disabled={isPlacingOrder}
          type="button"
          className="
                    mt-4
                    w-full
                    bg-blue-500
                    text-white
                    py-2.5
                    sm:py-2
                    rounded-lg
                    hover:bg-blue-600
                    transition-colors
                    text-lg
                    sm:text-base
                "
        >
          {itemAdded}
        </Button>
      </div>
    </div>
  );
}

export default DishCard;
