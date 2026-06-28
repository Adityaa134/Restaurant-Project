import { useEffect, useState, useRef } from "react";
import { setDishes } from "./features/dishes/dishSlice";
import { setCategories } from "./features/category/categorySlice";
import dishService from "./services/dishService";
import authService from "./services/authService";
import { Outlet } from "react-router-dom";
import { Header, Footer, ToasterProvider } from "./components/index";
import { useDispatch, useSelector } from "react-redux";
import categoryService from "./services/categoriesService";
import { login, logout, authCheckCompleted } from "./features/auth/authSlice";
import cartService from "./services/cartService";
import { setCartItems } from "./features/cart/cartSlice";
import { logger } from "./utils/logger";
import healthService from "./services/healthService";

function App() {
  const dispatch = useDispatch();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const authStatus = useSelector((state) => state.auth.authStatus);
  const userId = useSelector((state) => state.auth.userData?.userId);
  const authCheckInProgress = useRef(false);
  const restoreAttempted = useRef(false);

  const waitForBackend = async (retryCount = 3) => {
    try {
      await healthService.checkHealth();
    } catch {
      if (retryCount > 0) {
        await new Promise((resolve) => setTimeout(resolve, 3000));
        return waitForBackend(retryCount - 1);
      }

      throw new Error("Backend unavailable");
    }
  };

  const fetchInitialData = async () => {
    const dishes = await dishService.GetDishes();
    dispatch(setDishes(dishes));

    const categories = await categoryService.GetCategories();
    dispatch(setCategories(categories));

    if (userId) {
      const cartItems = await cartService.GetCartItems(userId);

      if (cartItems != null) {
        dispatch(setCartItems(cartItems));
      }
    }
  };

  useEffect(() => {
    if (restoreAttempted.current) return;
    restoreAttempted.current = true;
    const checkAuth = async () => {
      if (authCheckInProgress.current) return;
      authCheckInProgress.current = true;

      try {
        const response = await authService.RestoreSession();

        if (response?.userId) {
          dispatch(
            login({
              user: {
                userId: response.userId,
                userName: response.userName,
                email: response.email,
              },
              role: response.role,
              profileImage: response.profileImage,
            }),
          );
        } else {
          await authService.Logout();
          dispatch(logout());
        }
      } catch (error) {
        if (
          error?.response?.status === 401 ||
          error?.response?.status === 403
        ) {
          await authService.Logout();
          dispatch(logout());
        } else {
          logger.error("Auth check error :: App.jsx ::", error);
        }
      } finally {
        dispatch(authCheckCompleted());
        authCheckInProgress.current = false;
      }
    };

    checkAuth();
  }, [dispatch]);

  useEffect(() => {
    const initializeApp = async () => {
      try {
        await waitForBackend();
        await fetchInitialData();
      } catch {
        setError("Server is waking up. Please refresh after a moment.");
      } finally {
        setLoading(false);
      }
    };

    initializeApp();
  }, []);

  if (loading) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center gap-4">
        <div className="w-10 h-10 border-4 border-gray-300 border-t-black rounded-full animate-spin"></div>

        <p className="text-gray-600 text-sm">
          Preparing menu... Server may take a minute to wake up.
        </p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center px-6 text-center">
        <div className="text-3xl mb-4 opacity-80">⚠️</div>

        <h2 className="text-xl sm:text-2xl font-semibold text-gray-900 mb-3">
          Server is waking up
        </h2>

        <p className="text-gray-500 text-sm sm:text-base max-w-md leading-relaxed">
          Server may be starting after inactivity. Please refresh the page or
          try again after a few moments.
        </p>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col">
      <ToasterProvider />
      <Header />
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer />
    </div>
  );
}

export default App;
