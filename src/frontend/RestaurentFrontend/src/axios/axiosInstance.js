import axios from "axios"
import dayjs from "dayjs"
import { useDispatch } from "react-redux"
import { login } from "../features/auth/authSlice"
import { jwtDecode } from 'jwt-decode';
import store from "../store/store"


const baseURL = "https://localhost:7219/api"

const axiosInstance = axios.create({
    baseURL,
    headers: {
        Accept: "application/json"
    }
});

axiosInstance.interceptors.request.use(
    async (config) => {
        let token = localStorage.getItem('token');

        if (token) {
            // Checking if token is expired
            const user = jwtDecode(token);
            const isExpired = Date.now() >= user.exp * 1000;

            if (isExpired) {
                try {
                    const refreshToken = localStorage.getItem('refreshToken')
                    const response = await axios.post(`${baseURL}/Account/refresh-token/`, {
                        JwtToken: token.toString(),
                        RefreshToken: refreshToken.toString()
                    });

                    const newTokens = response.data;
                    localStorage.setItem('token', newTokens.token);
                    localStorage.setItem('refreshToken', newTokens.refreshToken);

                    const decodedToken = jwtDecode(newTokens.token)

                    const userData = {
                        userId: decodedToken.sub,
                        userName: decodedToken.name,
                        email: decodedToken.email
                    };
                    store.dispatch(login({
                        token:newTokens.token,
                        user: userData,
                        role: decodedToken.role,
                        profileImage:newTokens.profileImage
                    }));

                    config.headers.Authorization = `Bearer ${newTokens.token}`;
                } catch (error) {
                    console.log("Axios error : ", error)
                    localStorage.removeItem('token');
                    localStorage.removeItem('refreshToken');
                    window.location.href = '/login';
                    return Promise.reject(error);
                }
            } else {
                config.headers.Authorization = `Bearer ${token}`;
            }
        }

        return config
    }
)

export default axiosInstance;