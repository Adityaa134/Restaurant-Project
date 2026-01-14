import { useEffect, useState } from "react";
import categoryService from "../../services/categoriesService";
import {addCategory,deleteCategory} from "../../features/category/categorySlice"
import dishService from "../../services/dishService"
import {setDishes} from "../../features/dishes/dishSlice"
import { useDispatch } from "react-redux";

function ManageCategories() {
  const [categories, setCategories] = useState([]);
  const [loadingId, setLoadingId] = useState(null);
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchCategories = async () => {
      const data = await categoryService.GetAllCategoriesForAdmin();
      setCategories(Array.isArray(data) ? data : data.items ?? []);
    };
    fetchCategories();
  }, []);

  const toggleStatus = async (categoryId, currentStatus) => {
    if (loadingId) return;
    try {
      setLoadingId(categoryId);
      const updated = await categoryService.UpdateCategoryStatus(
        !currentStatus,
        categoryId
      );

      setCategories(prev =>
        prev.map(cat =>
          cat.categoryId === updated.categoryId ? updated : cat
        )
      );
      if(updated.status===false){
        dispatch(deleteCategory(updated.categoryId))
      }
      else{
        dispatch(addCategory(updated))
      }
      const dishes = await dishService.GetDishes();
      dispatch(setDishes(dishes));
    } catch (err) {
      console.error(err);
    } finally {
      setLoadingId(null);
    }
  };

  return (
    <div className="max-w-5xl mx-auto p-6 space-y-6">
      <h2 className="text-2xl font-bold">Manage Categories</h2>

      {categories.map(category => (
        <div
          key={category.categoryId}
          className="flex items-center justify-between bg-white p-4 rounded-xl border shadow-sm"
        >

          <div className="flex items-center gap-4">
            <img
              src={`https://localhost:7219${category.cat_Image}`}
              alt={category.cat_Name}
              className="w-14 h-14 rounded-lg object-cover border"
            />

            <div>
              <p className="font-semibold text-gray-900">
                {category.cat_Name}
              </p>

              <span
                className={`inline-block mt-1 px-3 py-1 text-xs rounded-full font-medium
                  ${
                    category.status
                      ? "bg-green-100 text-green-700"
                      : "bg-red-100 text-red-700"
                  }`}
              >
                {category.status ? "Active" : "Inactive"}
              </span>
            </div>
          </div>

          <button
            disabled={loadingId === category.categoryId}
            onClick={() =>
              toggleStatus(category.categoryId, category.status)
            }
            className={`
              relative w-12 h-6 rounded-full transition
              ${category.status ? "bg-green-500" : "bg-gray-300"}
              ${loadingId === category.categoryId ? "opacity-50 cursor-not-allowed" : ""}
            `}
          >
            <span
              className={`
                absolute top-0.5 left-0.5 w-5 h-5 bg-white rounded-full shadow
                transition-transform
                ${category.status ? "translate-x-6" : ""}
              `}
            />
          </button>
        </div>
      ))}
    </div>
  );
}

export default ManageCategories;