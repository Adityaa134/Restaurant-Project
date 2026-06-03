import { useEffect, useState } from "react";
import { useNavigate, NavLink, useLocation } from "react-router-dom";
import { useSelector } from "react-redux";
import {
  ShoppingCartIcon,
  Bars3Icon,
  XMarkIcon,
} from "@heroicons/react/24/outline";

import { SearchBar, ProfileMenu } from "../index";

function Header() {
  const navigate = useNavigate();
  const location = useLocation();
  const authStatus = useSelector((state) => state.auth.authStatus);
  const profileImage = useSelector((state) => state.auth.profileImage);
  const userType = useSelector((state) => state.auth.role);

  const cartItems = useSelector((state) => state.carts.cartItems);

  const [cartCount, setCartCount] = useState(0);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const categoriesData = useSelector((state) => state.category.categories);

  useEffect(() => {
    setCartCount(cartItems?.length || 0);
  }, [cartItems]);

  const navItems = [
    {
      url: "/",
      name: "Home",
      active: true,
    },
    {
      url: "/categories",
      name: "Categories",
      active: true,
    },
  ];

  return (
    <header className="bg-white border-b border-gray-200 shadow-sm sticky top-0 z-50">
      <div className="max-w-7xl mx-auto">
        <div className="hidden md:flex items-center h-16 px-10 gap-16">
          <ul className="flex items-center space-x-12 flex-shrink-0">
            {navItems.map(
              (item) =>
                item.active && (
                  <li key={item.name}>
                    <NavLink
                      to={item.url}
                      className="text-gray-700 hover:text-blue-600 font-medium transition"
                    >
                      {item.name}
                    </NavLink>
                  </li>
                ),
            )}

            {authStatus && userType === "admin" && (
              <li className="relative group">
                <span className="cursor-pointer text-gray-700 hover:text-blue-600 font-medium">
                  Admin
                </span>

                <ul className="absolute top-full left-0 mt-2 w-48 bg-white rounded-lg shadow-lg border opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 z-50">
                  <li>
                    <NavLink
                      to="/add-dish"
                      className="block px-4 py-3 hover:bg-gray-100"
                    >
                      Add Dish
                    </NavLink>
                  </li>

                  <li>
                    <NavLink
                      to="/manage-orders"
                      className="block px-4 py-3 hover:bg-gray-100"
                    >
                      Manage Orders
                    </NavLink>
                  </li>

                  <li>
                    <NavLink
                      to="/add-category"
                      className="block px-4 py-3 hover:bg-gray-100"
                    >
                      Add Category
                    </NavLink>
                  </li>

                  <li>
                    <NavLink
                      to="/manage-categories"
                      className="block px-4 py-3 hover:bg-gray-100"
                    >
                      Manage Categories
                    </NavLink>
                  </li>
                </ul>
              </li>
            )}
          </ul>

          <div className="flex-1 max-w-xl mx-4">
            <SearchBar />
          </div>

          <div className="flex items-center space-x-10 flex-shrink-0">
            <div
              className="relative cursor-pointer"
              onClick={() => navigate("/cart")}
            >
              <ShoppingCartIcon className="h-7 w-7 text-gray-700 hover:text-blue-600 transition" />

              {cartCount > 0 && (
                <span className="absolute -top-2 -right-2 bg-blue-600 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                  {cartCount}
                </span>
              )}
            </div>

            {authStatus ? (
              <ProfileMenu profileImage={profileImage} />
            ) : (
              <div className="flex items-center space-x-10">
                <NavLink
                  to="/login"
                  className="text-gray-700 hover:text-blue-600 font-medium"
                >
                  Login
                </NavLink>

                <NavLink
                  to="/register"
                  className="text-gray-700 hover:text-blue-600 font-medium"
                >
                  Register
                </NavLink>
              </div>
            )}
          </div>
        </div>

        <div className="md:hidden">
          <div className="flex items-center justify-between px-3 py-3 gap-3">
            <button
              onClick={() => setMobileMenuOpen(true)}
              className="flex-shrink-0"
            >
              <Bars3Icon className="h-7 w-7 text-gray-700" />
            </button>

            <div className="flex-1">
              <SearchBar />
            </div>

            <div className="flex items-center gap-3 flex-shrink-0">
              <div
                className="relative cursor-pointer"
                onClick={() => navigate("/cart")}
              >
                <ShoppingCartIcon className="h-7 w-7 text-gray-700" />

                {cartCount > 0 && (
                  <span className="absolute -top-2 -right-2 bg-blue-600 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                    {cartCount}
                  </span>
                )}
              </div>

              {authStatus && <ProfileMenu profileImage={profileImage} />}
            </div>
          </div>

          {location.pathname === "/" && (
            <div className="border-t border-gray-100 bg-white px-3 py-4">
              <div className="flex items-center justify-between mb-4">
                <h2 className="text-base font-semibold text-gray-800">
                  Popular Categories
                </h2>

                <button
                  onClick={() => navigate("/categories")}
                  className="text-sm font-medium text-gray-500 hover:text-blue-600 transition"
                >
                  View all
                </button>
              </div>

              <div className="flex overflow-x-auto gap-4 scrollbar-hide pb-2">
                <button
                  onClick={() => navigate("/categories")}
                  className="flex flex-col items-center min-w-[72px]"
                >
                  <div className="w-16 h-16 rounded-full bg-gradient-to-r from-blue-500 to-purple-500 flex items-center justify-center text-white font-semibold text-sm shadow-md">
                    All
                  </div>

                  <span className="mt-2 text-sm text-gray-700 font-medium">
                    All
                  </span>
                </button>

                {categoriesData?.slice(0, 5).map((category) => (
                  <button
                    key={category.categoryId}
                    onClick={() => navigate("/categories")}
                    className="flex flex-col items-center min-w-[72px]"
                  >
                    <div className="w-16 h-16 rounded-full overflow-hidden border border-gray-200 shadow-sm">
                      <img
                        src={category.cat_Image}
                        alt={category.cat_Name}
                        className="w-full h-full object-cover"
                      />
                    </div>

                    <span className="mt-2 text-sm text-gray-700 font-medium text-center line-clamp-1">
                      {category.cat_Name}
                    </span>
                  </button>
                ))}
              </div>
            </div>
          )}

          {mobileMenuOpen && (
            <div className="fixed inset-0 z-50">
              <div
                className="absolute inset-0 bg-black/40"
                onClick={() => setMobileMenuOpen(false)}
              />

              <div
                className="
                  absolute
                  top-0
                  left-0
                  h-auto
                  w-full
                  bg-white
                  shadow-2xl
                "
              >
                <div className="flex items-center gap-3 px-4 h-20 border-b border-gray-200">
                  <button
                    onClick={() => setMobileMenuOpen(false)}
                    className="flex-shrink-0"
                  >
                    <XMarkIcon className="h-7 w-7 text-gray-700" />
                  </button>

                  <div className="flex-1">
                    <SearchBar />
                  </div>

                  <div
                    className="relative flex-shrink-0 cursor-pointer"
                    onClick={() => {
                      navigate("/cart");
                      setMobileMenuOpen(false);
                    }}
                  >
                    <ShoppingCartIcon className="h-7 w-7 text-gray-700" />

                    {cartCount > 0 && (
                      <span className="absolute -top-2 -right-2 bg-blue-600 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                        {cartCount}
                      </span>
                    )}
                  </div>

                  {authStatus && (
                    <div className="flex-shrink-0">
                      <ProfileMenu profileImage={profileImage} />
                    </div>
                  )}
                </div>

                <div className="flex flex-col">
                  <NavLink
                    to="/"
                    onClick={() => setMobileMenuOpen(false)}
                    className="
                    h-14
flex
items-center
justify-center
text-base
font-medium
text-gray-700
border-b
border-gray-200
transition
hover:bg-gray-50

                    "
                  >
                    Home
                  </NavLink>

                  <NavLink
                    to="/categories"
                    onClick={() => setMobileMenuOpen(false)}
                    className="
                  h-14
flex
items-center
justify-center
text-base
font-medium
text-gray-700
border-b
border-gray-200
transition
hover:bg-gray-50

                  "
                  >
                    Categories
                  </NavLink>

                  {!authStatus && (
                    <>
                      <NavLink
                        to="/login"
                        onClick={() => setMobileMenuOpen(false)}
                        className="
                       h-14
flex
items-center
justify-center
text-base
font-medium
text-gray-700
border-b
border-gray-200
transition
hover:bg-gray-50

                        "
                      >
                        Login
                      </NavLink>

                      <NavLink
                        to="/register"
                        onClick={() => setMobileMenuOpen(false)}
                        className="
                        h-14
flex
items-center
justify-center
text-base
font-medium
text-gray-700
border-b
border-gray-200
transition
hover:bg-gray-50

                        "
                      >
                        Register
                      </NavLink>
                    </>
                  )}

                  {authStatus && userType === "admin" && (
                    <>
                      <div
                        className="px-6
                          h-12
flex
items-center
justify-center
text-xs
font-bold
tracking-widest
text-gray-400
border-b
border-gray-200
bg-gray-50"
                      >
                        ADMIN
                      </div>

                      <NavLink
                        to="/add-dish"
                        onClick={() => setMobileMenuOpen(false)}
                        className="h-14
flex
items-center
justify-center
text-base
font-medium
text-gray-700
border-b
border-gray-200
transition
hover:bg-gray-50"
                      >
                        Add Dish
                      </NavLink>

                      <NavLink
                        to="/manage-orders"
                        onClick={() => setMobileMenuOpen(false)}
                        className="h-14
flex
items-center
justify-center
text-base
font-medium
text-gray-700
border-b
border-gray-200
transition
hover:bg-gray-50"
                      >
                        Manage Orders
                      </NavLink>

                      <NavLink
                        to="/add-category"
                        onClick={() => setMobileMenuOpen(false)}
                        className="h-14
flex
items-center
justify-center
text-base
font-medium
text-gray-700
border-b
border-gray-200
transition
hover:bg-gray-50"
                      >
                        Add Category
                      </NavLink>

                      <NavLink
                        to="/manage-categories"
                        onClick={() => setMobileMenuOpen(false)}
                        className="h-14
flex
items-center
justify-center
text-base
font-medium
text-gray-700
border-b
border-gray-200
transition
hover:bg-gray-50"
                      >
                        Manage Categories
                      </NavLink>
                    </>
                  )}
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}

export default Header;
