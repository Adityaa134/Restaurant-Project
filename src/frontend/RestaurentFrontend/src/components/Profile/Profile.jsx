import { useEffect, useRef, useState } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useForm } from "react-hook-form";
import { DefaultAvtar } from "../index";
import { useDebounce } from "use-debounce";
import userService from "../../services/userService";
import authService from "../../services/authService";
import { updateUserProfile } from "../../features/auth/authSlice";

const PersonalProfile = () => {
  const userId = useSelector((state) => state.auth.userData?.userId);
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState(null);
  const [preview, setPreview] = useState(DefaultAvtar);
  const [isUsernameTaken, setIsUsernameTaken] = useState(false);
  const [usernameValue, setUsernameValue] = useState("");
  const [debouncedUsername] = useDebounce(usernameValue, 500);
  const [profileImage, setProfileImage] = useState("");
  const dispatch = useDispatch();

  const originalUsernameRef = useRef("");
  const {
    register,
    handleSubmit,
    watch,
    setValue,
    reset,
    trigger,
    formState: { errors },
  } = useForm();

  const username = watch("username");

  useEffect(() => {
    try {
      const fetchData = async () => {
        const data = await userService.GetUserById(userId);
        setUser(data);
        setPreview(data.profileImage ? data.profileImage : DefaultAvtar);
        setProfileImage(data.profileImage);
        originalUsernameRef.current = data.userName;
        reset({
          userName: data.userName,
          phone: data.phoneNumber,
        });
        setLoading(false);
      };
      fetchData();
    } catch (error) {
      console.log(error);
      setLoading(false);
    }
  }, [userId, reset]);

  useEffect(() => {
    if (
      debouncedUsername &&
      debouncedUsername !== originalUsernameRef.current
    ) {
      trigger("userName");
    }
  }, [debouncedUsername, trigger]);

  const checkUsernameUnique = async (userName) => {
    if (!userName) {
      setIsUsernameTaken(false);
      return true;
    }

    if (userName === originalUsernameRef.current) {
      setIsUsernameTaken(false);
      return true;
    }

    const result = await authService.checkUsernameExists(userName);

    if (result.exists) {
      setIsUsernameTaken(true);
      return "Username is already taken";
    }

    setIsUsernameTaken(false);
    return true;
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (!file) return;

    setPreview(URL.createObjectURL(file));
    setValue("profileImage", e.target.files);
  };

  const onSubmit = async (data) => {
    try {
      const response = await userService.AddProfileImage({
        userId,
        userName: data.userName,
        profileImagePath: profileImage || "",
        phonenumber: data.phone || "",
        profileImage: data.profileImage || [],
      });

      if (response.profileImage) {
        dispatch(
          updateUserProfile({
            profileImage: response.profileImage,
          }),
        );
      }

      alert("Profile updated successfully");
    } catch (error) {
      console.log(error);
      alert("Failed to update profile");
    }
  };

  return (
    <div className="px-3 sm:px-4 pt-4 sm:pt-8 pb-0">
      {loading ? (
        <div className="max-w-5xl mx-auto p-4 sm:p-6 animate-pulse">
          <div className="bg-white border border-gray-200 rounded-2xl shadow-sm p-5 sm:p-8">
            <div className="flex flex-col items-center">
              <div className="w-28 h-28 bg-gray-200 rounded-full"></div>

              <div className="mt-3 h-9 w-24 bg-gray-200 rounded-full"></div>

              <div className="mt-5 h-5 w-64 bg-gray-200 rounded"></div>
            </div>

            <div className="my-8 h-px bg-gray-200"></div>

            <div className="mb-6">
              <div className="h-4 w-20 bg-gray-200 rounded mb-3"></div>

              <div className="h-14 w-full bg-gray-200 rounded-2xl"></div>
            </div>

            <div className="mb-6">
              <div className="h-4 w-24 bg-gray-200 rounded mb-3"></div>

              <div className="h-14 w-full bg-gray-200 rounded-2xl"></div>
            </div>

            <div className="mb-8">
              <div className="h-4 w-28 bg-gray-200 rounded mb-3"></div>

              <div className="h-14 w-full bg-gray-200 rounded-2xl"></div>
            </div>

            <div className="flex flex-col sm:flex-row justify-end gap-4">
              <div className="h-14 w-full sm:w-40 bg-gray-200 rounded-2xl"></div>

              <div className="h-14 w-full sm:w-48 bg-gray-200 rounded-2xl"></div>
            </div>
          </div>
        </div>
      ) : (
        <form
          onSubmit={handleSubmit(onSubmit)}
          className="
         max-w-3xl mx-auto
    bg-white shadow rounded-2xl
    p-5 sm:p-6 md:p-8
    mb-0
    "
        >
          <div className="flex flex-col items-center gap-4 pt-2">
            <div className="relative">
              <img
                src={preview}
                alt="profile"
                className="
    w-24 h-24 sm:w-28 sm:h-28
    rounded-full object-cover
    border-2 border-gray-200
    shadow-sm
    "
              />

              <label className="absolute -bottom-2 left-1/2 -translate-x-1/2 bg-blue-600 text-white text-xs px-3 py-1 rounded-full cursor-pointer hover:bg-blue-700">
                Change
                <input type="file" hidden onChange={handleImageChange} />
              </label>
            </div>

            <p className="text-gray-600 text-sm">
              Manage your personal information
            </p>
          </div>

          <hr className="my-8" />

          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700">
              Email
            </label>
            <input
              value={user.email}
              disabled
              className="  mt-2 w-full
                px-4 py-3
                border border-gray-300
                rounded-xl
                text-base bg-gray-100 cursor-not-allowed"
            />
          </div>

          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700">
              Username
            </label>

            <input
              {...register("userName", {
                required: "Username is required",
                minLength: {
                  value: 5,
                  message: "Username should be between 5 to 10 characters",
                },
                maxLength: {
                  value: 10,
                  message: "Username should be between 5 to 10 characters",
                },
                pattern: {
                  value: /^[a-zA-Z0-9_]*$/,
                  message:
                    "Username should only contain letters, numbers and underscore",
                },
                validate: checkUsernameUnique,
              })}
              onChange={(e) => setUsernameValue(e.target.value)}
              className="  mt-2 w-full
    px-4 py-3
    border border-gray-300
    rounded-xl
    text-base focus:ring-blue-500 focus:border-blue-500"
            />

            {errors.userName && (
              <p className="text-sm text-red-600">{errors.userName.message}</p>
            )}
          </div>

          <div className="mb-6">
            <label className="block text-sm font-medium text-gray-700">
              Phone Number
            </label>

            <input
              {...register("phone", {
                minLength: {
                  value: 10,
                  message: "Phone number must be exactly 10 digits.",
                },
                maxLength: {
                  value: 10,
                  message: "Phone number must be exactly 10 digits.",
                },
                pattern: {
                  value: /^[0-9]*$/,
                  message: "Phone number should contain only digits",
                },
              })}
              className="  mt-2 w-full
    px-4 py-3
    border border-gray-300
    rounded-xl
    text-base focus:ring-blue-500 focus:border-blue-500"
            />

            {errors.phone && (
              <p className="text-sm text-red-600">{errors.phone.message}</p>
            )}
          </div>

          <div className="flex flex-col sm:flex-row justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={() => {
                const confirmReset = window.confirm(
                  "Are you sure you want to discard unsaved changes?",
                );

                if (confirmReset) {
                  reset();
                }
              }}
              className="w-full sm:w-auto
    px-4 py-3
    border rounded-xl
    text-gray-700
    hover:bg-gray-100
    transition"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isUsernameTaken}
              className="
                   w-full sm:w-auto
                   px-5 py-3
                 bg-blue-600 text-white
                   rounded-xl
                 hover:bg-blue-700
                   disabled:opacity-50 disabled:cursor-not-allowed
                                "
            >
              Save Changes
            </button>
          </div>
        </form>
      )}
    </div>
  );
};

export default PersonalProfile;
