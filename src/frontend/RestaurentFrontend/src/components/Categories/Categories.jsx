import { useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { DishCard } from "../index";

function CategoriesPage() {
  const categories = useSelector((state) => state.category.categories);
  const allDishes = useSelector((state) => state.dishes.dishes);
  const [selectedCategory, setSelectedCategory] = useState("All");

  const visibleDishes =
    selectedCategory === "All"
      ? allDishes
      : allDishes.filter(
          (dish) => dish.categoryName === selectedCategory
        );

  const handleCategoryClick = (categoryName) => {
    setSelectedCategory(categoryName);
  };

  return (
    <div className="bg-white min-h-screen pb-10">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        
        <h1 className="text-3xl font-bold text-gray-800 text-center my-6">
          Explore Our Categories
        </h1>

        <div className="flex space-x-6 overflow-x-auto pb-4 scrollbar-hide mb-8">
          
          <div
            className={`flex-shrink-0 w-28 text-center cursor-pointer transition-transform transform hover:scale-105 ${
              selectedCategory === "All" ? "scale-110" : ""
            }`}
            onClick={() => handleCategoryClick("All")}
          >
            <div
              className={`rounded-full h-24 w-24 mx-auto bg-gradient-to-r from-indigo-500 to-purple-500 flex items-center justify-center shadow-lg ${
                selectedCategory === "All"
                  ? "ring-4 ring-indigo-500"
                  : "ring-2 ring-gray-200"
              }`}
            >
              <span className="text-white font-semibold text-lg">All</span>
            </div>
            <h3 className="mt-3 text-sm font-medium text-gray-700">All</h3>
          </div>

          {categories.map((category) => (
            <div
              key={category.categoryId}
              className={`flex-shrink-0 w-28 text-center cursor-pointer transition-transform transform hover:scale-105 ${
                selectedCategory === category.cat_Name ? "scale-110" : ""
              }`}
              onClick={() => handleCategoryClick(category.cat_Name)}
            >
              <div
                className={`rounded-full h-24 w-24 mx-auto overflow-hidden shadow-lg border-2 ${
                  selectedCategory === category.cat_Name
                    ? "border-indigo-500"
                    : "border-gray-200"
                }`}
              >
                <img
                  src={`https://localhost:7219${category.cat_Image}`}
                  alt={category.cat_Name}
                  className="h-full w-full object-cover"
                />
              </div>
              <h3 className="mt-3 text-sm font-medium text-gray-700">
                {category.cat_Name}
              </h3>
            </div>
          ))}
        </div>

        <h2 className="text-2xl font-semibold text-gray-800 mb-4">
          {selectedCategory === "All"
            ? "All Dishes"
            : `${selectedCategory}`}
        </h2>

        {visibleDishes.length === 0 ? (
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