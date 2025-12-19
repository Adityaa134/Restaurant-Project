import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.jsx'
import { Provider } from "react-redux"
import { RouterProvider } from "react-router-dom"
import store from "./store/store.js"
import { createBrowserRouter } from 'react-router-dom'
import { Protected } from "./components/index.js"
import {Home,Login,Register,AddDish,ConfirmEmail,ConfirmEmailSuccess,DishDetails,EditDish,PageNotExist,Categories,ForgotPassword,ResetPassword,Cart,Profile} from "./pages/index.js"
import { GoogleOAuthProvider } from '@react-oauth/google';


const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      {
        path: "/",
        element: <Home />
      },
      {
        path: "/categories",
        element: <Categories />
      },
      {
        path: "/login",
        element: (
          <Protected authentication={false}>
            < Login />
          </Protected>
        )
      },
      {
        path: "/register",
        element: (
          <Protected authentication={false}>
            < Register />
          </Protected>
        )
      },
      {
        path: "/add-dish",
        element: (
          <Protected requiredRole="admin">
            < AddDish />
          </Protected>
        )
      },
      {
        path: "/confirm-email",
        element: <ConfirmEmail />
      },
      {
        path: "/confirm-email-success",
        element: <ConfirmEmailSuccess />
      },
      {
        path: "/dish/:dishId",
        element: < DishDetails />
      },
      {
        path: "/edit-dish/:dishId",
        element: (
          <Protected requiredRole="admin">
            < EditDish />
          </Protected>
        )
      },
      {
        path: "/page-not-exist",
        element: < PageNotExist />
      },
      {
        path: "/forgot-password",
        element: < ForgotPassword />
      },
      {
        path: "/reset-password",
        element: < ResetPassword />
      },
      {
        path: "/cart",
        element: < Cart />
      },
      {
        path: "/profile",
        element: (
          <Protected>
            < Profile />
          </Protected>
        )
      },
    ]
  }
])



createRoot(document.getElementById('root')).render(
  <GoogleOAuthProvider clientId={import.meta.env.VITE_CLIENT_ID}>
    <Provider store={store}>
      <RouterProvider router={router} />
    </Provider>
  </GoogleOAuthProvider>
)
