import { useState } from "react"
import { useForm } from 'react-hook-form'
import { useNavigate, Link } from 'react-router-dom'
import authService from "../../services/authService"
import { login as authLogin } from "../../features/auth/authSlice"
import { useDispatch } from 'react-redux'
import { Input, Button } from "../index"
import { GoogleLogin } from '@react-oauth/google';
import { FiEye, FiEyeOff } from "react-icons/fi";

function Login() {
  const [error, setError] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const dispatch = useDispatch()
  const navigate = useNavigate()
  const { register, handleSubmit, formState: { errors: formErrors } } = useForm()

  const login = async (data) => {
    if (isSubmitting) return;
    setIsSubmitting(true)
    setError("")
    try {
      const response = await authService.Login(data);
      if (response.email) {
        dispatch(authLogin(response))
        localStorage.setItem("token", response.token)
        localStorage.setItem("refreshToken", response.refreshToken)
        navigate("/")
      }
      else {
        navigate(`/confirm-email?email=${encodeURIComponent(data.email)}`);
      }

    } catch (error) {
      setError(error.message)
    }
    finally {
      setIsSubmitting(false)
    }
  }

  const handleGoogleSuccess = async (credentialResponse) => {
    setError("")
    try {
      const response = await authService.GoogleLogin(credentialResponse.credential)
      if (response.token) {
        dispatch(authLogin(response))
        localStorage.setItem("token", response.token)
        localStorage.setItem("refreshToken", response.refreshToken)
        navigate("/")
      }
    } catch (error) {
      setError(error.message)
    }
  }

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <div className="text-center">
          <h2 className="text-3xl font-bold text-gray-900">Welcome back</h2>
          <p className="mt-2 text-sm text-gray-600">
            Sign in to your account
          </p>
        </div>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow-sm sm:rounded-lg sm:px-10 border border-gray-200">
          <form onSubmit={handleSubmit(login)} className="space-y-6">
            <div>
              <Input
                type="text"
                label="Username"
                placeholder="Enter your username"
                className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 transition-colors duration-200 sm:text-sm"
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
                      "Username should only contain digits, alphabets and underscore",
                  },
                })}
              />
              {formErrors.userName && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.userName.message}
                </p>
              )}
            </div>
            <div className="w-full">
              <div className="flex items-center justify-between mb-1">
                <label className="block text-gray-700 text-sm">
                  Password
                </label>

                <Link
                  to="/forgot-password"
                  className="text-sm text-blue-600 hover:text-blue-500 hover:underline transition"
                >
                  Forgot Password?
                </Link>
              </div>

              <div className="relative">
                <Input
                  type={showPassword ? "text" : "password"}
                  placeholder="Enter your password"
                  className="appearance-none w-full px-3 py-2 pr-12 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                  {...register("password", {
                    required: "Password is required",
                    validate: {
                      minLength: (value) =>
                        value.length >= 8 || "Password must be at least 8 characters",
                      hasLowercase: (value) =>
                        /[a-z]/.test(value) ||
                        "Password must contain at least one lowercase letter",
                      hasUppercase: (value) =>
                        /[A-Z]/.test(value) ||
                        "Password must contain at least one uppercase letter",
                      hasDigit: (value) =>
                        /\d/.test(value) || "Password must contain at least one digit",
                      hasSpecialChar: (value) =>
                        /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(value) ||
                        "Password must contain at least one special character",
                    },
                  })}
                />

                <button
                  type="button"
                  onClick={() => setShowPassword((prev) => !prev)}
                  className="absolute inset-y-0 right-3 flex items-center text-gray-400 hover:text-gray-600"
                >
                  {showPassword ? (
                    <FiEyeOff size={18} />
                  ) : (
                    <FiEye size={18} />
                  )}
                </button>
              </div>

              {formErrors.password && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.password.message}
                </p>
              )}
            </div>

            <Button
              type="submit"
              disabled={isSubmitting}
              className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 transition"
            >
              Sign in
            </Button>
          </form>

          {error && (
            <div className="mt-6 rounded-md bg-red-50 p-4 border border-red-200">
              <div className="text-sm text-red-700 text-center">
                {error}
              </div>
            </div>
          )}

          <div className="my-4 flex items-center">
            <hr className="flex-grow border-gray-300" />
            <span className="mx-4 text-gray-500">OR</span>
            <hr className="flex-grow border-gray-300" />
          </div>

          <GoogleLogin
            onSuccess={handleGoogleSuccess}
            onError={() => console.log("Login Failed")}
          />

          <div className="mt-6 text-center">
            <Link
              to="/register"
              className="text-blue-600 hover:text-blue-500 font-medium"
            >
              Create your account
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Login