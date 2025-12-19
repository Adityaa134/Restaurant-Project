import { createSlice } from "@reduxjs/toolkit"

const initialState = {
    authStatus: false,
    userData: null,
    token: null,
    role: null,
    profileImage: null
}

const authSlice = createSlice({
    name: "auth",
    initialState,
    reducers: {
        login: (state, action) => {
            state.authStatus = true;
            state.userData = action.payload.user
            state.token = action.payload.token;
            state.role = action.payload.role
            state.profileImage = action.payload.profileImage
        },
        logout: (state, action) => {
            state.authStatus = false,
            state.userData = action.payload
            state.token = action.payload;
            state.role = null
            state.profileImage = null
        },
        updateUserProfile: (state, action) => {
            state.profileImage = action.payload.profileImage;
            if (state.userData) {
                state.userData.profileImage = action.payload.profileImage;
            }
        }
    }
})

export const { login, logout, updateUserProfile } = authSlice.actions

export default authSlice.reducer