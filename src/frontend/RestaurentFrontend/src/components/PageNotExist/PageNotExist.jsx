import { Link } from "react-router-dom";

export default function PageNotExist() {
  return (
    <div className="flex items-center justify-center min-h-[70vh] px-6">
      <div className="text-center max-w-md">
        <p className="text-7xl sm:text-8xl font-bold text-gray-200 mb-4">404</p>

        <h1 className="text-3xl sm:text-4xl font-bold text-gray-900 mb-3">
          Page Not Found
        </h1>

        <p className="text-gray-500 text-sm sm:text-base mb-8">
          The page you're looking for doesn't exist or may have been moved.
        </p>

        <Link
          to="/"
          className="
            inline-flex
            items-center
            justify-center
            px-6
            py-3
            rounded-xl
            bg-blue-600
            text-white
            font-medium
            hover:bg-blue-700
            transition
          "
        >
          ← Back Home
        </Link>
      </div>
    </div>
  );
}
