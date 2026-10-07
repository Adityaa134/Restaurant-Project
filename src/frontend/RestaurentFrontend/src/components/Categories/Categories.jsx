import { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { DishCard } from "../index";
import dishService from "../../services/dishService";

function CategoriesPage() {
  const categories = useSelector((state) => state.category.categories);
  const allDishes = useSelector((state) => state.dishes.dishes);
  const [selectedCategory, setSelectedCategory] = useState("All");
  const [selectedCategoryId, setSelectedCategoryId] = useState(null);
  const [categoryDishes, setCategoryDishes] = useState(null);
  const [loadingCategoryDishes, setLoadingCategoryDishes] = useState(false);

  useEffect(() => {
    if (selectedCategoryId === null) {
      setCategoryDishes(null);
      return;
    }

    const fetchCategoryDishes = async () => {
      setLoadingCategoryDishes(true);
      try {
        const result =
          await dishService.GetDishesByCategory(selectedCategoryId);
        setCategoryDishes(Array.isArray(result) ? result : []);
      } catch (err) {
        console.error(err);
      } finally {
        setLoadingCategoryDishes(false);
      }
    };

    fetchCategoryDishes();
  }, [selectedCategoryId]);

  const safeAllDishes = Array.isArray(allDishes)
    ? allDishes
    : Array.isArray(allDishes?.items)
      ? allDishes.items
      : [];
  const visibleDishes =
    selectedCategory === "All"
      ? safeAllDishes
      : Array.isArray(categoryDishes)
        ? categoryDishes
        : [];

  const handleCategoryClick = (categoryName, categoryId) => {
    setSelectedCategory(categoryName);
    setSelectedCategoryId(categoryName === "All" ? null : categoryId);
  };

  return (
    <div className="bg-gray-50 min-h-screen pb-10">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <h1 className="text-2xl md:text-3xl font-bold text-gray-800 text-center my-5 md:my-6">
          Browse Categories
        </h1>

        <div className="flex space-x-4 md:space-x-6 overflow-x-auto pb-4 scrollbar-hide mb-8">
          <div
            className={`flex-shrink-0 w-20 md:w-28 text-center cursor-pointer transition-transform transform hover:scale-105 ${
              selectedCategory === "All" ? "scale-105" : ""
            }`}
            onClick={() => handleCategoryClick("All", null)}
          >
            <div
              className={`rounded-full h-16 w-16 md:h-24 md:w-24 mx-auto bg-gradient-to-r from-indigo-500 to-purple-500 flex items-center justify-center shadow-md ${
                selectedCategory === "All"
                  ? "ring-4 ring-indigo-500"
                  : "ring-2 ring-gray-200"
              }`}
            >
              <span className="text-white font-semibold text-sm md:text-lg">
                All
              </span>
            </div>
            <h3 className="mt-2 text-xs md:text-sm font-medium text-gray-700 line-clamp-1">
              All
            </h3>
          </div>

          {categories.map((category) => (
            <div
              key={category.categoryId}
              className={`flex-shrink-0 w-20 md:w-28 text-center cursor-pointer transition-transform transform hover:scale-105 ${
                selectedCategory === category.cat_Name ? "scale-110" : ""
              }`}
              onClick={() =>
                handleCategoryClick(category.cat_Name, category.categoryId)
              }
            >
              <div
                className={`rounded-full h-16 w-16 md:h-24 md:w-24 mx-auto overflow-hidden shadow-md border-2 ${
                  selectedCategory === category.cat_Name
                    ? "border-indigo-500"
                    : "border-gray-200"
                }`}
              >
                <img
                  src={category.cat_Image}
                  alt={category.cat_Name}
                  className="h-full w-full object-cover"
                />
              </div>
              <h3 className="mt-2 text-xs md:text-sm font-medium text-gray-700 line-clamp-1">
                {category.cat_Name}
              </h3>
            </div>
          ))}
        </div>

        <h2 className="text-2xl font-semibold text-gray-800 mb-4">
          {selectedCategory === "All" ? "All Dishes" : `${selectedCategory}`}
        </h2>

        {loadingCategoryDishes ? (
          <div className="flex justify-center py-12">
            <div className="w-8 h-8 border-[3px] border-gray-200 border-t-gray-700 rounded-full animate-spin" />
          </div>
        ) : visibleDishes.length === 0 ? (
          <p className="text-gray-600 text-lg">No dishes found.</p>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {visibleDishes.map((dish) => (
              <div key={dish.dishId}>
                <DishCard {...dish} />
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

export default CategoriesPage;
