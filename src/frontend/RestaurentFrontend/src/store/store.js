import { configureStore } from '@reduxjs/toolkit';
import authSlice from "../features/auth/authSlice"
import dishSlice from "../features/dishes/dishSlice"
import categorySlice from "../features/category/categorySlice"
import cartSlice from "../features/cart/cartSlice"
import orderSlice from "../features/orders/orderSlice"

const store = configureStore({
    reducer: {
        auth: authSlice,
        dishes: dishSlice,
        category: categorySlice,
        carts: cartSlice,
        orders:orderSlice
    }
})

export default store;