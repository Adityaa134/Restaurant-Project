import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { DefaultAvtar,LogOutBtn } from "../index";

const ProfileMenu = ({profileImage}) => {
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
          src={
            profileImage
              ? `https://localhost:7219${profileImage}`
              : DefaultAvtar
          }
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
      top-12 
      w-48 
      bg-white 
      rounded-md 
      shadow-lg 
      border 
      z-50
      overflow-hidden
    "
        >
          <button
            className="block w-full text-left px-4 py-2 text-sm hover:bg-gray-100"
            onClick={() => {
              setOpen(false);
              navigate("/profile");
            }}
          >
            Personal Info
          </button>

          <button
            className="block w-full text-left px-4 py-2 text-sm hover:bg-gray-100"
            onClick={() => {
              setOpen(false);
              navigate("/your-orders");
            }}
          >
            Your Orders
          </button>

          <div className="border-t" />
          <div className="px-1 py-1">
            <LogOutBtn />
          </div>
        </div>
      )}
    </div>
  );
};

export default ProfileMenu;