import { createSlice } from "@reduxjs/toolkit"

const initialState = {
    cartItems: []
}

const cartSlice = createSlice({
    name: "carts",
    initialState,
    reducers: {
        addItemToCart: (state, action) => {
            state.cartItems.push(action.payload)
        },
        removeItemFromCart: (state, action) => {
            state.cartItems = state.cartItems.filter((cartItem) => (
                cartItem.cartId !== action.payload
            ))
        },
        removeItemsFromCart: (state, action) => {
            state.cartItems = [];
        },
        setCartItems:(state,action)=>{
            state.cartItems = action.payload
        },
        updateCartItems:(state,action)=>{
           const index = state.cartItems.findIndex((cartItem) => (
                cartItem.cartId === action.payload.cartId
            ))

            if (index !== -1) {
                state.cartItems[index] = action.payload
            }
        }
    }
})

export const{addItemToCart,removeItemFromCart,removeItemsFromCart,setCartItems,updateCartItems} = cartSlice.actions

export default cartSlice.reducer