import { useState } from "react";
import { useSearchParams } from "react-router-dom";
import authService from "../../services/authService";
import { Link } from "react-router-dom";

function ConfirmEmail() {
  const [emailSentMessage, setEmailSentMessage] = useState("");
  const [error, setError] = useState("");
  const [isSending, setIsSending] = useState(false);
  const [searchParams] = useSearchParams();
  const email = searchParams.get("email");

  const handleResendEmail = async () => {
    if (isSending) return;
    try {
      setIsSending(true);
      setError("");
      setEmailSentMessage("");

      const response = await authService.ResendConfirmEmail(email);

      setEmailSentMessage(response);
    } catch (error) {
      setError(error.message);
    } finally {
      setIsSending(false);
    }
  };

  return (
    <div className="min-h-[75vh] sm:min-h-[82vh] bg-gray-50 flex items-center justify-center px-4 py-10 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white w-full max-w-md rounded-3xl border border-gray-200 shadow-sm p-6 sm:p-8">
          {emailSentMessage && (
            <div
              className={`flex items-start justify-between gap-3 px-4 py-3 rounded-xl mb-6 ${
                error
                  ? "bg-red-100 text-red-700 border border-red-200"
                  : "bg-green-100 text-green-700 border border-green-200"
              }`}
            >
              <span className="text-sm font-medium">{emailSentMessage}</span>
              <button
                onClick={() => setEmailSentMessage("")}
                className="text-gray-500 hover:text-gray-700 transition-colors duration-200"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-4 w-4"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M6 18L18 6M6 6l12 12"
                  />
                </svg>
              </button>
            </div>
          )}

          <div className="flex justify-center mb-7">
            <div className="bg-blue-100 p-5 rounded-full">
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

          <h1 className="text-3xl font-bold text-gray-900 text-center mb-3">
            Confirm Your Email
          </h1>
          <p className="text-gray-600 text-center leading-8 text-sm sm:text-base mb-7">
            We've sent a confirmation email to your registered address. Please
            check your inbox and click on the link to verify your account.
          </p>

          <button
            onClick={handleResendEmail}
            disabled={isSending}
            className="
            w-full
            flex
            items-center
            justify-center
            gap-2
            py-3
            px-4
            rounded-2xl
            text-sm
            font-medium
            text-white
            transition
            duration-200
            bg-blue-600
            hover:bg-blue-700
            disabled:bg-blue-400
            disabled:cursor-not-allowed
          "
          >
            {isSending && (
              <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
            )}
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
  );
}

export default ConfirmEmail;
