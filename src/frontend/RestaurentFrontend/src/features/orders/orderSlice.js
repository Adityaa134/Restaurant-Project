import { createSlice } from "@reduxjs/toolkit"

const initialState = {
    orders: [],
    selectedOrderById:null
}

const orderSlice = createSlice({
    name:"orders",
    initialState,
    reducers:{
        addOrder: (state, action) => {
            state.orders.push(action.payload)
        },
        setOrders: (state, action) => {
            state.orders = action.payload
        },
        updateOrder: (state, action) => {
            const index = state.orders.findIndex((order) => (
                order.orderId === action.payload.orderId
            ))

            if (index !== -1) {
                state.orders[index] = action.payload
            }

            if (state.selectedOrderById?.odrerId === action.payload.orderId)
                state.selectedOrderById = action.payload
        }
    }
})

export const  {addOrder,setOrders,updateOrder} = orderSlice.actions

export default orderSlice.reducer