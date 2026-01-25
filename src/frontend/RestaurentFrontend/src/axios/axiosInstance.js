import axios from "axios"
import { logout } from "../features/auth/authSlice"
import store from "../store/store"
import authService from "../services/authService";


const baseURL = "https://localhost:7219/api"

const axiosInstance = axios.create({
    baseURL,
    withCredentials: true,
    headers: {
        Accept: "application/json"
    }
});


axiosInstance.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config;
    if (
      originalRequest.skipAuthRefresh ||
      originalRequest._retry
    ) {
      return Promise.reject(error);
    }

    if (error.response?.status === 401) {
      originalRequest._retry = true;

      try {
        await axiosInstance.post(
          "/Account/refresh-token",
          {},
          { skipAuthRefresh: true }
        );

        return axiosInstance(originalRequest);

      } catch (refreshError) {
        await authService.Logout();
        store.dispatch(logout());
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);
export default axiosInstance;