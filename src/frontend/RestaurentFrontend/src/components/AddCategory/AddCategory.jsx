import { useState } from "react";
import { useForm } from "react-hook-form";
import { Input, Button } from "../index";
import categoryService from "../../services/categoriesService";
import { addCategory } from "../../features/category/categorySlice";
import { useDispatch } from "react-redux";

function AddCategory() {
  const [successMessage, setSuccessMessage] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const dispatch = useDispatch();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm();

  const onSubmit = async (data) => {
    if (isSubmitting) return;

    setIsSubmitting(true);
    setError("");
    setSuccessMessage("");

    try {
      const response = await categoryService.AddCategory({
        categoryName: data.categoryName,
        status: data.status,
        cat_Image: data.cat_Image,
      });

      if (!response) return;
      reset();
      setSuccessMessage("Category added successfully!");

      if (response.status === true) {
        dispatch(addCategory(response));
      }
    } catch (err) {
      setError(err.message || "Failed to add category");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="bg-gray-50 px-4 py-6 sm:py-8 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <div className="text-center">
          <h2 className="text-3xl font-bold text-gray-900">
            Add a New Category
          </h2>
          <p className="mt-2 text-sm text-gray-600">
            Create a new category for your menu
          </p>
        </div>
      </div>

      <div className="mt-5 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-6 px-5 shadow-sm rounded-2xl sm:px-8 border border-gray-200">
          {successMessage && (
            <p className="text-green-600 mb-4 text-center font-medium">
              {successMessage}
            </p>
          )}

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <div>
              <Input
                type="text"
                label="Category Name"
                placeholder="Enter category name"
                className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md 
                           placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 
                           transition-colors duration-200 sm:text-sm"
                {...register("categoryName", {
                  required: "Category name is required",
                  maxLength: {
                    value: 200,
                    message: "Maximum 200 characters allowed",
                  },
                })}
              />
              {errors.categoryName && (
                <p className="mt-1 text-sm text-red-600">
                  {errors.categoryName.message}
                </p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Status
              </label>
              <select
                className="w-full px-3 py-2 border border-gray-300 rounded-md 
                           focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                {...register("status", {
                  required: "Status is required",
                })}
              >
                <option value="">Select status</option>
                <option value={true}>Active</option>
                <option value={false}>Inactive</option>
              </select>
              {errors.status && (
                <p className="mt-1 text-sm text-red-600">
                  {errors.status.message}
                </p>
              )}
            </div>

            <div>
              <Input
                type="file"
                label="Choose category image (.jpg, .jpeg, .png)"
                accept="image/*"
                className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md 
                           bg-gray-50 focus:outline-none focus:ring-blue-500 focus:border-blue-500 
                           transition-colors duration-200 sm:text-sm"
                {...register("cat_Image", {
                  required: "Category image is required",
                })}
              />
              {errors.cat_Image && (
                <p className="mt-1 text-sm text-red-600">
                  {errors.cat_Image.message}
                </p>
              )}
            </div>

            <Button
              type="submit"
              disabled={isSubmitting}
              className={`
            w-full
            py-3
            rounded-xl
            text-white
            flex
            items-center
            justify-center
            transition
            ${
              isSubmitting
                ? "bg-blue-400 cursor-not-allowed opacity-80"
                : "bg-blue-600 hover:bg-blue-700"
            }
          `}
            >
              {isSubmitting ? (
                <span className="w-5 h-5 border-[3px] border-white border-t-transparent rounded-full animate-spin"></span>
              ) : (
                "Add Category"
              )}
            </Button>
          </form>

          {error && (
            <p className="mt-6 text-center text-red-600 text-sm">{error}</p>
          )}
        </div>
      </div>
    </div>
  );
}

export default AddCategory;
