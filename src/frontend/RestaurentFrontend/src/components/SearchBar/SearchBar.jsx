import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { MagnifyingGlassIcon, XMarkIcon } from "@heroicons/react/24/outline";
import { Input } from "../index";
import dishService from "../../services/dishService";

export default function SearchBar() {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [selected, setSelected] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    if (query.trim().length === 0) {
      setResults([]);
      setLoading(false);
      return;
    }

    setLoading(true);
    const fetchData = async () => {
      try {
        const data = await dishService.SearchDish(query);
        setSelected(false);
        setResults(data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    const delayDebounce = setTimeout(fetchData, 400);
    return () => clearTimeout(delayDebounce);
  }, [query]);

  return (
    <div className="flex justify-center w-full px-6 relative">
      <div className="relative w-full max-w-md">
        <MagnifyingGlassIcon className="absolute left-3 top-2.5 h-5 w-5 text-gray-400" />

        <Input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search for a dish..."
          className="w-full pl-10 pr-10 py-2 border rounded-full shadow-sm 
                               focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        {query && (
          <button
            type="button"
            onClick={() => {
              setQuery("");
              setResults([]);
              setSelected(false);
            }}
            className="absolute right-3 top-2.5 text-gray-400 hover:text-gray-600 transition"
          >
            <XMarkIcon className="h-5 w-5" />
          </button>
        )}

        {results.length > 0 && (
          <ul
            className="
                        absolute mt-2 w-full bg-white border border-gray-200 
                        rounded-2xl shadow-xl z-50 overflow-hidden

                        max-h-64 overflow-y-auto

                        sm:rounded-lg sm:shadow-lg
                        "
          >
            {results.map((dish) => (
              <li
                key={dish.dishId}
                onClick={() => {
                  navigate(`/dish/${dish.dishId}`);
                  setResults([]);
                  setSelected(true);
                }}
                className="
                        flex items-center gap-3
                        px-3 py-3
                        cursor-pointer
                        hover:bg-gray-100
                        active:bg-gray-200
                        transition

                        border-b border-gray-100 last:border-b-0
                        "
              >
                <img
                  src={dish.dish_Image_Path}
                  alt={dish.dishName}
                  className="
                            w-12 h-12
                            rounded-xl
                            object-cover
                            flex-shrink-0
                            "
                />

                <span
                  className="
                        text-gray-800
                        text-base
                        font-medium
                        truncate
                        "
                >
                  {dish.dishName}
                </span>
              </li>
            ))}
          </ul>
        )}

        {loading && (
          <div className="absolute mt-2 w-full bg-white border border-gray-200 rounded-2xl shadow-xl z-50 overflow-hidden animate-pulse">
            {[...Array(3)].map((_, i) => (
              <div
                key={i}
                className="flex items-center gap-3 px-3 py-3 border-b border-gray-100 last:border-b-0"
              >
                <div className="w-12 h-12 rounded-xl bg-gray-200 flex-shrink-0" />
                <div
                  className={`h-4 bg-gray-200 rounded-full ${i === 0 ? "w-36" : i === 1 ? "w-28" : "w-32"}`}
                />
              </div>
            ))}
          </div>
        )}

        {!loading &&
          query.trim() !== "" &&
          results.length === 0 &&
          !selected && (
            <div className="absolute mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-md text-center text-gray-500 text-sm py-2">
              No dishes found
            </div>
          )}
      </div>
    </div>
  );
}
