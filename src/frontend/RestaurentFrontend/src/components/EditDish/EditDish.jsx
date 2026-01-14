import { useEffect, useState } from 'react'
import { useParams} from "react-router-dom"
import dishService from "../../services/dishService"
import { useForm } from "react-hook-form"
import { Input, Button, Select } from "../index"
import { useDispatch, useSelector } from "react-redux"
import { updateDish } from "../../features/dishes/dishSlice"

function EditDish() {
    const [dishImage, setDishImage] = useState(null)
    const [dishPath, setDishPath] = useState('')
    const [successMessage, setSuccessMessage] = useState("")
    const { dishId } = useParams()
    const [loading, setLoading] = useState(true)
    const [isSubmitting, setIsSubmitting] = useState(false);
    const dispatch = useDispatch()
    const { register, handleSubmit, formState: { errors: formErrors }, reset } = useForm({
        defaultValues: {
            dishName: '',
            price: '',
            description: '',
            categoryId: ''
        }
    });
    const [errors, setErrors] = useState()
    const categories = useSelector((state) => state.category.categories)

    useEffect(() => {
        try {
            const getDishDetails = async () => {
                const data = await dishService.GetDishById(dishId)
                reset({
                    dishName: data.dishName || '',
                    price: data.price || '',
                    description: data.description || '',
                    categoryId: data.categoryId || '',
                    dishId: data.dishId || ''
                });
                setDishPath(data.dish_Image_Path)
                setDishImage(`https://localhost:7219${data.dish_Image_Path}`)
                setLoading(false)
            }
            getDishDetails()
        } catch (error) {
            console.log(error)
            setLoading(false)
        }
    }, [dishId, reset])

    const onSubmit = async (data) => {
        if(isSubmitting) return;
        setIsSubmitting(true)
        setErrors("")
        try {
            let response = await dishService.EditDish({
                dishId: data.dishId || dishId,
                dishName: data.dishName,
                price: data.price,
                description: data.description,
                categoryId: data.categoryId,
                dish_Image: data.dish_Image,
                dish_Image_Path: dishPath
            })

            if (response.dishId) {
                dispatch(updateDish(response))
                const updatedDish = await dishService.GetDishById(dishId);
                reset({
                    dishName: updatedDish.dishName,
                    price: updatedDish.price,
                    description: updatedDish.description,
                    categoryId: updatedDish.categoryId
                });
                setDishImage(`https://localhost:7219${updatedDish.dish_Image_Path}`)
                setSuccessMessage("Dish edited successfully!");
            }
        } catch (error) {
            setErrors(error.message)
            console.log(error)
        }
        finally{
            setIsSubmitting(false)
        }
    }

    return (
        <>
            {
                loading ?
                    <div className="flex justify-center items-center min-h-screen">
                        <div className="text-lg text-gray-700 animate-pulse">
                            Loading dish details...
                        </div>
                    </div>
                    :
                    <div className="min-h-screen flex items-center justify-center bg-gray-100 px-4">
                        <div className="w-full max-w-md bg-white rounded-lg shadow-lg p-6">
                            <h2 className="text-2xl font-semibold text-gray-800 mb-6 text-center">Edit Dish</h2>

                            {successMessage && <p className="text-green-600 mb-4 text-center font-medium">{successMessage}</p>}

                            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">

                                <div>
                                    <Input
                                        type="text"
                                        label="Dish Id"
                                        placeholder="Enter dish name"
                                        disabled
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring focus:ring-blue-300"
                                        {...register("dishId", {
                                            required: "Dish Id is required",
                                        })}
                                    />
                                    {formErrors.dishId && <p className="text-red-500 text-sm mt-1">{formErrors.dishName.message}</p>}
                                </div>

                                <div>
                                    <Input
                                        type="text"
                                        label="Dish Name"
                                        placeholder="Enter dish name"
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring focus:ring-blue-300"
                                        {...register("dishName", {
                                            required: "Dish Name is required",
                                            validate: {
                                                matchPattern: (value) => /^[a-zA-Z\s\-'.,&()]+$/.test(value) || "Dish Name should only contains characters"
                                            }
                                        })}
                                    />
                                    {formErrors.dishName && <p className="text-red-500 text-sm mt-1">{formErrors.dishName.message}</p>}
                                </div>


                                <div>
                                    <Input
                                        type="number"
                                        label="Price (₹)"
                                        placeholder="Enter price"
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring focus:ring-blue-300"
                                        {...register("price", {
                                            required: "Price is required",
                                            validate: {
                                                matchPattern: (value) => /^\d+(\.\d{1,2})?$/.test(value) || "Price should only conatins digits."
                                            }
                                        })}
                                    />
                                    {formErrors.price && <p className="text-red-500 text-sm mt-1">{formErrors.price.message}</p>}
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
                                    {formErrors.categoryId && <p className="text-red-500 text-sm mt-1">{formErrors.categoryId.message}</p>}
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
                                    {formErrors.description && <p className="text-red-500 text-sm mt-1">{formErrors.description.message}</p>}
                                </div>

                                <div>
                                    <label>Current Dish Image</label>
                                    <div>
                                        <img
                                            src={dishImage}
                                            alt="Current dish"
                                            style={{ width: '200px', height: '150px', objectFit: 'cover' }}
                                        />
                                    </div>
                                </div>


                                <div>
                                    <Input
                                        type="file"
                                        label="Choose only .jpg,.jpeg and .png Image and upto 5 mb"
                                        accept="image/*"
                                        className="w-full border border-gray-300 rounded-lg p-2 bg-gray-50 focus:outline-none focus:ring focus:ring-blue-300"
                                        {...register("dish_Image", {
                                        })}
                                    />
                                    {formErrors.dish_Image && <p className="text-red-500 text-sm mt-1">{formErrors.dish_Image.message}</p>}
                                </div>


                                <Button
                                    type="submit"
                                    disabled={isSubmitting}
                                    className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition duration-300"
                                >
                                    Edit Dish
                                </Button>
                            </form>
                            {errors && <p className="text-red-600 mt-8 text-center">{errors}</p>}
                        </div>
                    </div>
            }
        </>
    )
}

export default EditDish