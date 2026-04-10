import axiosInstance from "../axios/axiosInstance"

export class Order {

    async GetOrders(page) {
        try {
            let response = await axiosInstance.get(`/api/Orders?page=${page}`)
            return response.data
        }
        catch (error) {
            logger.log("Order :: GetOrders :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
        }
    }

    async GetOrderById(orderId) {
        try {
            let response = await axiosInstance.get(`/api/Orders/${orderId}`)
            return response.data
        } catch (error) {
            logger.log("Order :: GetOrderById :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }
    }

    async CreateOrder(orderData) {
        try {
            const response = await axiosInstance.post(`/api/Orders`,orderData);
            return response.data;
        } catch (error) {
            logger.error("DishService :: CreateOrder :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
        }
    }

    async GetOrdersByUserId(userId) {
        try {
            const response = await axiosInstance.get(`/api/Orders/user-orders/${userId}`);
            return response.data
        } catch (error) {
            logger.log("Order :: GetOrdersByUserId :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
            return false
        }
    }

    async CancelOrder(orderId,orderStatus){
        try {
            let response = await axiosInstance.put(`/api/Orders/cancel-order`,{
                OrderId:orderId,
                OrderStatus:orderStatus
            });
            return response.data
        } catch (error) {
            logger.log("Order :: CancelOrder :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
        }
    }

    async UpdateOrderStatus(orderId,orderStatus){
        try {
            let response = await axiosInstance.put(`/api/Orders`,{
                OrderId:orderId,
                OrderStatus:orderStatus
            });
            return response.data
        } catch (error) {
            logger.error("Order :: UpdateOrderStatus :: ",{
                status: error.response?.status,
                detail:error.response?.data?.detail || error.message
            });
        }
    }
}

const orderService = new Order();
export default orderService