import { useState } from 'react'
import { useForm } from "react-hook-form"
import { Input, Button, Select } from "../index"
import { useSelector, useDispatch } from "react-redux"
import dishService from "../../services/dishService"
import { addDish } from "../../features/dishes/dishSlice"

function AddDish() {

  const [successMessage, setSuccessMessage] = useState("")
  const [errors, setErrors] = useState()
  const categories = useSelector((state) => state.category.categories)
  const [isSubmitting, setIsSubmitting] = useState(false);
  const dispatch = useDispatch()

  const onSubmit = async (data) => {
    if (isSubmitting) return;
    setIsSubmitting(true);
    setErrors("")
    try {
      let response = await dishService.AddDish(data)
      if (response.dishId) {
        dispatch(addDish(response))
        reset()
        setSuccessMessage("Dish added successfully!");
      }
    } catch (error) {
      setErrors(error.message)
    }
    finally {
    setIsSubmitting(false);
    }
  }

  const { register, handleSubmit, formState: { errors: formErrors }, reset } = useForm()

  return (

    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <div className="text-center">
          <h2 className="text-3xl font-bold text-gray-900">Add a New Dish</h2>
          <p className="mt-2 text-sm text-gray-600">
            Fill the details below to add a dish to your menu
          </p>
        </div>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-6 shadow-sm sm:rounded-lg sm:px-10 border border-gray-200">

          {successMessage && (
            <p className="text-green-600 mb-4 text-center font-medium">
              {successMessage}
            </p>
          )}

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">


            <div>
              <Input
                type="text"
                label="Dish Name"
                placeholder="Enter dish name"
                className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md 
                       placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 
                       transition-colors duration-200 sm:text-sm"
                {...register("dishName", {
                  required: "Dish Name is required",
                  validate: {
                    matchPattern: (value) =>
                      /^[a-zA-Z\s\-'.,&()]+$/.test(value) ||
                      "Dish Name should only contain characters",
                  },
                })}
              />
              {formErrors.dishName && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.dishName.message}
                </p>
              )}
            </div>


            <div>
              <Input
                type="number"
                label="Price (₹)"
                placeholder="Enter price"
                className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md 
                       placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 
                       transition-colors duration-200 sm:text-sm"
                {...register("price", {
                  required: "Price is required",
                  validate: {
                    matchPattern: (value) =>
                      /^\d+(\.\d{1,2})?$/.test(value) ||
                      "Price should only contain digits.",
                  },
                })}
              />
              {formErrors.price && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.price.message}
                </p>
              )}
            </div>


            <div>
              <Select
                label="Category"
                options={categories}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg bg-white focus:outline-none focus:ring focus:ring-blue-300"
                {...register("categoryId", {
                  required: "Category is required"
                })}
              />
              {formErrors.categoryId && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.categoryId.message}
                </p>
              )}
            </div>


            <div>
              <Input
                placeholder="Enter short description"
                label="Description"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring focus:ring-blue-300"
                {...register("description", {
                  required: "Description is required",
                  validate: {
                    matchPattern: (value) =>
                      /^[a-zA-Z\s',.!?-]+$/.test(value) ||
                      "Description should only contain letters, spaces, and basic punctuation (',.!?-)"
                  }
                })}
              />
              {formErrors.description && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.description.message}
                </p>
              )}
            </div>


            <div>
              <Input
                type="file"
                label="Choose only .jpg, .jpeg and .png image (up to 5 MB)"
                accept="image/*"
                className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md 
                       bg-gray-50 focus:outline-none focus:ring-blue-500 focus:border-blue-500 
                       transition-colors duration-200 sm:text-sm"
                {...register("dish_Image", {
                  required: "Dish Image is required",
                })}
              />
              {formErrors.dish_Image && (
                <p className="mt-1 text-sm text-red-600">
                  {formErrors.dish_Image.message}
                </p>
              )}
            </div>


            <Button
              type="submit"
              disabled={isSubmitting}
              className="group relative w-full flex justify-center py-2 px-4 border border-transparent 
                     text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 
                     focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 
                     transition-colors duration-200"
            >
              Add Dish
            </Button>
          </form>


          {errors && (
            <p className="mt-6 text-center text-red-600 text-sm">{errors}</p>
          )}
        </div>
      </div>
    </div>
  )
}

export default AddDish