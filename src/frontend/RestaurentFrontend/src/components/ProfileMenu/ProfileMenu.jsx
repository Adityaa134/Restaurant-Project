import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { DefaultAvtar, LogOutBtn } from "../index";

const ProfileMenu = ({ profileImage }) => {
  const [open, setOpen] = useState(false);
  const navigate = useNavigate();

  return (
    <div className="relative flex items-center justify-center h-16 w-12 mr-1">
      <button
        type="button"
        className="flex items-center justify-center h-11 w-11 rounded-full"
        onClick={() => setOpen(!open)}
      >
        <img
          src={profileImage ? profileImage : DefaultAvtar}
          alt="profile"
          className="
          w-11 h-11 
          rounded-full 
          object-cover 
          cursor-pointer 
          border border-gray-300
          transition-all duration-200
          hover:border-gray-400
          "
          onClick={() => setOpen(!open)}
        />
      </button>

      {open && (
        <div
          className="
      absolute
right-0
top-14
w-64
bg-white
rounded-2xl
shadow-xl
border
border-gray-100
overflow-hidden
z-50
animate-in
fade-in
duration-200

    "
        >
          <div className="px-5 py-4 bg-gray-50 border-b border-gray-100">
            <p className="font-semibold text-gray-900 text-sm">My Account</p>

            <p className="text-xs text-gray-500 mt-1">
              Manage your profile and orders
            </p>
          </div>
          <button
            className="w-full
text-left
px-5
py-3.5
text-sm
font-medium
text-gray-700
hover:bg-gray-50
transition-colors"
            onClick={() => {
              setOpen(false);
              navigate("/profile");
            }}
          >
            Personal Info
          </button>

          <button
            className="w-full
text-left
px-5
py-3.5
text-sm
font-medium
text-gray-700
hover:bg-gray-50
transition-colors"
            onClick={() => {
              setOpen(false);
              navigate("/your-orders");
            }}
          >
            Your Orders
          </button>

          <div className="border-t border-gray-100" />
          <div className="px-3 py-2 bg-gray-50">
            <LogOutBtn />
          </div>
        </div>
      )}
    </div>
  );
};

export default ProfileMenu;
