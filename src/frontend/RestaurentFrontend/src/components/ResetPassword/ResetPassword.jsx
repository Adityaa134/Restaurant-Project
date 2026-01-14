import { useState } from "react";
import { useForm } from "react-hook-form";
import { useSearchParams } from "react-router-dom";
import { Button,Input } from "../index";
import authService from "../../services/authService";

function ResetPassword() {
  const [searchParams] = useSearchParams();
  const uid = searchParams.get("uid");
  const token = searchParams.get("token");

  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors: formErrors },
    reset
  } = useForm();

  const password = watch("password");

  const onSubmit = async (data) => {
    try {
      const response = await authService.ResetPassword(uid, token, data.password,data.confirmPassword);
      if (response === true) {
        setMessage("Password changed successfully!");
        setError("");
        reset()
      } else {
        setError("Failed to reset password. Please try again.");
        setMessage("");
      }
    } catch (err) {
      setError("Something went wrong. Try again later.");
      setMessage("");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 px-4">
      <div className="bg-white p-8 rounded-xl shadow-lg w-full max-w-md relative">
        
        
        {message && (
          <div className="absolute top-2 left-1/2 transform -translate-x-1/2 w-full max-w-sm">
            <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded relative shadow">
              <span>{message}</span>
              <button
                onClick={() => setMessage("")}
                className="absolute top-1 right-2 text-lg font-bold text-green-700 hover:text-green-900"
              >
                ×
              </button>
            </div>
          </div>
        )}

        
        {error && (
          <div className="absolute top-2 left-1/2 transform -translate-x-1/2 w-full max-w-sm">
            <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative shadow">
              <span>{error}</span>
              <button
                onClick={() => setError("")}
                className="absolute top-1 right-2 text-lg font-bold text-red-700 hover:text-red-900"
              >
                ×
              </button>
            </div>
          </div>
        )}

       
        <h2 className="text-2xl font-bold text-gray-800 text-center">
          Reset Password
        </h2>
        <p className="text-gray-600 text-center mt-2">
          Enter your new password below.
        </p>

        
        <form onSubmit={handleSubmit(onSubmit)} className="mt-6 space-y-5">
          <div>

            <Input
              type="password"
              label="Password"
              placeholder="Enter your password"
              className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              {...register("password", {
                required: "Password is required",
                validate: {
                  minLength: (value) => value.length >= 8 || "Password must be at least 8 characters",
                  hasLowercase: (value) => /[a-z]/.test(value) || "Password must contain at least one lowercase letter",
                  hasUppercase: (value) => /[A-Z]/.test(value) || "Password must contain at least one uppercase letter",
                  hasDigit: (value) => /\d/.test(value) || "Password must contain at least one digit",
                  hasSpecialChar: (value) => /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(value) || "Password must contain at least one special character"
                }
              })}
            />
            {formErrors.password && <p className="text-red-500 text-sm mt-1">{formErrors.password.message}</p>}
          </div>

          <div>

            <Input
              type="password"
              label="Confirm Password"
              placeholder="Confirm your password"
              className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              {...register("confirmPassword", {
                required: "Confirm Password is required",
                validate: value => value === password || "Password and Confirm Password do not match"
              })}
            />
            {formErrors.confirmPassword && <p className="text-red-500 text-sm mt-1">{formErrors.confirmPassword.message}</p>}
          </div>

          <Button
            type="submit"
            className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors"
          >
            Reset Password
          </Button>
        </form>
      </div>
    </div>
  );
}

export default ResetPassword;
