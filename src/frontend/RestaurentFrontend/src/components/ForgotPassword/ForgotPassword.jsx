import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link } from "react-router-dom";
import { Button } from "../index";
import authService from "../../services/authService";

function ForgotPassword() {
  const [message, setMessage] = useState("");
  const [isSending, setIsSending] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const onSubmit = async (data) => {
    try {
      setIsSending(true);
      const response = await authService.ForgotPasswordEmail(data.email);
      if (response.success) {
        setMessage(response.message);
      } else {
        setMessage(response.message);
      }
    } catch (error) {
      console.log(error.message);
      setMessage("Something went wrong. Try again later.");
    } finally {
      setIsSending(false);
    }
  };

  return (
    <div className="min-h-[65vh] sm:min-h-[72vh] flex items-center justify-center bg-gray-100 px-4 py-6 sm:py-8">
      <div className="bg-white w-full max-w-md rounded-3xl shadow-sm border border-gray-200 p-6 sm:p-8 relative">
        {message && (
          <div className="absolute -top-16 left-1/2 -translate-x-1/2 w-[90%] sm:w-full max-w-sm z-10">
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

        <h2 className="text-3xl font-semibold text-gray-900 text-center">
          Forgot Password?
        </h2>
        <p className="text-gray-500 text-center mt-3 text-sm sm:text-base">
          No worries, we’ll send you a reset link.
        </p>

        <form onSubmit={handleSubmit(onSubmit)} className="mt-8 space-y-6">
          <div>
            <label
              htmlFor="email"
              className="block text-gray-700 font-medium mb-2 text-sm sm:text-base"
            >
              Enter the email address linked to your account
            </label>
            <input
              type="email"
              id="email"
              placeholder="you@example.com"
              className="w-full px-4 py-3 rounded-xl border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500"
              {...register("email", {
                required: "Email is required",
                pattern: {
                  value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                  message: "Please enter a valid email address",
                },
              })}
            />
            {errors.email && (
              <p className="text-red-500 text-sm mt-1">
                {errors.email.message}
              </p>
            )}
          </div>

          <Button
            type="submit"
            disabled={isSending}
            className={`
    w-full
    bg-blue-600
    py-3
    rounded-2xl
    font-medium
    flex
    items-center
    justify-center
    transition
    ${
      isSending
        ? "bg-blue-400 cursor-not-allowed opacity-80"
        : "hover:bg-blue-700"
    }
  `}
          >
            {isSending ? (
              <span className="w-5 h-5 border-[3px] border-white border-t-transparent rounded-full animate-spin"></span>
            ) : (
              <span className="text-white">Send Reset Link</span>
            )}
          </Button>
        </form>

        <p className="text-sm text-gray-500 text-center mt-6">
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
