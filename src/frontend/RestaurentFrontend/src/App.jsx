import React, { useEffect, useState } from 'react'
import { setDishes } from "./features/dishes/dishSlice"
import { setCategories } from "./features/category/categorySlice"
import dishService from "./services/dishService"
import { Outlet } from 'react-router-dom'
import { Header, Footer } from "./components/index"
import { useDispatch, useSelector } from "react-redux"
import categoryService from "./services/categoriesService"
import { login, logout,updateUserProfile } from "./features/auth/authSlice"
import { jwtDecode } from 'jwt-decode';
import cartService from './services/cartService'
import userService from './services/userService'
import { setCartItems } from "./features/cart/cartSlice"

function App() {

  const dispatch = useDispatch()
  const [loading, setLoading] = useState(true)
  const authStatus = useSelector((state) => state.auth.authStatus)
  const userId = useSelector((state) => state.auth.userData?.userId)

  useEffect(() => {
    const token = localStorage.getItem('token');

    if (token) {
      try {
        const decodedToken = jwtDecode(token);
        
        const userData = {
          userId: decodedToken.sub,
          userName: decodedToken.name,
          email: decodedToken.email
        };

        dispatch(login({
          token,
          user: userData,
          role: decodedToken.role
        }));

        const fetchUserProfile = async () => {
          try {
            const userProfile = await userService.GetUserById(decodedToken.sub);
            if (userProfile.profileImage) {
              dispatch(updateUserProfile({
                profileImage: userProfile.profileImage
              }));
            }
          } catch (error) {
            console.log('Failed to fetch profile:', error);
          }
        };

        fetchUserProfile();


      } catch (error) {
        console.log('Invalid token:', error);
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        dispatch(logout());
      }
    }
  }, [dispatch, authStatus]);


  useEffect(() => {
    dishService.GetDishes()
      .then((response) => {
        if (response) {
          dispatch(setDishes(response))
        }
      })
      .catch((error) => {
        console.log(error)
      })
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    categoryService.GetCategories()
      .then((response) => {
        if (response) {
          dispatch(setCategories(response))
        }
      }).catch((error) => {
        console.log(error)
      })
  }, [])

  useEffect(() => {
    cartService.GetCartItems(userId)
      .then((response) => {
        if (response != null)
          dispatch(setCartItems(response))
      }).catch((error) => {
        console.log(error)
      })
  }, [userId])

  return (
    <div className="min-h-screen flex flex-col">
      <Header />
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer />
    </div>

  )
}

export default App