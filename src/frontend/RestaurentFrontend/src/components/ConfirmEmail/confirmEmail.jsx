import { useState } from 'react'
import { useSearchParams } from 'react-router-dom';
import authService from '../../services/authService';
import { Link } from 'react-router-dom';

function ConfirmEmail() {

  const [emailSentMessage, setEmailSentMessage] = useState("")
  const [error, setError] = useState("")
  const [searchParams] = useSearchParams();
  const email = searchParams.get('email');

  const handleResendEmail = async () => {
    try {
      const response = await authService.ResendConfirmEmail(email)
      setEmailSentMessage(response)
    } catch (error) {
      setError(error.message)
    }
  }

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-6 shadow-sm sm:rounded-lg sm:px-10 border border-gray-200">

          {emailSentMessage && (
            <div
              className={`flex items-center justify-between px-4 py-3 rounded-md mb-6 ${error
                  ? "bg-red-100 text-red-700 border border-red-200"
                  : "bg-green-100 text-green-700 border border-green-200"
                }`}
            >
              <span className="text-sm font-medium">{emailSentMessage}</span>
              <button
                onClick={() => setEmailSentMessage("")}
                className="text-gray-500 hover:text-gray-700 transition-colors duration-200"
              >
                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
          )}

          <div className="flex justify-center mb-6">
            <div className="bg-blue-100 p-4 rounded-full">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-12 w-12 text-blue-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
                />
              </svg>
            </div>
          </div>

          <h1 className="text-2xl font-bold text-gray-900 text-center mb-2">
            Confirm Your Email
          </h1>
          <p className="text-gray-600 text-center mb-6">
            We've sent a confirmation email to your registered address.
            Please check your inbox and click on the link to verify your account.
          </p>

          <button
            onClick={handleResendEmail}
            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors duration-200"
          >
            Resend Confirmation Email
          </button>

          <div className="mt-6 text-center">
            <p className="text-sm text-gray-600">
              Already confirmed?{" "}
              <Link
                to="/login"
                className="font-medium text-blue-600 hover:text-blue-500 transition-colors duration-200"
              >
                Go to Login
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default ConfirmEmail