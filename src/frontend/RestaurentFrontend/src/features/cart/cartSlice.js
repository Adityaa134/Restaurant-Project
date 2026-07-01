import { createSlice } from "@reduxjs/toolkit";

const initialState = {
  cartItems: [],
};

const cartSlice = createSlice({
  name: "carts",
  initialState,
  reducers: {
    addItemToCart: (state, action) => {
      state.cartItems.push(action.payload);
      localStorage.setItem("guestCart", JSON.stringify(state.cartItems));
    },
    removeItemFromCart: (state, action) => {
      state.cartItems = state.cartItems.filter(
        (cartItem) => cartItem.cartId !== action.payload,
      );
      localStorage.setItem("guestCart", JSON.stringify(state.cartItems));
    },
    removeItemsFromCart: (state, action) => {
      state.cartItems = [];
      localStorage.removeItem("guestCart");
    },
    setCartItems: (state, action) => {
      state.cartItems = action.payload;
    },
    updateCartItems: (state, action) => {
      const index = state.cartItems.findIndex(
        (cartItem) => cartItem.cartId === action.payload.cartId,
      );

      if (index !== -1) {
        state.cartItems[index] = action.payload;
      }
    },
    updateLocalCartItemQuantity: (state, action) => {
      const { dishId, quantity } = action.payload;
      const item = state.cartItems.find((x) => x.dishId === dishId);
      if (item) {
        item.quantity += quantity;
      }
    },
  },
});

export const {
  addItemToCart,
  removeItemFromCart,
  removeItemsFromCart,
  setCartItems,
  updateCartItems,
  updateLocalCartItemQuantity,
} = cartSlice.actions;

export default cartSlice.reducer;
