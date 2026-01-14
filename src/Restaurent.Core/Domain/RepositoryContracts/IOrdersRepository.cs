using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.DTO;
using Restaurent.Core.Enums;

namespace Restaurent.Core.Domain.RepositoryContracts
{
    public interface IOrdersRepository
    {
        /// <summary>
        /// Returns orders form the data store
        /// </summary>
        /// <param name="page">The current page number</param>
        /// <param name="pageSize">The number of orders</param>
        /// <returns>Returns orders form the data store</returns>
        Task<PaginationResponse<OrderResponse>?> GetOrders(int page,int pageSize);

        /// <summary>
        /// Creates order in the data store
        /// </summary>
        /// <param name="order">The order to create</param>
        /// <returns>Returns the created order</returns>
        Task<Order> CreateOrder(Order order);

        /// <summary>
        /// Checks if a order exist
        /// </summary>
        /// <param name="orderId">The order to check</param>
        /// <returns>Returns true if exist otherwise false</returns>
        Task<bool> OrderExists(Guid orderId);

        /// <summary>
        /// Search for the order based on the order id 
        /// </summary>
        /// <param name="orderId">the order id to search</param>
        /// <returns>Returns the order from the data store</returns>
        Task<OrderResponse?> GetOrderByOrderId(Guid orderId);

        /// <summary>
        /// Search for the order based on the order id 
        /// </summary>
        /// <param name="orderId">the order id to search</param>
        /// <returns>Returns the order from the data store</returns>
        Task<Order?> GetOrderDetailsByOrderId(Guid orderId);

        /// <summary>
        /// Returns the orders of the user 
        /// </summary>
        /// <param name="userId">The user orders to search</param>
        /// <returns>Returns the orders of the user from the data store</returns>
        Task<List<OrderResponse>?> GetOrdersByUserId(Guid userId);

        /// <summary>
        /// Updates the order status in the data store
        /// </summary>
        /// <param name="orderStatus">The status to change</param>
        /// <param name="orderId">The order which status to be change</param>
        /// <returns>Returns updated order details</returns>
        Task<OrderResponse?> UpdateOrderStatus(OrderStatus orderStatus,Guid orderId);
    }
}
