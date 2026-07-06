import { createSlice } from "@reduxjs/toolkit";

const initialState = {
  dishes: [],
  selectedDishById: null,
};

const dishSlice = createSlice({
  name: "dishes",
  initialState,
  reducers: {
    addDish: (state, action) => {
      state.dishes.push(action.payload);
    },
    updateDish: (state, action) => {
      const index = state.dishes.findIndex(
        (dish) => dish.dishId === action.payload.dishId,
      );

      if (index !== -1) {
        state.dishes[index] = action.payload;
      }

      if (state.selectedDishById?.dishId === action.payload.dishId)
        state.selectedDishById = action.payload;
    },
    deleteDish: (state, action) => {
      state.dishes = state.dishes.filter(
        (dish) => dish.dishId !== action.payload,
      );
    },
    setDishById: (state, action) => {
      state.selectedDishById = action.payload;
    },
    setDishes: (state, action) => {
      state.dishes = action.payload;
    },
    setInitialDishes: (state, action) => {
      state.dishes = action.payload.items;
      state.hasMore = action.payload.hasMore;
      state.nextCursorCreatedAt = action.payload.nextCursorCreatedAt;
      state.nextCursorDishId = action.payload.nextCursorDishId;
    },
    appendDishes: (state, action) => {
      state.dishes = [...state.dishes, ...action.payload.items];
      state.hasMore = action.payload.hasMore;
      state.nextCursorCreatedAt = action.payload.nextCursorCreatedAt;
      state.nextCursorDishId = action.payload.nextCursorDishId;
    },
    setDishesLoading: (state, action) => {
      state.dishesLoading = action.payload;
    },
  },
});

export const {
  setDishes,
  setDishById,
  deleteDish,
  updateDish,
  addDish,
  appendDishes,
  setDishesLoading,
  setInitialDishes,
} = dishSlice.actions;

export default dishSlice.reducer;
