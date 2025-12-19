
import axiosInstance from "../axios/axiosInstance"

export class Dish {

    async GetDishes() {
        try {
            let response = await axiosInstance.get("/Dishes")
            return response.data
        }
        catch (error) {
            console.log("Dish :: GetDishes :: ", error)
            return false
        }

    }

    async GetDishById(dishId) {
        try {
            let response = await axiosInstance.get(`/Dishes/${dishId}`)
            return response.data
        } catch (error) {
            console.log("Dish :: GetDishById :: ", error)
            return false
        }
    }

    async AddDish({ dishName, price, description, categoryId, dish_Image }) {
        try {
            const formData = new FormData();
            formData.append('DishName', dishName);
            formData.append('Price', parseFloat(price));
            formData.append('Description', description);
            formData.append('CategoryId', categoryId.toString());
            formData.append('Dish_Image', dish_Image[0]);

            const response = await axiosInstance.post(`/AdminDishes/add-dish`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

            return response.data;
        } catch (error) {
            console.log("DishService :: AddDish :: ", error);
            throw error;
        }
    }

    async EditDish({ dishId, dishName, price, description, categoryId, dish_Image, dish_Image_Path }) {
    try {
        const formData = new FormData();
        formData.append('DishId', dishId.toString());
        formData.append('DishName', dishName);
        formData.append('Price', parseFloat(price));
        formData.append('Description', description);
        formData.append('CategoryId', categoryId.toString());
        formData.append("Image_Path", dish_Image_Path.toString());

        if (dish_Image && dish_Image.length > 0) {
            formData.append('Dish_Image', dish_Image[0]);
        }

        const response = await axiosInstance.put(`/AdminDishes`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });

        return response.data;
    } catch (error) {
        console.log("Dish :: EditDish :: ", error);
        throw error;
    }
}

    async DeleteDish(dishId) {
        try {
            const response = await axiosInstance.delete(`/AdminDishes/${dishId}`);
            return true
        } catch (error) {
            console.log("DishService :: DeleteDish :: ", error)
            return false
        }
    }
    async SearchDish(searchString){
        try {
            let response = await axiosInstance.get(`/Dishes/${searchString}`);
            return response.data
        } catch (error) {
            console.log("DishService :: SearchDish :: ", error)
        }
    }
}

const dishService = new Dish();
export default dishService