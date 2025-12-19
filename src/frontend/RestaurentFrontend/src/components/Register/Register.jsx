import React, { useEffect, useState } from 'react'
import { Button, Input } from '../index'
import authService from '../../services/authService'
import { login } from "../../features/auth/authSlice"
import { Link, useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { useDispatch } from 'react-redux'
import { useDebounce } from 'use-debounce';
import { GoogleLogin } from '@react-oauth/google';
import { FiEye, FiEyeOff } from "react-icons/fi";

function Register() {
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [emailValue, setEmailValue] = useState("");
  const [usernameValue, setUsernameValue] = useState("");
  const [error, setError] = useState("")
  const navigate = useNavigate()
  const { register, handleSubmit, formState: { errors: formErrors }, reset, watch, trigger } = useForm({
    mode: "all"
  })
  const dispatch = useDispatch()

  const password = watch("password");

  const [debouncedEmail] = useDebounce(emailValue, 500);
  const [debouncedUsername] = useDebounce(usernameValue, 500);

  const createAccount = async (data) => {
    setError("")
    try {
      const response = await authService.Register(data)
      if (response.token) {
        dispatch(login(response))
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
  }

  useEffect(() => {
    if (debouncedEmail) {
      trigger("email");
    }
    if (debouncedUsername) {
      trigger("username");
    }
  }, [debouncedEmail, debouncedUsername, trigger]);


  const checkEmailUnique = async (email) => {
    if (!email) return true;
    const result = await authService.checkEmailExists(email);
    return !result.exists || "Email is already taken";
  };

  const checkUsernameUnique = async (userName) => {
    if (!userName) return true;
    const result = await authService.checkUsernameExists(userName);
    return !result.exists || "Username is already taken";
  };

  const handleGoogleSuccess = async (credentialResponse) => {
    setError("")
    try {
      const response = await authService.GoogleLogin(credentialResponse.credential)
      if (response.token) {
        dispatch(login(response))
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
          <h2 className="text-3xl font-bold text-gray-900">
            Create your account
          </h2>
          <p className="mt-2 text-sm text-gray-600">
            Join us and get started today
          </p>
        </div>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow-sm sm:rounded-lg sm:px-10 border border-gray-200">
          <form onSubmit={handleSubmit(createAccount)} className="space-y-6">

            <div>
              <Input
                type="text"
                label="Username"
                placeholder="Choose a username"
                className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 transition-colors duration-200 sm:text-sm"
                {...register("userName", {
                  required: "Username is required",
                  minLength: {
                    value: 5,
                    message: "Username should be between 5 to 10 characters"
                  },
                  maxLength: {
                    value: 10,
                    message: "Username should be between 5 to 10 characters"
                  },
                  pattern: {
                    value: /^[a-zA-Z0-9_]*$/,
                    message: "Username should only contain letters, numbers and underscore"
                  },
                  validate: {
                    checkUsernameUnique: checkUsernameUnique
                  }
                })}
                onChange={(e) => setUsernameValue(e.target.value)}
              />
              {formErrors.userName && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.userName.message}
                </p>
              )}
            </div>


            <div>
              <Input
                type="email"
                label="Email"
                placeholder="Enter your email"
                className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 transition-colors duration-200 sm:text-sm"
                {...register("email", {
                  required: "Email is required",
                  pattern: {
                    value: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
                    message: "Please enter a valid email address"
                  },
                  validate: checkEmailUnique
                })}
                onChange={(e) => setEmailValue(e.target.value)}
              />
              {formErrors.email && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.email.message}
                </p>
              )}
            </div>


            <div>
              <Input
                type="text"
                label="Phone Number"
                placeholder="Enter your phone number"
                className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 transition-colors duration-200 sm:text-sm"
                {...register("phoneNumber", {
                  required: false,
                  minLength: {
                    value: 10,
                    message: "Phone number must be exactly 10 digits."
                  },
                  maxLength: {
                    value: 10,
                    message: "Phone number must be exactly 10 digits."
                  },
                  pattern: {
                    value: /^[0-9]*$/,
                    message: "Phone number should contain only digits"
                  }
                })}
              />
              {formErrors.phoneNumber && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.phoneNumber.message}
                </p>
              )}
            </div>


            <div>
              <label className="block text-gray-700 text-sm mb-1">
                Password
              </label>

              <div className="relative">
                <Input
                  type={showPassword ? "text" : "password"}
                  placeholder="Enter your password"
                  className="
                  w-full px-3 py-2 pr-12
                  border border-gray-300 rounded-md
                  focus:outline-none focus:ring-blue-500 focus:border-blue-500
                "
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
                {formErrors.password && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.password.message}
                </p>
                )}

                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="
                  absolute inset-y-0 right-3
                  flex items-center
                  text-gray-400 hover:text-gray-600
                  focus:outline-none
                "
                >
                  {showPassword ? <FiEyeOff size={18} /> : <FiEye size={18} />}
                </button>
              </div>
            </div>

            <div>
              <label className="block text-gray-700 text-sm mb-1">
                Confirm Password
              </label>

              <div className="relative">
                <Input
                  type={showConfirmPassword ? "text" : "password"}
                  placeholder="Confirm your password"
                  className="w-full px-3 py-2 pr-12 border border-gray-300 rounded-md"
                  {...register("confirmPassword", {
                  required: "Please confirm your password",
                  validate: (value) =>
                    value === watch("password") || "Passwords do not match",
                })}
                />
                {formErrors.confirmPassword && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.confirmPassword.message}
                </p>
                )}

                <button
                  type="button"
                  onClick={() =>
                    setShowConfirmPassword(!showConfirmPassword)
                  }
                  className="absolute inset-y-0 right-3 flex items-center text-gray-400"
                >
                  {showConfirmPassword ? <FiEyeOff size={18} /> : <FiEye size={18} />}
                </button>
              </div>
            </div>



            <Button
              type="submit"
              className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors duration-200"
            >
              Create Account
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
            onError={() => console.log('Login Failed')}
            theme="outline"
            size="large"
            text="continue_with"
            shape="rectangular"
          />


          <div className="mt-6">
            <div className="relative">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-gray-300" />
              </div>
              <div className="relative flex justify-center text-sm">
                <span className="px-2 bg-white text-gray-500">Already have an account?</span>
              </div>
            </div>

            <div className="mt-6 text-center">
              <Link
                to="/login"
                className="text-blue-600 hover:text-blue-500 font-medium transition-colors duration-200"
              >
                Sign in to your account
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  )

}

export default Register