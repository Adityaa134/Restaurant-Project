import { createSlice } from "@reduxjs/toolkit"

const initialState = {
    authStatus: false, //user is logged in or not
    authChecked: false, // app finished checking auth token
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
            state.authChecked = true;
            state.userData = action.payload.user
            state.token = action.payload.token;
            state.role = action.payload.role
            state.profileImage = action.payload.profileImage
        },
        logout: (state, action) => {
            state.authStatus = false,
            state.authChecked = true;
            state.userData = action.payload
            state.token = action.payload;
            state.role = null
            state.profileImage = null
        },
        authCheckCompleted: (state) => {
            state.authChecked = true; 
        },
        updateUserProfile: (state, action) => {
            state.profileImage = action.payload.profileImage;
            if (state.userData) {
                state.userData.profileImage = action.payload.profileImage;
            }
        }
    }
})

export const { login, logout, updateUserProfile, authCheckCompleted} = authSlice.actions

export default authSlice.reducer