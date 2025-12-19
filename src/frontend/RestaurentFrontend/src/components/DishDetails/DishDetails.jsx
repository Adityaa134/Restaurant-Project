import React, { useEffect, useState } from 'react'
import { useParams, useNavigate } from "react-router-dom"
import dishService from "../../services/dishService"
import { Button } from "../index"
import { deleteDish } from "../../features/dishes/dishSlice"
import { useDispatch,useSelector } from "react-redux"
import {addItemToCart} from "../../features/cart/cartSlice"
import cartService from '../../services/cartService'


function DishDetails() {
    const userId = useSelector((state) => state.auth.userData?.userId);
    const [itemAdded, setItemAdded] = useState("Order Now");
    const [dish, setDish] = useState(null)
    const [loading, setLoading] = useState(true)
    const { dishId } = useParams()
    const navigate = useNavigate()
    const dispatch = useDispatch()
    const userType = useSelector((state) => state.auth.role)
    
    useEffect(() => {
        try {
            const fetchData = async () => {
                const data = await dishService.GetDishById(dishId);
                setDish(data); 
                setLoading(false);
            };
            fetchData();
        } catch (error) {
            console.log(error)
            setLoading(false)
        }
    }, [dishId])

     useEffect(() => {
        const checkItemInCart = async () => {
            if (dishId) {
                try {
                    const exists = await cartService.CheckCartItemExist(userId, dishId);
                    if (exists) {
                        setItemAdded("View Item");
                    } else {
                        setItemAdded("Order Now");
                    }
                } catch (error) {
                    console.log("Error checking item in cart:", error);
                    setItemAdded("Order Now");
                }
            }
        };

        checkItemInCart();
    }, [userId, dishId]);

    const handleDelete = async () => {
        if (window.confirm("Are you sure you want to delete this dish?")) {
            try {
                const result = await dishService.DeleteDish(dishId)
                if (result === true) {
                    alert("Dish deleted successfully!");
                    dispatch(deleteDish(dishId))
                    navigate("/");
                }
                else {
                    alert("Something went wrong while deleting the dish.");
                }
            } catch (error) {
                console.error(error);
                alert("Something went wrong while deleting the dish.");
            }
        }
    }

    const handleEdit = () => {
        navigate(`/edit-dish/${dishId}`);
    };

    const addItem = async () => {
        try {
            const response = await cartService.AddItemToCart(userId, dishId);
            if (response) {
                dispatch(addItemToCart(response));
                setItemAdded("View Item");
            }

        } catch (error) {
            console.log(error);
        }
    };

    const handleOrderButton = () => {
        if (itemAdded === "Order Now") {
            addItem();
        } else if (itemAdded === "View Item") {
            navigate("/cart");
        }
    };

    return (
        
        <>
            {loading ? (
               <div className="max-w-3xl mx-auto p-6 animate-pulse">
                    <div className="flex items-center space-x-4 mb-6">
                        <div className="w-20 h-20 bg-gray-300 rounded-full"></div>
                        <div className="h-4 bg-gray-300 rounded w-48"></div>
                    </div>

                    <div className="space-y-4">
                        <div className="h-10 bg-gray-300 rounded"></div>
                        <div className="h-10 bg-gray-300 rounded"></div>
                        <div className="h-10 bg-gray-300 rounded"></div>
                    </div>
                </div>
            ) : (
                <div className="bg-white min-h-screen">
                    
                    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                        <div className="bg-white rounded-xl shadow-md overflow-hidden border border-gray-200 max-w-2xl mx-auto">
                            
                            <div className="w-full h-80 sm:h-96">
                                <img
                                    src={`https://localhost:7219${dish.dish_Image_Path}`}
                                    alt={dish.dishName}
                                    className="w-full h-full object-cover"
                                />
                            </div>

                            
                            <div className="p-6">
                                <h1 className="text-2xl sm:text-3xl font-bold text-gray-800 mb-4">
                                    {dish.dishName}
                                </h1>

                                <p className="text-gray-600 mb-6 leading-relaxed">
                                    {dish.description || "No description available."}
                                </p>

                                <div className="flex flex-col sm:flex-row items-center justify-between gap-4 mb-6">
                                    <span className="text-2xl font-bold text-green-600">
                                        ₹ {dish.price}
                                    </span>
                                    <Button
                                    onClick={handleOrderButton}
                                    className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded-lg shadow-sm transition-colors duration-200 w-full sm:w-auto text-center">
                                        {itemAdded}
                                    </Button>
                                </div>

                                {userType === "admin" && (
                                    <div className="flex flex-col sm:flex-row items-center gap-4 pt-4 border-t border-gray-200">
                                        <Button
                                            onClick={handleEdit}
                                            className="bg-yellow-500 hover:bg-yellow-600 text-white px-4 py-2 rounded-lg shadow-sm transition-colors duration-200 w-full sm:w-1/2"
                                        >
                                            Edit
                                        </Button>
                                        <Button
                                            onClick={handleDelete}
                                            className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg shadow-sm transition-colors duration-200 w-full sm:w-1/2"
                                        >
                                            Delete
                                        </Button>
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </>


    )
}

export default DishDetails