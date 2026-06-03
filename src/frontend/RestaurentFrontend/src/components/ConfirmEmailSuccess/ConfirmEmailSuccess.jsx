import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import authService from "../../services/authService";
import { Link } from "react-router-dom";

function ConfirmEmailSuccess() {
  const [searchParams] = useSearchParams();
  const [message, setMessage] = useState("");
  const [isSuccess, setIsSuccess] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const uid = searchParams.get("uid");
  const token = searchParams.get("token");

  useEffect(() => {
    const confirmEmail = async () => {
      try {
        setIsLoading(true);
        const response = await authService.ConfirmEmail(uid, token);
        setIsSuccess(true);
        setMessage(response);
      } catch (error) {
        setIsSuccess(false);
        setMessage(error.message);
      } finally {
        setIsLoading(false);
      }
    };

    if (uid && token) {
      confirmEmail();
    } else {
      setIsLoading(false);
      setIsSuccess(false);
      setMessage("Invalid confirmation link");
    }
  }, [uid, token]);

  return (
    <>
      {isLoading ? (
        <div className="min-h-[50vh] sm:min-h-[60vh] bg-gray-50 flex items-center justify-center px-4 py-6 sm:px-6 lg:px-8">
          <div className="sm:mx-auto sm:w-full sm:max-w-md">
            <div className="bg-white w-full max-w-md rounded-3xl border border-gray-200 shadow-sm p-6 sm:p-8 text-center mt-2 sm:mt-0">
              <div className="animate-spin rounded-full h-14 w-14 border-[3px] border-gray-300 border-t-blue-600 mx-auto mb-7"></div>
              <h2 className="text-xl font-semibold text-gray-900 mb-2">
                Verifying your email...
              </h2>
              <p className="text-gray-600">
                Please wait while we confirm your account
              </p>
            </div>
          </div>
        </div>
      ) : isSuccess ? (
        <div className="min-h-[62vh] sm:min-h-[72vh] bg-gray-50 flex items-center justify-center px-4 py-10 sm:px-6 lg:px-8">
          <div className="sm:mx-auto sm:w-full sm:max-w-md">
            <div className="bg-white w-full max-w-md rounded-3xl border border-gray-200 shadow-sm p-6 sm:p-8 text-center">
              <div className="flex justify-center mb-7">
                <div className="bg-green-100 p-5 rounded-full">
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    className="h-12 w-12 text-green-600"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                </div>
              </div>

              <h1 className="text-3xl font-bold text-gray-900 mb-3">
                Email Confirmed!
              </h1>

              <p className="text-gray-600 leading-8 text-sm sm:text-base mb-8">
                Your account has been successfully verified. Please log in to
                continue.
              </p>

              <Link
                to="/login"
                className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors duration-200"
              >
                Go to Login
              </Link>
            </div>
          </div>
        </div>
      ) : (
        <div className="min-h-[62vh] sm:min-h-[72vh] bg-gray-50 flex items-center justify-center px-4 py-10 sm:px-6 lg:px-8">
          <div className="sm:mx-auto sm:w-full sm:max-w-md">
            <div className="bg-white w-full max-w-md rounded-3xl border border-gray-200 shadow-sm p-6 sm:p-8 text-center mt-2 sm:mt-0">
              <div className="flex justify-center mb-7">
                <div className="bg-red-100 p-5 rounded-full">
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    className="h-12 w-12 text-red-600"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                </div>
              </div>

              <h1 className="text-3xl font-bold text-gray-900 mb-3">
                Verification Failed
              </h1>

              <p className="text-gray-600 leading-8 text-sm sm:text-base mb-8">
                {message || "There was an issue verifying your email address."}
              </p>

              <Link
                to="/login"
                className="w-full flex justify-center items-center py-3 px-4 rounded-2xl text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 transition duration-200"
              >
                Return to Login
              </Link>
            </div>
          </div>
        </div>
      )}
    </>
  );
}

export default ConfirmEmailSuccess;
