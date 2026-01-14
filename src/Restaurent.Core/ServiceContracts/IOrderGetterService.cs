using Restaurent.Core.Domain.Entities;
using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IOrderGetterService
    {
        /// <summary>
        /// Returns orders form the data store
        /// </summary>
        /// <param name="request">Page details for orders</param>
        /// <returns>Returns orders form the data store</returns>
        Task<PaginationResponse<OrderResponse>?> GetOrders(PaginationRequest request);

        /// <summary>
        /// Search for the order based on the order id 
        /// </summary>
        /// <param name="orderId">the order id to search</param>
        /// <returns>Returns the order from the data store</returns>
        Task<OrderResponse?> GetOrderByOrderId(Guid orderId);

        /// <summary>
        /// Returns the orders of the user 
        /// </summary>
        /// <param name="userId">The user orders to search</param>
        /// <returns>Returns the orders of the user from the data store</returns>
        Task<List<OrderResponse>?> GetOrdersByUserId(Guid userId);
    }
}
