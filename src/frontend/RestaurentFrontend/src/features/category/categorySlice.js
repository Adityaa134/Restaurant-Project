import { createSlice } from "@reduxjs/toolkit"

const initialState = {
    categories:[]
}

const categorySlice = createSlice({
    name:"category",
    initialState,
    reducers:{
        setCategories:(state,action)=>{
            state.categories = action.payload
        },
        addCategory: (state, action) => {
            state.categories.push(action.payload)
        },
        deleteCategory: (state, action) => {
            state.categories = state.categories.filter((category) => (
                category.categoryId !== action.payload
            ))
        }
    }
})

export const {addCategory,setCategories,deleteCategory} = categorySlice.actions

export default categorySlice.reducer