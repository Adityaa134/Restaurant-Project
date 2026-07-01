import axiosInstance from "../axios/axiosInstance";
import { logger } from "../utils/logger";

export class CartService {
  async AddItemToCart(userId, dishId) {
    try {
      const response = await axiosInstance.post(`/api/Carts/add-to-cart`, {
        UserId: userId,
        DishId: dishId,
      });
      return response.data;
    } catch (error) {
      logger.error("CartService :: AddItemToCart :: ", {
        status: error.response?.status,
        detail: error.response?.data?.detail || error.message,
      });
    }
  }

  async GetCartItems(userId) {
    try {
      if (userId == undefined) {
        let response = await axiosInstance.get(`/api/Carts/GetCartItems`);
        return response.data;
      }
      let response = await axiosInstance.get(
        `/api/Carts/GetCartItems?userId=${userId}`,
      );
      return response.data;
    } catch (error) {
      logger.log("CartService :: GetCartItems :: ", {
        status: error.response?.status,
        detail: error.response?.data?.detail || error.message,
      });
    }
  }

  async UpdateQuantity(quantity, cartId) {
    try {
      let response = await axiosInstance.put(`/api/Carts/update-quantity`, {
        Quantity: quantity,
        CartId: cartId,
      });
      return response.data;
    } catch (error) {
      logger.log("CartService :: UpdateQuantity :: ", {
        status: error.response?.status,
        detail: error.response?.data?.detail || error.message,
      });
    }
  }

  async MergeCart(userId, items) {
    try {
      const response = await axiosInstance.post(
        `/api/Carts/${userId}/merge`,
        items,
      );
      return response.data;
    } catch (error) {
      logger.error("CartService :: MergeCart :: ", {
        status: error.response?.status,
        detail: error.response?.data?.detail || error.message,
      });
    }
  }
}

const cartService = new CartService();
export default cartService;
