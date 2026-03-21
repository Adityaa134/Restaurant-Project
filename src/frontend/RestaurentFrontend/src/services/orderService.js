import axiosInstance from "../axios/axiosInstance"

export class Order {

    async GetOrders(page) {
        try {
            let response = await axiosInstance.get(`/api/Orders?page=${page}`)
            return response.data
        }
        catch (error) {
            console.log("Order :: GetOrders :: ", error)
        }
    }

    async GetOrderById(orderId) {
        try {
            let response = await axiosInstance.get(`/api/Orders/${orderId}`)
            return response.data
        } catch (error) {
            console.log("Order :: GetOrderById :: ", error)
            return false
        }
    }

    async CreateOrder(orderData) {
        try {
            const response = await axiosInstance.post(`/api/Orders`,orderData);
            return response.data;
        } catch (error) {
            console.log("Order :: CreateOrder :: ", error);
        }
    }

    async GetOrdersByUserId(userId) {
        try {
            const response = await axiosInstance.get(`/api/Orders/user-orders/${userId}`);
            return response.data
        } catch (error) {
            console.log("Order :: GetOrdersByUserId :: ", error)
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
            console.log("Order :: CancelOrder :: ", error)
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
            console.log("Order :: UpdateOrderStatus :: ", error)
        }
    }
}

const orderService = new Order();
export default orderService