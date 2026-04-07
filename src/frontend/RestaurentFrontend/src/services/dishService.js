import axiosInstance from "../axios/axiosInstance"

export class Dish {

    async GetDishes() {
        try {
            let response = await axiosInstance.get("/api/Dishes")
            return response.data
        }
        catch (error) {
            logger.log("DishService :: GetDishes :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }

    }

    async GetDishById(dishId) {
        try {
            let response = await axiosInstance.get(`/api/Dishes/${dishId}`)
            return response.data
        } catch (error) {
            logger.log("DishService :: GetDishById :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
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

            const response = await axiosInstance.post(`/api/AdminDishes/add-dish`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

            return response.data;
        } catch (error) {
            logger.error("DishService :: AddDish :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
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

        const response = await axiosInstance.put(`/api/AdminDishes`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });

        return response.data;
    } catch (error) {
        logger.error("DishService :: EditDish :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
        });
        throw error;
    }
    }

    async DeleteDish(dishId) {
        try {
            const response = await axiosInstance.delete(`/api/AdminDishes/${dishId}`);
            return true
        } catch (error) {
            logger.error("DishService :: DeleteDish :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }
    }
    
    async SearchDish(searchString){
        try {
            let response = await axiosInstance.get(`/api/Dishes/${searchString}`);
            return response.data
        } catch (error) {
            logger.log("DishService :: SearchDish :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
        }
    }
}

const dishService = new Dish();
export default dishService