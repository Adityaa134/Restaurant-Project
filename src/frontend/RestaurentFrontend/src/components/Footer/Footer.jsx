import { Link } from "react-router-dom";
function Footer() {
  return (
    <footer className="bg-gray-900 border-t border-gray-800">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">

        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div>
            <h3 className="text-white text-xl font-semibold mb-2">
              🍔 Restaurant
            </h3>
            <p className="text-gray-400 text-sm">
              Food delivered fast, fresh, and right to your doorstep.
            </p>
          </div>

          <div>
            <h4 className="text-white text-lg font-medium mb-4">
              Quick Links
            </h4>
            <ul className="space-y-3 text-sm">
              <li>
                <Link
                  to="/"
                  className="text-gray-400 hover:text-white transition"
                >
                  Browse Dishes
                </Link>
              </li>
              <li>
                <Link
                  to="/your-orders"
                  className="text-gray-400 hover:text-white transition"
                >
                  My Orders
                </Link>
              </li>
              <li>
                <Link
                  to="/cart"
                  className="text-gray-400 hover:text-white transition"
                >
                  Cart
                </Link>
              </li>
            </ul>
          </div>

          <div>
            <h4 className="text-white text-lg font-medium mb-4">
              Support
            </h4>
            <ul className="space-y-3 text-sm">
              <li>
                <a href="#" className="text-gray-400 hover:text-white transition">
                  Help Center
                </a>
              </li>
              <li>
                <a href="#" className="text-gray-400 hover:text-white transition">
                  Privacy Policy
                </a>
              </li>
              <li>
                <a href="#" className="text-gray-400 hover:text-white transition">
                  Terms & Conditions
                </a>
              </li>
            </ul>
          </div>
        </div>

        <div className="mt-10 pt-6 border-t border-gray-800 text-center text-sm text-gray-400">
          © {new Date().getFullYear()} Restaurant. All rights reserved.
        </div>

      </div>
    </footer>
  );
}

export default Footer;