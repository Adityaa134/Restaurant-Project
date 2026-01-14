import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link } from "react-router-dom";
import { Button } from "../index";
import authService from "../../services/authService";

function ForgotPassword() {
  const [message, setMessage] = useState("");
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const onSubmit = async (data) => {
    try {
      const response = await authService.ForgotPasswordEmail(data.email);
      if (response.success) {
        setMessage(response.message);
      } else {
        setMessage(response.message);
      }
    } catch (error) {
      console.log(error.message);
      setMessage("Something went wrong. Try again later.");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 px-4">
      <div className="bg-white p-8 rounded-xl shadow-lg w-full max-w-md relative">
        
        {message && (
          <div className="absolute top-2 left-1/2 transform -translate-x-1/2 w-full max-w-sm">
            <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded relative shadow">
              <span>{message}</span>
              <Button
                onClick={() => setMessage("")}
                className="absolute top-1 right-2 text-lg font-bold text-green-700 hover:text-green-900"
              >
                ×
              </Button>
            </div>
          </div>
        )}

        
        <h2 className="text-2xl font-bold text-gray-800 text-center">
          Forgot Password?
        </h2>
        <p className="text-gray-600 text-center mt-2">
          No worries, we’ll send you a reset link.
        </p>

        
        <form onSubmit={handleSubmit(onSubmit)} className="mt-6 space-y-5">
          <div>
            <label
              htmlFor="email"
              className="block text-gray-700 font-medium mb-1"
            >
              Enter the email address linked to your account
            </label>
            <input
              type="email"
              id="email"
              placeholder="you@example.com"
              className="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              {...register("email", {
                required: "Email is required",
                pattern: {
                  value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                  message: "Please enter a valid email address",
                },
              })}
            />
            {errors.email && (
              <p className="text-red-500 text-sm mt-1">{errors.email.message}</p>
            )}
          </div>

          <Button
            type="submit"
            className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors"
          >
            Send Reset Link
          </Button>
        </form>

       
        <p className="text-sm text-gray-600 text-center mt-5">
          Remember your password?{" "}
          <Link to="/login" className="text-blue-600 hover:underline">
            Back to Login
          </Link>
        </p>
      </div>
    </div>
  );
}

export default ForgotPassword;