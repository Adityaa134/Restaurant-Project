import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { MagnifyingGlassIcon } from "@heroicons/react/24/outline";
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
                    className="w-full pl-10 pr-4 py-2 border rounded-full shadow-sm 
                               focus:outline-none focus:ring-2 focus:ring-blue-500"
                />

                {results.length > 0 && (
                    <ul className="absolute mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-lg z-10 max-h-64 overflow-y-auto">
                        {results.map((dish) => (
                            <li
                                key={dish.dishId}
                                onClick={() => {
                                    navigate(`/dish/${dish.dishId}`);
                                    setResults([]);     
                                    setSelected(true); 
                                }}
                                className="flex items-center px-4 py-2 cursor-pointer hover:bg-gray-100 transition"
                            >
                                <img
                                    src={`https://localhost:7219${dish.dish_Image_Path}`}
                                    alt={dish.dishName}
                                    className="w-10 h-10 rounded-md object-cover mr-3"
                                />
                                <span className="text-gray-800">{dish.dishName}</span>
                            </li>
                        ))}
                    </ul>
                )}

                {loading && (
                    <div className="absolute mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-md text-center text-gray-500 text-sm py-2">
                        Searching...
                    </div>
                )}

                {!loading && query.trim() !== "" && results.length === 0 && !selected && (
                    <div className="absolute mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-md text-center text-gray-500 text-sm py-2">
                        No dishes found
                    </div>
                )}
            </div>
        </div>
    );
}